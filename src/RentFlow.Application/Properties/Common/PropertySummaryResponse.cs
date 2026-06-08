using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

public sealed record PropertySummaryResponse(
    Guid Id,
    string Title,
    string City,
    string Country,
    decimal PriceAmount,
    string PriceCurrency,
    PropertyStatus Status,
    string? PrimaryImageUrl,
    DateTime CreatedAtUtc);
