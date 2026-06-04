namespace RentFlow.Application.Abstractions.Caching;

/// <summary>
/// A distributed cache abstraction used by query handlers to apply the cache-aside
/// pattern. Implementations are expected to fail open: when the backing store is
/// unavailable, calls degrade to a cache miss rather than throwing.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Returns the cached value for <paramref name="key"/>, or invokes
    /// <paramref name="factory"/> to produce it, caching any non-null result.
    /// </summary>
    /// <typeparam name="T">The cached value type.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">Produces the value on a cache miss.</param>
    /// <param name="expiration">The time-to-live for a freshly cached value; <see langword="null"/> uses the implementation default.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The cached or freshly produced value, which may be <see langword="null"/>.</returns>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>Removes a single entry from the cache.</summary>
    /// <param name="key">The cache key to evict.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically increments a monotonic version counter, returning the new value.
    /// Used to invalidate whole families of cached query results in O(1) without
    /// scanning keys: the counter forms part of the result cache key, so bumping it
    /// orphans every previously cached entry.
    /// </summary>
    /// <param name="key">The version counter key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The incremented counter value.</returns>
    Task<long> IncrementVersionAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Reads the current value of a version counter, treating a missing counter as zero.</summary>
    /// <param name="key">The version counter key.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The current counter value, or zero when unset.</returns>
    Task<long> GetVersionAsync(string key, CancellationToken cancellationToken = default);
}
