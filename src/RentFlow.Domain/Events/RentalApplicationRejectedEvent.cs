using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when an owner rejects a rental application.</summary>
/// <param name="ApplicationId">The identifier of the rejected application.</param>
/// <param name="PropertyId">The identifier of the related property.</param>
/// <param name="TenantId">The identifier of the tenant whose application was rejected.</param>
public sealed record RentalApplicationRejectedEvent(
    Guid ApplicationId,
    Guid PropertyId,
    Guid TenantId) : DomainEvent;
