using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Contracts.Terminate;

/// <summary>Terminates an active contract before its scheduled end date; only the property owner may do so.</summary>
/// <param name="ContractId">The identifier of the contract to terminate.</param>
public sealed record TerminateContractCommand(Guid ContractId) : ICommand;
