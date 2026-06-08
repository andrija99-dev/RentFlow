using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when a contract is generated for an accepted application.</summary>
public sealed record ContractCreatedEvent(
    Guid ContractId,
    Guid RentalApplicationId) : DomainEvent;
