using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetForProperty;

public sealed record GetApplicationsForPropertyQuery(Guid PropertyId)
    : IQuery<IReadOnlyList<RentalApplicationResponse>>;
