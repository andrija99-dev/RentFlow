using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

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
