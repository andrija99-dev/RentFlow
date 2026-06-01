using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Reject;

/// <summary>Rejects a pending rental application; only the property owner may do so.</summary>
/// <param name="ApplicationId">The identifier of the application to reject.</param>
public sealed record RejectApplicationCommand(Guid ApplicationId) : ICommand;
