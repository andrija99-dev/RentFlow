using RentFlow.Domain.Enums;

namespace RentFlow.API.Webhooks;

public sealed record CreateWebhookSubscriptionRequest(string TargetUrl, WebhookEventType EventType);

public sealed record UpdateWebhookSubscriptionRequest(string TargetUrl);
