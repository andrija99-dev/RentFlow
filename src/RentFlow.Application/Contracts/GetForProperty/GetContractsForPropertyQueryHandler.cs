using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Contracts.Common;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Contracts.GetForProperty;

internal sealed class GetContractsForPropertyQueryHandler(
    IPropertyReadService propertyReads,
    IContractReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetContractsForPropertyQuery, IReadOnlyList<ContractResponse>>
{
    private readonly IPropertyReadService _propertyReads = propertyReads;
    private readonly IContractReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ContractResponse>> Handle(
        GetContractsForPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var ownerId = await _propertyReads.GetOwnerIdAsync(request.PropertyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Property), request.PropertyId);

        PropertyAuthorization.EnsureCanManage(ownerId, _currentUser);

        return await _readService.GetByPropertyAsync(request.PropertyId, cancellationToken);
    }
}
