using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Withdraw;

/// <summary>Withdraws a pending application; only the tenant who submitted it may do so.</summary>
/// <param name="ApplicationId">The identifier of the application to withdraw.</param>
public sealed record WithdrawApplicationCommand(Guid ApplicationId) : ICommand;
