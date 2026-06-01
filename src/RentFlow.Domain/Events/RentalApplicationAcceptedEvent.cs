using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when an owner accepts a rental application.</summary>
public sealed record RentalApplicationAcceptedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
