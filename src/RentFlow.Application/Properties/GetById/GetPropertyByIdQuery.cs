using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Properties.GetById;

/// <summary>Retrieves the full representation of a single property listing.</summary>
/// <param name="PropertyId">The identifier of the listing to retrieve.</param>
public sealed record GetPropertyByIdQuery(Guid PropertyId) : IQuery<PropertyResponse>;
