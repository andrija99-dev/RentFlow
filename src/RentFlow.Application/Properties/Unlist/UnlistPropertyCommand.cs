using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Unlist;

/// <summary>Withdraws a listing from the marketplace.</summary>
/// <param name="PropertyId">The identifier of the listing to unlist.</param>
public sealed record UnlistPropertyCommand(Guid PropertyId) : ICommand;
