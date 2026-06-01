using RentFlow.Domain.Enums;

namespace RentFlow.Application.Contracts.Common;

/// <summary>The representation of a rental contract returned by the API.</summary>
/// <param name="Id">The contract identifier.</param>
/// <param name="RentalApplicationId">The identifier of the application the contract was generated from.</param>
/// <param name="PropertyId">The identifier of the rented property.</param>
/// <param name="PropertyTitle">The title of the rented property.</param>
/// <param name="PropertyOwnerId">The identifier of the property's owner.</param>
/// <param name="TenantId">The identifier of the tenant.</param>
/// <param name="StartDate">The first day of the rental period.</param>
/// <param name="EndDate">The last day of the rental period.</param>
/// <param name="MonthlyRentAmount">The agreed monthly rent amount.</param>
/// <param name="MonthlyRentCurrency">The three-letter ISO-4217 currency code of the rent.</param>
/// <param name="DocumentBlobUrl">The locator of the generated contract document, or <see langword="null"/> when not yet attached.</param>
/// <param name="Status">The current status of the contract.</param>
/// <param name="CreatedAtUtc">The UTC creation timestamp.</param>
public sealed record ContractResponse(
    Guid Id,
    Guid RentalApplicationId,
    Guid PropertyId,
    string PropertyTitle,
    Guid PropertyOwnerId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRentAmount,
    string MonthlyRentCurrency,
    string? DocumentBlobUrl,
    ContractStatus Status,
    DateTime CreatedAtUtc);
