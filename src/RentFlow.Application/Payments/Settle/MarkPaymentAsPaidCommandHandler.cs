using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Payments.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Application.Payments.Settle;

internal sealed class MarkPaymentAsPaidCommandHandler(
    IPaymentRepository payments,
    IPaymentReadService readService,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : ICommandHandler<MarkPaymentAsPaidCommand>
{
    private readonly IPaymentRepository _payments = payments;
    private readonly IPaymentReadService _readService = readService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(MarkPaymentAsPaidCommand request, CancellationToken cancellationToken)
    {
        var projection = await _readService.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        PaymentAuthorization.EnsureCanSettle(projection.TenantId, _currentUser);

        var payment = await _payments.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        payment.MarkAsPaid(DateTime.UtcNow);

        _payments.Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
