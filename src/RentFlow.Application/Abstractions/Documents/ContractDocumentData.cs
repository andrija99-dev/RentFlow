namespace RentFlow.Application.Abstractions.Documents;

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
