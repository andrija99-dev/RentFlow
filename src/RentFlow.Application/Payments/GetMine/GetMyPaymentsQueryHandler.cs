using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Payments.Common;

namespace RentFlow.Application.Payments.GetMine;

internal sealed class GetMyPaymentsQueryHandler(
    IPaymentReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetMyPaymentsQuery, IReadOnlyList<PaymentResponse>>
{
    private readonly IPaymentReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<PaymentResponse>> Handle(
        GetMyPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.UserId
            ?? throw new ForbiddenAccessException("The current request is not associated with an authenticated tenant.");

        return await _readService.GetByTenantAsync(tenantId, cancellationToken);
    }
}
