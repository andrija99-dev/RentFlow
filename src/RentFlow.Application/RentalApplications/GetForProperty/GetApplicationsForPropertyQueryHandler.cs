using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Application.RentalApplications.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.RentalApplications.GetForProperty;

internal sealed class GetApplicationsForPropertyQueryHandler(
    IPropertyReadService propertyReads,
    IRentalApplicationReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetApplicationsForPropertyQuery, IReadOnlyList<RentalApplicationResponse>>
{
    private readonly IPropertyReadService _propertyReads = propertyReads;
    private readonly IRentalApplicationReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<RentalApplicationResponse>> Handle(
        GetApplicationsForPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = await _propertyReads.GetOwnerIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        PropertyAuthorization.EnsureCanManage(ownerId, _currentUser);

        return await _readService.GetByPropertyAsync(request.PropertyId, cancellationToken);
    }
}
