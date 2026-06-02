using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Webhooks.GetMine;

public sealed record GetMyWebhookSubscriptionsQuery : IQuery<IReadOnlyList<WebhookSubscriptionResponse>>;
