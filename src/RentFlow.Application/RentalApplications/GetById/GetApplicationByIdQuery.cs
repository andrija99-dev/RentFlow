using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetById;

/// <summary>Retrieves a single rental application visible to the calling user.</summary>
/// <param name="ApplicationId">The identifier of the application to retrieve.</param>
public sealed record GetApplicationByIdQuery(Guid ApplicationId) : IQuery<RentalApplicationResponse>;
