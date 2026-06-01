using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Domain.Entities;

/// <summary>
/// A scheduled rental payment for a contract. Aggregate root so payment reminders
/// and settlement can be processed independently of the contract.
/// </summary>
public sealed class Payment : AggregateRoot
{
    private Payment(Guid id, Guid contractId, DateOnly dueDate, Money amount) : base(id)
    {
        ContractId = contractId;
        DueDate = dueDate;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    private Payment()
    {
    }

    public Guid ContractId { get; private set; }

    public DateOnly DueDate { get; private set; }

    public DateTime? PaidAtUtc { get; private set; }

    public Money Amount { get; private set; } = null!;

    public PaymentStatus Status { get; private set; }

    /// <summary>Schedules a new pending payment for a contract.</summary>
    /// <exception cref="DomainException">Thrown when the contract identifier is empty.</exception>
    public static Payment Create(Guid contractId, DateOnly dueDate, Money amount)
    {
        if (contractId == Guid.Empty)
        {
            throw new DomainException("A payment must reference a contract.");
        }

        return new Payment(Guid.CreateVersion7(), contractId, dueDate, amount);
    }

    /// <summary>Records the payment as settled at the given time.</summary>
    /// <exception cref="DomainException">Thrown when the payment has already been settled.</exception>
    public void MarkAsPaid(DateTime paidAtUtc)
    {
        if (Status == PaymentStatus.Paid)
        {
            throw new DomainException("Payment has already been settled.");
        }

        Status = PaymentStatus.Paid;
        PaidAtUtc = paidAtUtc;
    }

    /// <summary>Marks a still-pending payment as overdue.</summary>
    public void MarkAsOverdue()
    {
        if (Status == PaymentStatus.Pending)
        {
            Status = PaymentStatus.Overdue;
        }
    }
}
