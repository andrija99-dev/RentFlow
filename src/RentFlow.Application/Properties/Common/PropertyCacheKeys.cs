namespace RentFlow.Application.Properties.Common;

/// <summary>
/// Builds the Redis cache keys used by the property query handlers. Detail entries
/// are keyed by id and evicted individually; search results are keyed by a version
/// counter so the whole family can be invalidated by bumping that counter.
/// </summary>
public static class PropertyCacheKeys
{
    /// <summary>The version counter whose value is woven into every search cache key.</summary>
    public const string SearchVersion = "properties:search:version";

    /// <summary>Returns the cache key for a single property's detail representation.</summary>
    /// <param name="id">The property identifier.</param>
    /// <returns>The detail cache key.</returns>
    public static string Detail(Guid id) => $"property:{id}";

    /// <summary>Returns the cache key for a page of search results at a given version.</summary>
    /// <param name="version">The current value of the <see cref="SearchVersion"/> counter.</param>
    /// <param name="criteria">The search criteria.</param>
    /// <returns>The versioned search cache key.</returns>
    public static string Search(long version, PropertySearchCriteria criteria) =>
        $"properties:search:v{version}:{criteria.ToCacheToken()}";
}
