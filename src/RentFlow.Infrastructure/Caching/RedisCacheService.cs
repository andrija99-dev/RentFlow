using System.Text.Json;
using Microsoft.Extensions.Logging;
using RentFlow.Application.Abstractions.Caching;
using StackExchange.Redis;

namespace RentFlow.Infrastructure.Caching;

/// <summary>
/// A Redis-backed <see cref="ICacheService"/>. Fails open: when Redis is
/// unreachable or a cache operation throws, the call degrades to a cache miss so
/// the request still succeeds against the source of truth.
/// </summary>
internal sealed class RedisCacheService(
    IConnectionMultiplexer connection,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IConnectionMultiplexer _connection = connection;
    private readonly ILogger<RedisCacheService> _logger = logger;

    private IDatabase? Database => _connection.IsConnected ? _connection.GetDatabase() : null;

    /// <inheritdoc />
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var database = Database;

        if (database is not null && TryGet<T>(database, key) is { } cached)
        {
            return cached;
        }

        var value = await factory(cancellationToken).ConfigureAwait(false);

        if (value is not null && database is not null)
        {
            await SetAsync(database, key, value, expiration ?? DefaultExpiration).ConfigureAwait(false);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = Database;
        if (database is null)
        {
            return;
        }

        try
        {
            await database.KeyDeleteAsync(key).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to evict cache key {CacheKey}", key);
        }
    }

    /// <inheritdoc />
    public async Task<long> IncrementVersionAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = Database;
        if (database is null)
        {
            return 0;
        }

        try
        {
            return await database.StringIncrementAsync(key).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to increment cache version {CacheKey}", key);
            return 0;
        }
    }

    /// <inheritdoc />
    public async Task<long> GetVersionAsync(string key, CancellationToken cancellationToken = default)
    {
        var database = Database;
        if (database is null)
        {
            return 0;
        }

        try
        {
            var value = await database.StringGetAsync(key).ConfigureAwait(false);
            return value.HasValue && value.TryParse(out long version) ? version : 0;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to read cache version {CacheKey}", key);
            return 0;
        }
    }

    private T? TryGet<T>(IDatabase database, string key)
        where T : class
    {
        try
        {
            var cached = database.StringGet(key);
            return cached.HasValue ? JsonSerializer.Deserialize<T>((string)cached!, SerializerOptions) : null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to read cache key {CacheKey}", key);
            return null;
        }
    }

    private async Task SetAsync<T>(IDatabase database, string key, T value, TimeSpan expiration)
    {
        try
        {
            var payload = JsonSerializer.Serialize(value, SerializerOptions);
            await database.StringSetAsync(key, payload, expiration).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to write cache key {CacheKey}", key);
        }
    }
}
