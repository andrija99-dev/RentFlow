using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Properties.GetById;

public sealed record GetPropertyByIdQuery(Guid PropertyId) : IQuery<PropertyResponse>;
