using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetMine;

/// <summary>Lists the applications submitted by the current tenant.</summary>
public sealed record GetMyApplicationsQuery : IQuery<IReadOnlyList<RentalApplicationResponse>>;
