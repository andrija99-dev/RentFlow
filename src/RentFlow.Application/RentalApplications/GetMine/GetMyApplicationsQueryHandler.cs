using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.RentalApplications.GetMine;

/// <summary>Lists the current tenant's applications via Dapper, newest first.</summary>
internal sealed class GetMyApplicationsQueryHandler(
    IRentalApplicationReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetMyApplicationsQuery, IReadOnlyList<RentalApplicationResponse>>
{
    private readonly IRentalApplicationReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<RentalApplicationResponse>> Handle(
        GetMyApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated tenant.");

        return await _readService.GetByTenantAsync(tenantId, cancellationToken);
    }
}
