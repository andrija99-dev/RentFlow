using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

/// <summary>
/// The normalized filter, sort and paging inputs for a property search. Built from
/// the inbound query and consumed by the Dapper read service, which assembles the
/// SQL via a query builder.
/// </summary>
/// <param name="SearchTerm">Free-text matched against title and description; optional.</param>
/// <param name="City">Case-insensitive city filter; optional.</param>
/// <param name="Country">Case-insensitive country filter; optional.</param>
/// <param name="Status">Lifecycle status filter; optional.</param>
/// <param name="MinPrice">Inclusive lower bound on the monthly rent; optional.</param>
/// <param name="MaxPrice">Inclusive upper bound on the monthly rent; optional.</param>
/// <param name="OwnerId">Restricts results to a single owner; optional.</param>
/// <param name="Page">The 1-based page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record PropertySearchCriteria(
    string? SearchTerm,
    string? City,
    string? Country,
    PropertyStatus? Status,
    decimal? MinPrice,
    decimal? MaxPrice,
    Guid? OwnerId,
    int Page,
    int PageSize)
{
    /// <summary>
    /// Produces a stable, collision-resistant token that uniquely identifies this
    /// set of criteria. Used as part of the cache key for search results.
    /// </summary>
    /// <returns>A canonical string representation of the criteria.</returns>
    public string ToCacheToken() => string.Join(
        '|',
        SearchTerm?.Trim().ToLowerInvariant() ?? string.Empty,
        City?.Trim().ToLowerInvariant() ?? string.Empty,
        Country?.Trim().ToLowerInvariant() ?? string.Empty,
        Status?.ToString() ?? string.Empty,
        MinPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        MaxPrice?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        OwnerId?.ToString() ?? string.Empty,
        Page.ToString(System.Globalization.CultureInfo.InvariantCulture),
        PageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));
}
