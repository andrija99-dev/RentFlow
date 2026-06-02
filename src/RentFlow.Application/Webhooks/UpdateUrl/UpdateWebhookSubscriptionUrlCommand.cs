using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Webhooks.UpdateUrl;

public sealed record UpdateWebhookSubscriptionUrlCommand(Guid SubscriptionId, string TargetUrl) : ICommand;
