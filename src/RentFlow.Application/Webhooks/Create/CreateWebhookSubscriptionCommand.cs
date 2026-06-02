using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Application.Webhooks.Create;

public sealed record CreateWebhookSubscriptionCommand(string TargetUrl, WebhookEventType EventType)
    : ICommand<WebhookSubscriptionCreatedResponse>;
