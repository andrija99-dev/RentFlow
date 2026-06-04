using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Accept;

public sealed record AcceptApplicationCommand(Guid ApplicationId) : ICommand;
