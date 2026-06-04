using RentFlow.Domain.Enums;

namespace RentFlow.Application.Contracts.Common;

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
