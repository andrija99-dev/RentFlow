using RentFlow.Domain.Enums;

namespace RentFlow.Application.RentalApplications.Common;

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
