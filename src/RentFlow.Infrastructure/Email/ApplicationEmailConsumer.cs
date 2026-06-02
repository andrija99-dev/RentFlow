using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RentFlow.Application.Abstractions.Email;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Infrastructure.Identity;
using RentFlow.Infrastructure.Messaging;

namespace RentFlow.Infrastructure.Email;

/// <summary>
/// Background consumer that emails tenants when their rental application changes
/// state. It reads relayed events from its own durable queue, resolves the tenant's
/// address and the property title, and sends the matching notification via
/// <see cref="IEmailSender"/>.
/// </summary>
internal sealed class ApplicationEmailConsumer(
    IRabbitMqConnection connection,
    IServiceScopeFactory scopeFactory,
    IEmailSender emailSender,
    ILogger<ApplicationEmailConsumer> logger) : BackgroundService
{
    private readonly IRabbitMqConnection _connection = connection;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly ILogger<ApplicationEmailConsumer> _logger = logger;

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
                MessagingTopology.EmailQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.QueueBindAsync(
                MessagingTopology.EmailQueue,
                MessagingTopology.EventsExchange,
                MessagingTopology.AllEventsBindingKey,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            await _channel.BasicQosAsync(0, prefetchCount: 10, global: false, cancellationToken: stoppingToken)
                .ConfigureAwait(false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnReceivedAsync;

            await _channel.BasicConsumeAsync(
                MessagingTopology.EmailQueue,
                autoAck: false,
                consumer,
                cancellationToken: stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("Email notification consumer listening on queue '{Queue}'.", MessagingTopology.EmailQueue);

            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email notification consumer stopped; emails will not be sent until the host restarts.");
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
                await NotifyAsync(envelope, eventArgs.CancellationToken).ConfigureAwait(false);
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, eventArgs.CancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process email message {MessageId}.", eventArgs.BasicProperties.MessageId);
            await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, eventArgs.CancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task NotifyAsync(OutboxIntegrationMessage envelope, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Deserialize<ApplicationEventPayload>(envelope.Payload, MessagingJson.Options);
        if (payload is null)
        {
            return;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var tenant = await userManager.FindByIdAsync(payload.TenantId.ToString()).ConfigureAwait(false);
        if (tenant?.Email is null)
        {
            _logger.LogWarning("Skipping {Event}: tenant {TenantId} has no email address.", envelope.Type, payload.TenantId);
            return;
        }

        var property = await scope.ServiceProvider
            .GetRequiredService<IPropertyReadService>()
            .GetByIdAsync(payload.PropertyId, cancellationToken)
            .ConfigureAwait(false);

        var message = ApplicationEmailFactory.Build(
            envelope.Type,
            tenant.Email,
            tenant.FirstName,
            property?.Title ?? "your selected property");

        if (message is null)
        {
            return;
        }

        await _emailSender.SendAsync(message, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Sent {Event} notification to tenant {TenantId}.", envelope.Type, payload.TenantId);
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
