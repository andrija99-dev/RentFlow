using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Search;

/// <summary>Runs a filtered, paged search over the published property catalog.</summary>
/// <param name="SearchTerm">Free-text matched against title and description; optional.</param>
/// <param name="City">Case-insensitive city filter; optional.</param>
/// <param name="Country">Case-insensitive country filter; optional.</param>
/// <param name="Status">Lifecycle status filter; optional.</param>
/// <param name="MinPrice">Inclusive lower bound on the monthly rent; optional.</param>
/// <param name="MaxPrice">Inclusive upper bound on the monthly rent; optional.</param>
/// <param name="OwnerId">Restricts results to a single owner; optional.</param>
/// <param name="Page">The 1-based page number.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record SearchPropertiesQuery(
    string? SearchTerm,
    string? City,
    string? Country,
    PropertyStatus? Status,
    decimal? MinPrice,
    decimal? MaxPrice,
    Guid? OwnerId,
    int Page,
    int PageSize) : IQuery<PagedResult<PropertySummaryResponse>>;
