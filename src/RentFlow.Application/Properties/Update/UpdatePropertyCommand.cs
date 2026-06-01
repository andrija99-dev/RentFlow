using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Properties.Update;

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
