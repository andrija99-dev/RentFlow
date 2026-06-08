using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when an owner rejects a rental application.</summary>
public sealed record RentalApplicationRejectedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
