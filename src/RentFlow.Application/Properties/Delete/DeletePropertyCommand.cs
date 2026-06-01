using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Delete;

public sealed record DeletePropertyCommand(Guid PropertyId) : ICommand;
