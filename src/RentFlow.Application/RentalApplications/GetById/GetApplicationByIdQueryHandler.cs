using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.RentalApplications.GetById;

/// <summary>Loads the application via Dapper and authorizes the caller to view it.</summary>
internal sealed class GetApplicationByIdQueryHandler(
    IRentalApplicationReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetApplicationByIdQuery, RentalApplicationResponse>
{
    private readonly IRentalApplicationReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<RentalApplicationResponse> Handle(
        GetApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _readService.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(RentalApplication), request.ApplicationId);

        ApplicationAuthorization.EnsureCanView(application.TenantId, application.PropertyOwnerId, _currentUser);

        return application;
    }
}
