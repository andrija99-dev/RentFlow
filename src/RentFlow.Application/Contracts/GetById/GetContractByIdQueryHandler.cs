using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Contracts.Common;
using RentFlow.Application.Exceptions;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Contracts.GetById;

/// <summary>Loads the contract via Dapper and authorizes the caller to view it.</summary>
internal sealed class GetContractByIdQueryHandler(
    IContractReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetContractByIdQuery, ContractResponse>
{
    private readonly IContractReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ContractResponse> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await _readService.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException(nameof(Contract), request.ContractId);

        ContractAuthorization.EnsureCanView(contract.TenantId, contract.PropertyOwnerId, _currentUser);

        return contract;
    }
}
