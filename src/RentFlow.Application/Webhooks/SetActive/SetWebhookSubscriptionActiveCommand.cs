using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Webhooks.SetActive;

public sealed record SetWebhookSubscriptionActiveCommand(Guid SubscriptionId, bool IsActive) : ICommand;
