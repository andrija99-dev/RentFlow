using RentFlow.Domain.Common;

namespace RentFlow.Domain.Events;

/// <summary>Raised when a contract is generated for an accepted application.</summary>
/// <param name="ContractId">The identifier of the created contract.</param>
/// <param name="RentalApplicationId">The identifier of the application the contract was generated from.</param>
public sealed record ContractCreatedEvent(
    Guid ContractId,
    Guid RentalApplicationId) : DomainEvent;
