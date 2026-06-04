using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Contracts.Terminate;

public sealed record TerminateContractCommand(Guid ContractId) : ICommand;
