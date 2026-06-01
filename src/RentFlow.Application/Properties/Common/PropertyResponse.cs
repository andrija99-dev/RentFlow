using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

public sealed record PropertyResponse(
    Guid Id,
    string Title,
    string Description,
    AddressDto Address,
    MoneyDto Price,
    Guid OwnerId,
    PropertyStatus Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<PropertyImageDto> Images);

public sealed record AddressDto(string Street, string City, string PostalCode, string Country);

public sealed record MoneyDto(decimal Amount, string Currency);

public sealed record PropertyImageDto(Guid Id, string BlobUrl, bool IsPrimary);
