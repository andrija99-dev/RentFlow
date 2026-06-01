namespace RentFlow.Application.Abstractions.Documents;

/// <summary>The data rendered into a generated rental-contract document.</summary>
/// <param name="ContractId">The contract identifier.</param>
/// <param name="PropertyTitle">The title of the rented property.</param>
/// <param name="Street">The property's street line.</param>
/// <param name="City">The property's city or town.</param>
/// <param name="PostalCode">The property's postal or ZIP code.</param>
/// <param name="Country">The property's country.</param>
/// <param name="OwnerId">The identifier of the property owner (landlord).</param>
/// <param name="TenantId">The identifier of the tenant.</param>
/// <param name="StartDate">The first day of the rental period.</param>
/// <param name="EndDate">The last day of the rental period.</param>
/// <param name="MonthlyRentAmount">The agreed monthly rent amount.</param>
/// <param name="MonthlyRentCurrency">The three-letter ISO-4217 currency code of the rent.</param>
public sealed record ContractDocumentData(
    Guid ContractId,
    string PropertyTitle,
    string Street,
    string City,
    string PostalCode,
    string Country,
    Guid OwnerId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRentAmount,
    string MonthlyRentCurrency);
