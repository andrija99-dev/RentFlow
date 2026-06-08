using RentFlow.Application.Abstractions.Identity;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Exceptions;
using RentFlow.Application.Payments.Common;
using RentFlow.Application.Payments.Settle;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Application;

public sealed class MarkPaymentAsPaidCommandHandlerTests
{
    private readonly Mock<IPaymentRepository> _payments = new();
    private readonly Mock<IPaymentReadService> _reads = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUser> _currentUser = new();

    private readonly Guid _paymentId = Guid.CreateVersion7();
    private readonly Guid _tenantId = Guid.CreateVersion7();

    private MarkPaymentAsPaidCommandHandler CreateHandler() =>
        new(_payments.Object, _reads.Object, _unitOfWork.Object, _currentUser.Object);

    private PaymentResponse Projection() => new(
        _paymentId,
        Guid.CreateVersion7(),
        Guid.CreateVersion7(),
        "Loft",
        Guid.CreateVersion7(),
        _tenantId,
        DateOnly.FromDateTime(DateTime.UtcNow),
        null,
        950m,
        "EUR",
        PaymentStatus.Pending);

    [Fact]
    public async Task Handle_AsTenant_MarksPaidAndSaves()
    {
        var payment = Payment.Create(Guid.CreateVersion7(), DateOnly.FromDateTime(DateTime.UtcNow), Money.Create(950m, "EUR"));
        _currentUser.SetupGet(u => u.UserId).Returns(_tenantId);
        _currentUser.Setup(u => u.IsInRole(It.IsAny<string>())).Returns(false);
        _reads.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync(Projection());
        _payments.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        await CreateHandler().Handle(new MarkPaymentAsPaidCommand(_paymentId), CancellationToken.None);

        Assert.Equal(PaymentStatus.Paid, payment.Status);
        _payments.Verify(r => r.Update(payment), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenProjectionMissing_ThrowsNotFound()
    {
        _reads.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync((PaymentResponse?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(new MarkPaymentAsPaidCommand(_paymentId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenNotTenantOrAdmin_ThrowsForbidden()
    {
        _currentUser.SetupGet(u => u.UserId).Returns(Guid.CreateVersion7());
        _currentUser.Setup(u => u.IsInRole(It.IsAny<string>())).Returns(false);
        _reads.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync(Projection());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            CreateHandler().Handle(new MarkPaymentAsPaidCommand(_paymentId), CancellationToken.None));

        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAggregateMissing_ThrowsNotFound()
    {
        _currentUser.SetupGet(u => u.UserId).Returns(_tenantId);
        _currentUser.Setup(u => u.IsInRole(It.IsAny<string>())).Returns(false);
        _reads.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync(Projection());
        _payments.Setup(r => r.GetByIdAsync(_paymentId, It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateHandler().Handle(new MarkPaymentAsPaidCommand(_paymentId), CancellationToken.None));
    }
}
