using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Payments.Common;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Payments.GetById;

internal sealed class GetPaymentByIdQueryHandler(
    IPaymentReadService readService,
    ICurrentUser currentUser) : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    private readonly IPaymentReadService _readService = readService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaymentResponse> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _readService.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        PaymentAuthorization.EnsureCanView(payment.TenantId, payment.PropertyOwnerId, _currentUser);

        return payment;
    }
}
