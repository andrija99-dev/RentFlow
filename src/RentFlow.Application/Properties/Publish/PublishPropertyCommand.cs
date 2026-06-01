using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Publish;

/// <summary>Publishes a draft listing, making it available for rental applications.</summary>
/// <param name="PropertyId">The identifier of the listing to publish.</param>
public sealed record PublishPropertyCommand(Guid PropertyId) : ICommand;
