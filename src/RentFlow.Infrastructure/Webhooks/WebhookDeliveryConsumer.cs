using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RentFlow.Domain.Interfaces;
using RentFlow.Infrastructure.Messaging;

namespace RentFlow.Infrastructure.Webhooks;

/// <summary>
/// Background consumer that delivers relayed domain events to registered webhook
/// subscriptions. For each message it loads the active subscriptions for the event
/// type and POSTs the payload to each target URL, signed with the subscription's
/// secret (HMAC-SHA256) so receivers can verify authenticity.
/// </summary>
internal sealed class WebhookDeliveryConsumer(
    IRabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    IHttpClientFactory httpClientFactory,
    ILogger<WebhookDeliveryConsumer> logger) : BackgroundService
{
    /// <summary>The named <see cref="HttpClient"/> used for outbound webhook calls.</summary>
    public const string HttpClientName = "webhook-delivery";

    private readonly IRabbitMqConnection _connection = connection;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<WebhookDeliveryConsumer> _logger = logger;

    private IChannel? _channel;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var rabbit = await _connection.GetConnectionAsync(stoppingToken).ConfigureAwait(false);
            _channel = await rabbit.CreateChannelAsync(cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.ExchangeDeclareAsync(
                MessagingTopology.EventsExchange,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.QueueDeclareAsync(
                MessagingTopology.WebhookQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.QueueBindAsync(
                MessagingTopology.WebhookQueue,
                MessagingTopology.EventsExchange,
                MessagingTopology.AllEventsBindingKey,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.BasicQosAsync(0, prefetchCount: 10, global: false, cancellationToken: stoppingToken)
                .ConfigureAwait(false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnReceivedAsync;

            await _channel.BasicConsumeAsync(
                MessagingTopology.WebhookQueue,
                autoAck: false,
                consumer,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("Webhook delivery consumer listening on queue '{Queue}'.", MessagingTopology.WebhookQueue);

            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook delivery consumer stopped; webhooks will not be delivered until the host restarts.");
        }
    }

    private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var channel = _channel!;
        try
        {
            var envelope = JsonSerializer.Deserialize<OutboxIntegrationMessage>(
                eventArgs.Body.Span,
                MessagingJson.Options);

            if (envelope is not null)
            {
                await DeliverAsync(envelope, eventArgs.CancellationToken).ConfigureAwait(false);
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, eventArgs.CancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process webhook message {MessageId}.", eventArgs.BasicProperties.MessageId);
            await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, eventArgs.CancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task DeliverAsync(OutboxIntegrationMessage envelope, CancellationToken cancellationToken)
    {
        var eventType = WebhookEventTypeMap.FromEventName(envelope.Type);
        if (eventType is null)
        {
            return;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var subscriptions = await scope.ServiceProvider
            .GetRequiredService<IWebhookSubscriptionRepository>()
            .GetActiveByEventTypeAsync(eventType.Value, cancellationToken)
            .ConfigureAwait(false);

        if (subscriptions.Count == 0)
        {
            return;
        }

        var client = _httpClientFactory.CreateClient(HttpClientName);

        foreach (var subscription in subscriptions)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, subscription.TargetUrl)
                {
                    Content = new StringContent(envelope.Payload, Encoding.UTF8, "application/json"),
                };

                request.Headers.TryAddWithoutValidation("X-RentFlow-Event", envelope.Type);
                request.Headers.TryAddWithoutValidation("X-RentFlow-Delivery", envelope.Id.ToString());
                request.Headers.TryAddWithoutValidation(
                    "X-RentFlow-Signature",
                    $"sha256={WebhookSignature.Compute(subscription.Secret, envelope.Payload)}");

                using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Delivered {Event} to subscription {Subscription} ({Status}).",
                        envelope.Type,
                        subscription.Id,
                        (int)response.StatusCode);
                }
                else
                {
                    _logger.LogWarning(
                        "Webhook {Subscription} returned {Status} for {Event}.",
                        subscription.Id,
                        (int)response.StatusCode,
                        envelope.Type);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed delivering webhook {Subscription} to {Url}.", subscription.Id, subscription.TargetUrl);
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
