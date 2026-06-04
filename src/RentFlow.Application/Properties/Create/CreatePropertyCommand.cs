using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Properties.Common;

namespace RentFlow.Application.Properties.Create;

public sealed record CreatePropertyCommand(
    string Title,
    string Description,
    string Street,
    string City,
    string PostalCode,
    string Country,
    decimal PriceAmount,
    string PriceCurrency) : ICommand<PropertyResponse>;
