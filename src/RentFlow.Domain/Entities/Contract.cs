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

    /// <summary>Gets the identifier of the application this contract was generated from.</summary>
    public Guid RentalApplicationId { get; private set; }

    /// <summary>Gets the first day of the rental period.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>Gets the last day of the rental period.</summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>Gets the agreed monthly rent.</summary>
    public Money MonthlyRent { get; private set; } = null!;

    /// <summary>Gets the blob storage URL of the generated contract document, if uploaded.</summary>
    public string? DocumentBlobUrl { get; private set; }

    /// <summary>Gets the current status of the contract.</summary>
    public ContractStatus Status { get; private set; }

    /// <summary>Gets the UTC timestamp at which the contract was created.</summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Creates an active contract for an accepted application.</summary>
    /// <param name="rentalApplicationId">The accepted application's identifier.</param>
    /// <param name="startDate">The first day of the rental period.</param>
    /// <param name="endDate">The last day of the rental period; must be after <paramref name="startDate"/>.</param>
    /// <param name="monthlyRent">The agreed monthly rent.</param>
    /// <returns>The created <see cref="Contract"/>.</returns>
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
    /// <param name="documentBlobUrl">The blob storage URL of the document.</param>
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
