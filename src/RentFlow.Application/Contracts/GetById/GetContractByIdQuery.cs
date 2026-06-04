using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Contracts.GetById;

public sealed record GetContractByIdQuery(Guid ContractId) : IQuery<ContractResponse>;
