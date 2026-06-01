using Dapper;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Contracts.Common;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <inheritdoc />
internal sealed class ContractReadService(ISqlConnectionFactory connectionFactory) : IContractReadService
{
    private const string BaseSql = """
        SELECT
            c."Id"                  AS Id,
            c."RentalApplicationId" AS RentalApplicationId,
            a."PropertyId"          AS PropertyId,
            p."Title"               AS PropertyTitle,
            p."OwnerId"             AS PropertyOwnerId,
            a."TenantId"            AS TenantId,
            c."StartDate"           AS StartDate,
            c."EndDate"             AS EndDate,
            c."MonthlyRentAmount"   AS MonthlyRentAmount,
            c."MonthlyRentCurrency" AS MonthlyRentCurrency,
            c."DocumentBlobUrl"     AS DocumentBlobUrl,
            c."Status"              AS Status,
            c."CreatedAtUtc"        AS CreatedAtUtc
        FROM "Contracts" c
        JOIN "RentalApplications" a ON a."Id" = c."RentalApplicationId"
        JOIN "Properties" p ON p."Id" = a."PropertyId"
        """;

    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;

    /// <inheritdoc />
    public async Task<ContractResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE c.\"Id\" = @Id;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        return await connection.QuerySingleOrDefaultAsync<ContractResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Guid?> GetOwnerIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT p."OwnerId"
            FROM "Contracts" c
            JOIN "RentalApplications" a ON a."Id" = c."RentalApplicationId"
            JOIN "Properties" p ON p."Id" = a."PropertyId"
            WHERE c."Id" = @Id;
            """;

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        return await connection.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContractResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"TenantId\" = @TenantId ORDER BY c.\"CreatedAtUtc\" DESC;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<ContractResponse>(
            new CommandDefinition(sql, new { TenantId = tenantId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContractResponse>> GetByPropertyAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"PropertyId\" = @PropertyId ORDER BY c.\"CreatedAtUtc\" DESC;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<ContractResponse>(
            new CommandDefinition(sql, new { PropertyId = propertyId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }
}
