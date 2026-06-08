namespace RentFlow.Domain.Common;

/// <summary>
/// Marker contract for domain events — facts that have happened in the domain and
/// may trigger side effects (webhooks, emails, projections). Kept free of any
/// framework dependency so the Domain layer stays pure.
/// </summary>
public interface IDomainEvent
{
    /// <summary>Gets the unique identifier of this event occurrence.</summary>
    Guid EventId { get; }

    /// <summary>Gets the UTC timestamp at which the event occurred.</summary>
    DateTime OccurredOnUtc { get; }
}
