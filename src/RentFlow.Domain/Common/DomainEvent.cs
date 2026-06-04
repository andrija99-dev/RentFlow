namespace RentFlow.Domain.Common;

/// <summary>
/// Base record for domain events. Supplies a unique identifier and an occurrence
/// timestamp so concrete events only need to declare their payload.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.CreateVersion7();

    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
