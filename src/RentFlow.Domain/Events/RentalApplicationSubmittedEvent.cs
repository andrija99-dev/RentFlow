using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when a tenant submits a rental application for a property.</summary>
public sealed record RentalApplicationSubmittedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
