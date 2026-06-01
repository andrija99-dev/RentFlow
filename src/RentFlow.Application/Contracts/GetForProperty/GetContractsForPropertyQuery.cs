using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetForProperty;

public sealed record GetContractsForPropertyQuery(Guid PropertyId) : IQuery<IReadOnlyList<ContractResponse>>;
