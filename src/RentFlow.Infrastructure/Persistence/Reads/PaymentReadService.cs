using Dapper;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Payments.Common;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <inheritdoc />
internal sealed class PaymentReadService(ISqlConnectionFactory connectionFactory) : IPaymentReadService
{
    private const string BaseSql = """
        SELECT
            pay."Id"             AS Id,
            pay."ContractId"     AS ContractId,
            a."PropertyId"       AS PropertyId,
            pr."Title"           AS PropertyTitle,
            pr."OwnerId"         AS PropertyOwnerId,
            a."TenantId"         AS TenantId,
            pay."DueDate"        AS DueDate,
            pay."PaidAtUtc"      AS PaidAtUtc,
            pay."AmountValue"    AS Amount,
            pay."AmountCurrency" AS Currency,
            pay."Status"         AS Status
        FROM "Payments" pay
        JOIN "Contracts" c ON c."Id" = pay."ContractId"
        JOIN "RentalApplications" a ON a."Id" = c."RentalApplicationId"
        JOIN "Properties" pr ON pr."Id" = a."PropertyId"
        """;

    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;

    /// <inheritdoc />
    public async Task<PaymentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE pay.\"Id\" = @Id;";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        return await connection.QuerySingleOrDefaultAsync<PaymentResponse>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaymentResponse>> GetByContractAsync(
        Guid contractId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE pay.\"ContractId\" = @ContractId ORDER BY pay.\"DueDate\";";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<PaymentResponse>(
            new CommandDefinition(sql, new { ContractId = contractId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaymentResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        const string sql = $"{BaseSql} WHERE a.\"TenantId\" = @TenantId ORDER BY pay.\"DueDate\";";

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = await connection.QueryAsync<PaymentResponse>(
            new CommandDefinition(sql, new { TenantId = tenantId }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return rows.ToList();
    }
}
