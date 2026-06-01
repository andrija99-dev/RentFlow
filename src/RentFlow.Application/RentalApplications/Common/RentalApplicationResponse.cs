using RentFlow.Domain.Enums;

namespace RentFlow.Application.RentalApplications.Common;

/// <summary>The representation of a rental application returned by the API.</summary>
/// <param name="Id">The application identifier.</param>
/// <param name="PropertyId">The identifier of the property applied for.</param>
/// <param name="PropertyTitle">The title of the property applied for.</param>
/// <param name="PropertyOwnerId">The identifier of the property's owner.</param>
/// <param name="TenantId">The identifier of the applying tenant.</param>
/// <param name="Status">The current status of the application.</param>
/// <param name="Message">The optional message the tenant included.</param>
/// <param name="CreatedAtUtc">The UTC submission timestamp.</param>
/// <param name="DecidedAtUtc">The UTC decision timestamp, or <see langword="null"/> when still pending.</param>
public sealed record RentalApplicationResponse(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    Guid PropertyOwnerId,
    Guid TenantId,
    ApplicationStatus Status,
    string? Message,
    DateTime CreatedAtUtc,
    DateTime? DecidedAtUtc);
