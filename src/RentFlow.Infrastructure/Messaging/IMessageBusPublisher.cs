namespace RentFlow.Infrastructure.Messaging;

/// <summary>
/// Publishes relayed outbox messages to the message bus. Implementations fail open:
/// when no broker is configured the no-op implementation reports
/// <see cref="IsEnabled"/> as <see langword="false"/> and the outbox relay skips
/// publishing, leaving messages pending until a broker becomes available.
/// </summary>
internal interface IMessageBusPublisher
{
    /// <summary>Gets a value indicating whether a real broker is configured.</summary>
    bool IsEnabled { get; }

    /// <summary>Publishes an integration message to the events exchange.</summary>
    /// <param name="message">The message envelope to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task PublishAsync(OutboxIntegrationMessage message, CancellationToken cancellationToken = default);
}
