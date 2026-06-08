namespace RentFlow.Infrastructure.Messaging;

/// <summary>
/// No-op publisher used when no RabbitMQ connection string is configured. It reports
/// itself disabled so the outbox relay skips publishing and messages accumulate in
/// the outbox until a broker is configured.
/// </summary>
internal sealed class NullMessageBusPublisher : IMessageBusPublisher
{
    /// <inheritdoc />
    public bool IsEnabled => false;

    /// <inheritdoc />
    public Task PublishAsync(OutboxIntegrationMessage message, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
