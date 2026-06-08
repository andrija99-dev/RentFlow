using System.Net;
using System.Text;
using RentFlow.Application.Abstractions.Documents;

namespace RentFlow.Infrastructure.Documents;

/// <inheritdoc />
internal sealed class ContractDocumentGenerator : IContractDocumentGenerator
{
    /// <inheritdoc />
    public GeneratedDocument Generate(ContractDocumentData data)
    {
        var rent = $"{data.MonthlyRentAmount:0.00} {data.MonthlyRentCurrency}";
        var address = WebUtility.HtmlEncode(
            $"{data.Street}, {data.PostalCode} {data.City}, {data.Country}");
        var title = WebUtility.HtmlEncode(data.PropertyTitle);

        var html = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <title>Rental Contract {data.ContractId}</title>
            </head>
            <body>
                <h1>Rental Contract</h1>
                <p><strong>Contract ID:</strong> {data.ContractId}</p>
                <h2>Property</h2>
                <p>{title}<br />{address}</p>
                <h2>Parties</h2>
                <p><strong>Landlord (owner):</strong> {data.OwnerId}</p>
                <p><strong>Tenant:</strong> {data.TenantId}</p>
                <h2>Terms</h2>
                <p><strong>Rental period:</strong> {data.StartDate:yyyy-MM-dd} to {data.EndDate:yyyy-MM-dd}</p>
                <p><strong>Monthly rent:</strong> {rent}</p>
                <p>
                    By entering into this agreement the tenant agrees to pay the monthly rent for the
                    duration of the rental period and to comply with the terms of the tenancy.
                </p>
            </body>
            </html>
            """;

        return new GeneratedDocument(Encoding.UTF8.GetBytes(html), "text/html", ".html");
    }
}
