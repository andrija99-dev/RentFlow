using RentFlow.Domain.Enums;

namespace RentFlow.Application.Properties.Common;

/// <summary>
/// A lightweight projection of a property listing for search and browse results.
/// Carries only the fields needed to render a result card.
/// </summary>
/// <param name="Id">The listing identifier.</param>
/// <param name="Title">The listing title.</param>
/// <param name="City">The property's city.</param>
/// <param name="Country">The property's country.</param>
/// <param name="PriceAmount">The advertised monthly rent amount.</param>
/// <param name="PriceCurrency">The currency of the rent.</param>
/// <param name="Status">The current lifecycle status.</param>
/// <param name="PrimaryImageUrl">The blob URL of the primary image, or <see langword="null"/> when the listing has no images.</param>
/// <param name="CreatedAtUtc">The UTC creation timestamp.</param>
public sealed record PropertySummaryResponse(
    Guid Id,
    string Title,
    string City,
    string Country,
    decimal PriceAmount,
    string PriceCurrency,
    PropertyStatus Status,
    string? PrimaryImageUrl,
    DateTime CreatedAtUtc);
