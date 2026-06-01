using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetMine;

/// <summary>Lists the contracts belonging to the current tenant.</summary>
public sealed record GetMyContractsQuery : IQuery<IReadOnlyList<ContractResponse>>;
