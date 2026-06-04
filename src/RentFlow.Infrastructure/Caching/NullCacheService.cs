using RentFlow.Application.Abstractions.Caching;

namespace RentFlow.Infrastructure.Caching;

/// <summary>
/// A no-op <see cref="ICacheService"/> used when no Redis connection is configured.
/// Every read is a miss and writes are discarded, so query handlers run entirely
/// against the source of truth without any caching overhead.
/// </summary>
internal sealed class NullCacheService : ICacheService
{
    /// <inheritdoc />
    public Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class => factory(cancellationToken);

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public Task<long> IncrementVersionAsync(string key, CancellationToken cancellationToken = default) => Task.FromResult(0L);

    /// <inheritdoc />
    public Task<long> GetVersionAsync(string key, CancellationToken cancellationToken = default) => Task.FromResult(0L);
}
