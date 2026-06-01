using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetForProperty;

/// <summary>Lists the applications submitted for a property; only the owner may view them.</summary>
/// <param name="PropertyId">The identifier of the property whose applications to list.</param>
public sealed record GetApplicationsForPropertyQuery(Guid PropertyId)
    : IQuery<IReadOnlyList<RentalApplicationResponse>>;
