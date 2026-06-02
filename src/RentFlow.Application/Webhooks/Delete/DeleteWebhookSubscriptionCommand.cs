using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Webhooks.Delete;

public sealed record DeleteWebhookSubscriptionCommand(Guid SubscriptionId) : ICommand;
