namespace RentFlow.Infrastructure.Outbox;

/// <summary>
/// A persisted domain event awaiting relay to the message bus. It is written in the
/// same transaction as the originating aggregate change (transactional outbox) and
/// published asynchronously by the outbox processor, guaranteeing at-least-once
/// delivery even if the process crashes immediately after the commit.
/// </summary>
internal sealed class OutboxMessage
{
    /// <summary>Gets the message identifier (the originating event's identifier).</summary>
    public Guid Id { get; init; }

    /// <summary>Gets the short name of the originating domain event type.</summary>
    public required string Type { get; init; }

    /// <summary>Gets the serialized event payload (JSON).</summary>
    public required string Payload { get; init; }

    /// <summary>Gets the UTC time at which the event occurred.</summary>
    public DateTime OccurredOnUtc { get; init; }

    /// <summary>Gets or sets the UTC time at which the message was successfully published, if any.</summary>
    public DateTime? ProcessedOnUtc { get; set; }

    /// <summary>Gets or sets the last publishing error, if a relay attempt failed.</summary>
    public string? Error { get; set; }
}
