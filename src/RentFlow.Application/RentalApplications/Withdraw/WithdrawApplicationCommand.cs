using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Withdraw;

public sealed record WithdrawApplicationCommand(Guid ApplicationId) : ICommand;
