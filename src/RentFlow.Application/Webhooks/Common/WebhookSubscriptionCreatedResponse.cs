using RentFlow.Domain.Enums;

namespace RentFlow.Application.Webhooks.Common;

public sealed record WebhookSubscriptionCreatedResponse(
    Guid Id,
    Guid OwnerId,
    string TargetUrl,
    WebhookEventType EventType,
    bool IsActive,
    DateTime CreatedAtUtc,
    string Secret);
