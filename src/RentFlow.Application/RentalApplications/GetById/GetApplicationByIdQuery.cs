using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetById;

public sealed record GetApplicationByIdQuery(Guid ApplicationId) : IQuery<RentalApplicationResponse>;
