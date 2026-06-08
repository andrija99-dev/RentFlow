using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Payments.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Payments.GetForContract;

internal sealed class GetPaymentsForContractQueryHandler(
    IContractReadService contractReads,
    IPaymentReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetPaymentsForContractQuery, IReadOnlyList<PaymentResponse>>
{
    private readonly IContractReadService _contractReads = contractReads;
    private readonly IPaymentReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<PaymentResponse>> Handle(
        GetPaymentsForContractQuery request,
        CancellationToken cancellationToken)
    {
        var contract = await _contractReads.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException(nameof(Contract), request.ContractId);

        PaymentAuthorization.EnsureCanView(contract.TenantId, contract.PropertyOwnerId, _currentUser);

        return await _readService.GetByContractAsync(request.ContractId, cancellationToken);
    }
}
