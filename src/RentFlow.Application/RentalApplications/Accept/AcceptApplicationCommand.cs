using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.RentalApplications.Accept;

/// <summary>Accepts a pending rental application; only the property owner may do so.</summary>
/// <param name="ApplicationId">The identifier of the application to accept.</param>
public sealed record AcceptApplicationCommand(Guid ApplicationId) : ICommand;
