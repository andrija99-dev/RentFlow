using Dapper;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <inheritdoc />
internal sealed class WebhookSubscriptionReadService(ISqlConnectionFactory connectionFactory)
    : IWebhookSubscriptionReadService
{
    private const string BaseSql = """
        SELECT
            w."Id"           AS Id,
            w."OwnerId"      AS OwnerId,
            w."TargetUrl"    AS TargetUrl,
            w."EventType"    AS EventType,
            w."IsActive"     AS IsActive,
            w."CreatedAtUtc" AS CreatedAtUtc
        FROM "WebhookSubscriptions" w
        """;

    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;

    /// <inheritdoc />
    public async Task<WebhookSubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE w.\"Id\" = @Id;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        return await connection.QuerySingleOrDefaultAsync<WebhookSubscriptionResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WebhookSubscriptionResponse>> GetByOwnerAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE w.\"OwnerId\" = @OwnerId ORDER BY w.\"CreatedAtUtc\" DESC;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<WebhookSubscriptionResponse>(
            new CommandDefinition(sql, new { OwnerId = ownerId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }
}
