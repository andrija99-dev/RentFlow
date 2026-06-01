using Dapper;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Infrastructure.Persistence.Reads;

/// <summary>
/// Builder for the property search SQL. Translates a <see cref="PropertySearchCriteria"/>
/// into a parameterized WHERE clause and the matching page and count statements,
/// adding only the predicates the caller actually supplied (Builder pattern).
/// </summary>
internal sealed class PropertySearchQueryBuilder
{
    private const string Table = "\"Properties\"";

    private readonly List<string> _conditions = [];
    private readonly DynamicParameters _parameters = new();

    private PropertySearchQueryBuilder()
    {
    }

    /// <summary>Begins building a query for the supplied criteria.</summary>
    /// <param name="criteria">The filter and paging inputs.</param>
    /// <returns>A configured builder ready to <see cref="Build"/>.</returns>
    public static PropertySearchQueryBuilder For(PropertySearchCriteria criteria)
    {
        var builder = new PropertySearchQueryBuilder();

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            builder._conditions.Add("(p.\"Title\" ILIKE @SearchTerm OR p.\"Description\" ILIKE @SearchTerm)");
            builder._parameters.Add("SearchTerm", $"%{criteria.SearchTerm.Trim()}%");
        }

        if (!string.IsNullOrWhiteSpace(criteria.City))
        {
            builder._conditions.Add("p.\"AddressCity\" ILIKE @City");
            builder._parameters.Add("City", criteria.City.Trim());
        }

        if (!string.IsNullOrWhiteSpace(criteria.Country))
        {
            builder._conditions.Add("p.\"AddressCountry\" ILIKE @Country");
            builder._parameters.Add("Country", criteria.Country.Trim());
        }

        if (criteria.Status is { } status)
        {
            builder._conditions.Add("p.\"Status\" = @Status");
            builder._parameters.Add("Status", status.ToString());
        }

        if (criteria.MinPrice is { } minPrice)
        {
            builder._conditions.Add("p.\"PriceAmount\" >= @MinPrice");
            builder._parameters.Add("MinPrice", minPrice);
        }

        if (criteria.MaxPrice is { } maxPrice)
        {
            builder._conditions.Add("p.\"PriceAmount\" <= @MaxPrice");
            builder._parameters.Add("MaxPrice", maxPrice);
        }

        if (criteria.OwnerId is { } ownerId)
        {
            builder._conditions.Add("p.\"OwnerId\" = @OwnerId");
            builder._parameters.Add("OwnerId", ownerId);
        }

        builder._parameters.Add("Offset", (criteria.Page - 1) * criteria.PageSize);
        builder._parameters.Add("PageSize", criteria.PageSize);

        return builder;
    }

    /// <summary>Composes the final statements and the bound parameters.</summary>
    /// <returns>The page SQL, the count SQL and the shared parameters.</returns>
    public (string PageSql, string CountSql, DynamicParameters Parameters) Build()
    {
        var whereClause = _conditions.Count == 0
            ? string.Empty
            : "WHERE " + string.Join(" AND ", _conditions);

        var pageSql = $"""
            SELECT
                p."Id"            AS {nameof(PropertySummaryResponse.Id)},
                p."Title"         AS {nameof(PropertySummaryResponse.Title)},
                p."AddressCity"   AS {nameof(PropertySummaryResponse.City)},
                p."AddressCountry" AS {nameof(PropertySummaryResponse.Country)},
                p."PriceAmount"   AS {nameof(PropertySummaryResponse.PriceAmount)},
                p."PriceCurrency" AS {nameof(PropertySummaryResponse.PriceCurrency)},
                p."Status"        AS {nameof(PropertySummaryResponse.Status)},
                (
                    SELECT i."BlobUrl"
                    FROM "PropertyImages" i
                    WHERE i."PropertyId" = p."Id" AND i."IsPrimary" = TRUE
                    LIMIT 1
                ) AS {nameof(PropertySummaryResponse.PrimaryImageUrl)},
                p."CreatedAtUtc"  AS {nameof(PropertySummaryResponse.CreatedAtUtc)}
            FROM {Table} p
            {whereClause}
            ORDER BY p."CreatedAtUtc" DESC
            OFFSET @Offset LIMIT @PageSize;
            """;

        var countSql = $"""
            SELECT COUNT(*)
            FROM {Table} p
            {whereClause};
            """;

        return (pageSql, countSql, _parameters);
    }
}
