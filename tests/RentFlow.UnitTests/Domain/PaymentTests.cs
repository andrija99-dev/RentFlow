using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.UnitTests.Domain;

public sealed class PaymentTests
{
    private static Payment NewPending() =>
        Payment.Create(Guid.CreateVersion7(), DateOnly.FromDateTime(DateTime.UtcNow), Money.Create(950m, "EUR"));

    [Fact]
    public void Create_IsPending()
    {
        var payment = NewPending();

        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Null(payment.PaidAtUtc);
    }

    [Fact]
    public void Create_WithEmptyContract_Throws() =>
        Assert.Throws<DomainException>(() =>
            Payment.Create(Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow), Money.Create(950m, "EUR")));

    [Fact]
    public void MarkAsPaid_SetsPaidStatusAndTimestamp()
    {
        var payment = NewPending();
        var paidAt = DateTime.UtcNow;

        payment.MarkAsPaid(paidAt);

        Assert.Equal(PaymentStatus.Paid, payment.Status);
        Assert.Equal(paidAt, payment.PaidAtUtc);
    }

    [Fact]
    public void MarkAsPaid_WhenAlreadyPaid_Throws()
    {
        var payment = NewPending();
        payment.MarkAsPaid(DateTime.UtcNow);

        Assert.Throws<DomainException>(() => payment.MarkAsPaid(DateTime.UtcNow));
    }

    [Fact]
    public void MarkAsOverdue_FromPending_BecomesOverdue()
    {
        var payment = NewPending();

        payment.MarkAsOverdue();

        Assert.Equal(PaymentStatus.Overdue, payment.Status);
    }

    [Fact]
    public void MarkAsOverdue_WhenPaid_IsNoOp()
    {
        var payment = NewPending();
        payment.MarkAsPaid(DateTime.UtcNow);

        payment.MarkAsOverdue();

        Assert.Equal(PaymentStatus.Paid, payment.Status);
    }
}
