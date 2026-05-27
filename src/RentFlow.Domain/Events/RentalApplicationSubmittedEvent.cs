using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when a tenant submits a rental application for a property.</summary>
/// <param name="ApplicationId">The identifier of the submitted application.</param>
/// <param name="PropertyId">The identifier of the property applied for.</param>
/// <param name="TenantId">The identifier of the applying tenant.</param>
public sealed record RentalApplicationSubmittedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
