using RentFlow.Application.Abstractions.Caching;
using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Properties.Search;

/// <summary>
/// Serves a search page from the cache, falling back to the Dapper read service on a
/// miss. Results are keyed by the current search-version counter so any listing
/// change invalidates the whole result set without per-key bookkeeping.
/// </summary>
internal sealed class SearchPropertiesQueryHandler(
    IPropertyReadService readService,
    ICacheService cache) : IQueryHandler<SearchPropertiesQuery, PagedResult<PropertySummaryResponse>>
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(2);

    private readonly IPropertyReadService _readService = readService;
    private readonly ICacheService _cache = cache;

    public async Task<PagedResult<PropertySummaryResponse>> Handle(
        SearchPropertiesQuery request,
        CancellationToken cancellationToken)
    {
        var criteria = new PropertySearchCriteria(
            request.SearchTerm,
            request.City,
            request.Country,
            request.Status,
            request.MinPrice,
            request.MaxPrice,
            request.OwnerId,
            request.Page,
            request.PageSize);

        var version = await _cache.GetVersionAsync(PropertyCacheKeys.SearchVersion, cancellationToken);

        var result = await _cache.GetOrCreateAsync(
            PropertyCacheKeys.Search(version, criteria),
            async ct => (PagedResult<PropertySummaryResponse>?)await _readService.SearchAsync(criteria, ct),
            CacheLifetime,
            cancellationToken);

        return result!;
    }
}
