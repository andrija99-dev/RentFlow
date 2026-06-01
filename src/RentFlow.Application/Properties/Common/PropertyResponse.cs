using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

/// <summary>The full representation of a property listing, including its images.</summary>
/// <param name="Id">The listing identifier.</param>
/// <param name="Title">The listing title.</param>
/// <param name="Description">The listing description.</param>
/// <param name="Address">The property's postal address.</param>
/// <param name="Price">The advertised monthly rent.</param>
/// <param name="OwnerId">The identifier of the owner who created the listing.</param>
/// <param name="Status">The current lifecycle status.</param>
/// <param name="CreatedAtUtc">The UTC creation timestamp.</param>
/// <param name="Images">The images attached to the listing.</param>
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

/// <summary>A postal address.</summary>
/// <param name="Street">The street line.</param>
/// <param name="City">The city or town.</param>
/// <param name="PostalCode">The postal or ZIP code.</param>
/// <param name="Country">The country.</param>
public sealed record AddressDto(string Street, string City, string PostalCode, string Country);

/// <summary>A monetary amount in a specific currency.</summary>
/// <param name="Amount">The amount.</param>
/// <param name="Currency">The three-letter ISO-4217 currency code.</param>
public sealed record MoneyDto(decimal Amount, string Currency);

/// <summary>An image attached to a property listing.</summary>
/// <param name="Id">The image identifier.</param>
/// <param name="BlobUrl">The blob storage URL of the image.</param>
/// <param name="IsPrimary">Whether this is the listing's primary image.</param>
public sealed record PropertyImageDto(Guid Id, string BlobUrl, bool IsPrimary);
