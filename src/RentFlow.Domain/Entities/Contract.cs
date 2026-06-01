using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Domain.Entities;

/// <summary>
/// A rental contract generated when an application is accepted. Aggregate root
/// that owns the rental period, the agreed rent and the generated document.
/// </summary>
public sealed class Contract : AggregateRoot
{
    private Contract(
        Guid id,
        Guid rentalApplicationId,
        DateOnly startDate,
        DateOnly endDate,
        Money monthlyRent) : base(id)
    {
        RentalApplicationId = rentalApplicationId;
        StartDate = startDate;
        EndDate = endDate;
        MonthlyRent = monthlyRent;
        Status = ContractStatus.Active;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private Contract()
    {
    }

    public Guid RentalApplicationId { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public Money MonthlyRent { get; private set; } = null!;

    public string? DocumentBlobUrl { get; private set; }

    public ContractStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Creates an active contract for an accepted application.</summary>
    /// <exception cref="DomainException">Thrown when the application is empty or the date range is invalid.</exception>
    public static Contract Create(Guid rentalApplicationId, DateOnly startDate, DateOnly endDate, Money monthlyRent)
    {
        if (rentalApplicationId == Guid.Empty)
        {
            throw new DomainException("A contract must reference a rental application.");
        }

        if (endDate <= startDate)
        {
            throw new DomainException("Contract end date must be after the start date.");
        }

        var contract = new Contract(Guid.CreateVersion7(), rentalApplicationId, startDate, endDate, monthlyRent);
        contract.RaiseDomainEvent(new ContractCreatedEvent(contract.Id, rentalApplicationId));
        return contract;
    }

    /// <summary>Attaches the generated contract document's blob URL.</summary>
    /// <exception cref="DomainException">Thrown when the URL is missing.</exception>
    public void AttachDocument(string documentBlobUrl)
    {
        if (string.IsNullOrWhiteSpace(documentBlobUrl))
        {
            throw new DomainException("Document blob URL is required.");
        }

        DocumentBlobUrl = documentBlobUrl.Trim();
    }

    /// <summary>Marks an active contract as expired once its end date has passed.</summary>
    /// <exception cref="DomainException">Thrown when the contract is not active.</exception>
    public void Expire()
    {
        if (Status != ContractStatus.Active)
        {
            throw new DomainException($"Only active contracts can expire; current status is {Status}.");
        }

        Status = ContractStatus.Expired;
    }

    /// <summary>Terminates an active contract before its scheduled end date.</summary>
    /// <exception cref="DomainException">Thrown when the contract is not active.</exception>
    public void Terminate()
    {
        if (Status != ContractStatus.Active)
        {
            throw new DomainException($"Only active contracts can be terminated; current status is {Status}.");
        }

        Status = ContractStatus.Terminated;
    }
}
