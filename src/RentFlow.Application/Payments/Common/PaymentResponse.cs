using RentFlow.Domain.Enums;

namespace RentFlow.Application.Payments.Common;

public sealed record PaymentResponse(
    Guid Id,
    Guid ContractId,
    Guid PropertyId,
    string PropertyTitle,
    Guid PropertyOwnerId,
    Guid TenantId,
    DateOnly DueDate,
    DateTime? PaidAtUtc,
    decimal Amount,
    string Currency,
    PaymentStatus Status);
