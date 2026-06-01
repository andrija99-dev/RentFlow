using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetById;

/// <summary>Retrieves a single contract; visible to its tenant, the property owner or an admin.</summary>
/// <param name="ContractId">The identifier of the contract to retrieve.</param>
public sealed record GetContractByIdQuery(Guid ContractId) : IQuery<ContractResponse>;
