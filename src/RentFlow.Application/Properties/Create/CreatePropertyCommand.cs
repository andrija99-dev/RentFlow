using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Properties.Create;

/// <summary>Creates a new property listing owned by the current user, in the draft state.</summary>
/// <param name="Title">The listing title.</param>
/// <param name="Description">The listing description.</param>
/// <param name="Street">The street line of the address.</param>
/// <param name="City">The city or town.</param>
/// <param name="PostalCode">The postal or ZIP code.</param>
/// <param name="Country">The country.</param>
/// <param name="PriceAmount">The advertised monthly rent.</param>
/// <param name="PriceCurrency">The three-letter ISO-4217 currency code.</param>
public sealed record CreatePropertyCommand(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency) : ICommand<PropertyResponse>;
