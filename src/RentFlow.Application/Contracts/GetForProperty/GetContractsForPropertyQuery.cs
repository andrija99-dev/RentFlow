using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetForProperty;

/// <summary>Lists the contracts generated for a property; only the owner may view them.</summary>
/// <param name="PropertyId">The identifier of the property whose contracts are requested.</param>
public sealed record GetContractsForPropertyQuery(Guid PropertyId) : IQuery<IReadOnlyList<ContractResponse>>;
