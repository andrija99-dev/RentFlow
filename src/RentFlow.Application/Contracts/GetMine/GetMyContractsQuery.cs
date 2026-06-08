using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetMine;

public sealed record GetMyContractsQuery : IQuery<IReadOnlyList<ContractResponse>>;
