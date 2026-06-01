using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Delete;

/// <summary>Permanently removes a property listing and its images.</summary>
/// <param name="PropertyId">The identifier of the listing to delete.</param>
public sealed record DeletePropertyCommand(Guid PropertyId) : ICommand;
