using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Publish;

public sealed record PublishPropertyCommand(Guid PropertyId) : ICommand;
