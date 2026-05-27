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

    /// <summary>Gets the identifier of the contract this payment belongs to.</summary>
    public Guid ContractId { get; private set; }

    /// <summary>Gets the date the payment is due.</summary>
    public DateOnly DueDate { get; private set; }

    /// <summary>Gets the UTC timestamp at which the payment was settled, if any.</summary>
    public DateTime? PaidAtUtc { get; private set; }

    /// <summary>Gets the amount owed.</summary>
    public Money Amount { get; private set; } = null!;

    /// <summary>Gets the current status of the payment.</summary>
    public PaymentStatus Status { get; private set; }

    /// <summary>Schedules a new pending payment for a contract.</summary>
    /// <param name="contractId">The owning contract's identifier.</param>
    /// <param name="dueDate">The date the payment is due.</param>
    /// <param name="amount">The amount owed.</param>
    /// <returns>The created <see cref="Payment"/>.</returns>
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
    /// <param name="paidAtUtc">The UTC timestamp of settlement.</param>
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
