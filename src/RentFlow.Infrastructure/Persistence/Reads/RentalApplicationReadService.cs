using Dapper;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <inheritdoc />
internal sealed class RentalApplicationReadService(ISqlConnectionFactory connectionFactory)
    : IRentalApplicationReadService
{
    private const string BaseSql = """
        SELECT
            a."Id"           AS Id,
            a."PropertyId"   AS PropertyId,
            p."Title"        AS PropertyTitle,
            p."OwnerId"      AS PropertyOwnerId,
            a."TenantId"     AS TenantId,
            a."Status"       AS Status,
            a."Message"      AS Message,
            a."CreatedAtUtc" AS CreatedAtUtc,
            a."DecidedAtUtc" AS DecidedAtUtc
        FROM "RentalApplications" a
        JOIN "Properties" p ON p."Id" = a."PropertyId"
        """;

    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;

    /// <inheritdoc />
    public async Task<RentalApplicationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"Id\" = @Id;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        return await connection.QuerySingleOrDefaultAsync<RentalApplicationResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentalApplicationResponse>> GetByPropertyAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"PropertyId\" = @PropertyId ORDER BY a.\"CreatedAtUtc\" DESC;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<RentalApplicationResponse>(
            new CommandDefinition(sql, new { PropertyId = propertyId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentalApplicationResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"TenantId\" = @TenantId ORDER BY a.\"CreatedAtUtc\" DESC;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<RentalApplicationResponse>(
            new CommandDefinition(sql, new { TenantId = tenantId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }
}
