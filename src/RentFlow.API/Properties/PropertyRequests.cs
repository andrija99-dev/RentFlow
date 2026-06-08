using RentFlow.Domain.Enums;

namespace RentFlow.API.Properties;

public sealed record CreatePropertyRequest(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency);

public sealed record UpdatePropertyRequest(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency);

public sealed record SearchPropertiesRequest(
    string? SearchTerm,
    string? City,
    string? Country,
    PropertyStatus? Status,
    decimal? MinPrice,
    decimal? MaxPrice,
    Guid? OwnerId,
    int Page = 1,
    int PageSize = 20);
