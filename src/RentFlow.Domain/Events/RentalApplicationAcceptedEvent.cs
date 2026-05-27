using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when an owner accepts a rental application.</summary>
/// <param name="ApplicationId">The identifier of the accepted application.</param>
/// <param name="PropertyId">The identifier of the related property.</param>
/// <param name="TenantId">The identifier of the tenant whose application was accepted.</param>
public sealed record RentalApplicationAcceptedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
