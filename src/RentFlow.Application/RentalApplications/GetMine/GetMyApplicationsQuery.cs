using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetMine;

public sealed record GetMyApplicationsQuery : IQuery<IReadOnlyList<RentalApplicationResponse>>;
