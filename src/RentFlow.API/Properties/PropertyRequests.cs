using RentFlow.Domain.Enums;

namespace RentFlow.API.Properties;

/// <summary>The request body for creating a new property listing.</summary>
/// <param name="Title">The listing title.</param>
/// <param name="Description">The listing description.</param>
/// <param name="Street">The street line of the address.</param>
/// <param name="City">The city or town.</param>
/// <param name="PostalCode">The postal or ZIP code.</param>
/// <param name="Country">The country.</param>
/// <param name="PriceAmount">The advertised monthly rent.</param>
/// <param name="PriceCurrency">The three-letter ISO-4217 currency code.</param>
public sealed record CreatePropertyRequest(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency);

/// <summary>The request body for updating an existing property listing.</summary>
/// <param name="Title">The new title.</param>
/// <param name="Description">The new description.</param>
/// <param name="Street">The new street line.</param>
/// <param name="City">The new city or town.</param>
/// <param name="PostalCode">The new postal or ZIP code.</param>
/// <param name="Country">The new country.</param>
/// <param name="PriceAmount">The new monthly rent.</param>
/// <param name="PriceCurrency">The new three-letter ISO-4217 currency code.</param>
public sealed record UpdatePropertyRequest(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency);

/// <summary>The query-string parameters accepted by the property search endpoint.</summary>
/// <param name="SearchTerm">Free-text matched against title and description; optional.</param>
/// <param name="City">Case-insensitive city filter; optional.</param>
/// <param name="Country">Case-insensitive country filter; optional.</param>
/// <param name="Status">Lifecycle status filter; optional.</param>
/// <param name="MinPrice">Inclusive lower bound on the monthly rent; optional.</param>
/// <param name="MaxPrice">Inclusive upper bound on the monthly rent; optional.</param>
/// <param name="OwnerId">Restricts results to a single owner; optional.</param>
/// <param name="Page">The 1-based page number; defaults to 1.</param>
/// <param name="PageSize">The number of items per page; defaults to 20.</param>
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
