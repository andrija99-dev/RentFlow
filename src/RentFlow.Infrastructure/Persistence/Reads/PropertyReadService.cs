using Dapper;
using RentFlow.Application.Abstractions.Reads;
using RentFlow.Application.Common;
using RentFlow.Application.Properties.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <inheritdoc />
internal sealed class PropertyReadService(ISqlConnectionFactory connectionFactory) : IPropertyReadService
{
    private const string DetailSql = """
        SELECT
            "Id", "Title", "Description", "OwnerId", "Status", "CreatedAtUtc",
            "AddressStreet", "AddressCity", "AddressPostalCode", "AddressCountry",
            "PriceAmount", "PriceCurrency"
        FROM "Properties"
        WHERE "Id" = @Id;
        """;

    private const string ImagesSql = """
        SELECT "Id", "BlobUrl", "IsPrimary"
        FROM "PropertyImages"
        WHERE "PropertyId" = @PropertyId
        ORDER BY "IsPrimary" DESC, "Id";
        """;

    private readonly ISqlConnectionFactory _connectionFactory = connectionFactory;

    /// <inheritdoc />
    public async Task<PropertyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var row = await connection.QuerySingleOrDefaultAsync<PropertyDetailRow>(
            new CommandDefinition(DetailSql, new { Id = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        if (row is null)
        {
            return null;
        }

        var images = await connection.QueryAsync<PropertyImageDto>(
            new CommandDefinition(ImagesSql, new { PropertyId = id }, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        return new PropertyResponse(
            row.Id,
            row.Title,
            row.Description,
            new AddressDto(row.AddressStreet, row.AddressCity, row.AddressPostalCode, row.AddressCountry),
            new MoneyDto(row.PriceAmount, row.PriceCurrency),
            row.OwnerId,
            row.Status,
            row.CreatedAtUtc,
            images.ToList());
    }

    /// <inheritdoc />
    public async Task<PagedResult<PropertySummaryResponse>> SearchAsync(
        PropertySearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var (pageSql, countSql, parameters) = PropertySearchQueryBuilder.For(criteria).Build();

        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken)
            .ConfigureAwait(false);

        var totalCount = await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken))
            .ConfigureAwait(false);

        IReadOnlyList<PropertySummaryResponse> items = totalCount == 0
            ? []
            : (await connection.QueryAsync<PropertySummaryResponse>(
                new CommandDefinition(pageSql, parameters, cancellationToken: cancellationToken))
                .ConfigureAwait(false)).ToList();

        return new PagedResult<PropertySummaryResponse>(items, criteria.Page, criteria.PageSize, totalCount);
    }

    private sealed record PropertyDetailRow(
        Guid Id,
        string Title,
        string Description,
        Guid OwnerId,
        PropertyStatus Status,
        DateTime CreatedAtUtc,
        string AddressStreet,
        string AddressCity,
        string AddressPostalCode,
        string AddressCountry,
        decimal PriceAmount,
        string PriceCurrency);
}
