using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Contracts.Common;
using RentFlow.Application.Exceptions;

namespace RentFlow.Application.Contracts.GetMine;

internal sealed class GetMyContractsQueryHandler(
    IContractReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetMyContractsQuery, IReadOnlyList<ContractResponse>>
{
    private readonly IContractReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ContractResponse>> Handle(
        GetMyContractsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated tenant.");

        return await _readService.GetByTenantAsync(tenantId, cancellationToken);
    }
}
