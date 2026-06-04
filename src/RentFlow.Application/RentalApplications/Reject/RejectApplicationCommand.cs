using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Reject;

public sealed record RejectApplicationCommand(Guid ApplicationId) : ICommand;
