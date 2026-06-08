using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Unlist;

public sealed record UnlistPropertyCommand(Guid PropertyId) : ICommand;
