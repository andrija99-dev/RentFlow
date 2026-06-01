using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Update;

/// <summary>Updates the editable details of an existing property listing.</summary>
/// <param name="PropertyId">The identifier of the listing to update.</param>
/// <param name="Title">The new title.</param>
/// <param name="Description">The new description.</param>
/// <param name="Street">The new street line.</param>
/// <param name="City">The new city or town.</param>
/// <param name="PostalCode">The new postal or ZIP code.</param>
/// <param name="Country">The new country.</param>
/// <param name="PriceAmount">The new monthly rent.</param>
/// <param name="PriceCurrency">The new three-letter ISO-4217 currency code.</param>
public sealed record UpdatePropertyCommand(
    Guid PropertyId,
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency) : ICommand;
