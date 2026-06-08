using System.Text.Json;
using RabbitMQ.Client;

namespace RentFlow.Infrastructure.Messaging;

/// <summary>Publishes integration messages to the durable topic events exchange.</summary>
internal sealed class RabbitMqPublisher(IRabbitMqConnection connection) : IMessageBusPublisher
{
    private readonly IRabbitMqConnection _connection = connection;

    /// <inheritdoc />
    public bool IsEnabled => true;

    /// <inheritdoc />
    public async Task PublishAsync(OutboxIntegrationMessage message, CancellationToken cancellationToken = default)
    {
        var rabbit = await _connection.GetConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var channel = await rabbit.CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await channel.ExchangeDeclareAsync(
            MessagingTopology.EventsExchange,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var body = JsonSerializer.SerializeToUtf8Bytes(message, MessagingJson.Options);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = message.Id.ToString(),
            Type = message.Type,
        };

        await channel.BasicPublishAsync(
            exchange: MessagingTopology.EventsExchange,
            routingKey: message.Type,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
