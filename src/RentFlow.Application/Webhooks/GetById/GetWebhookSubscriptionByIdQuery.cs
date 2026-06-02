using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Webhooks.GetById;

public sealed record GetWebhookSubscriptionByIdQuery(Guid SubscriptionId) : IQuery<WebhookSubscriptionResponse>;
