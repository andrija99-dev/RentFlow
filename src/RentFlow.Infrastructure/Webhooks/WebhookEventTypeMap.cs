using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;

namespace RentFlow.Infrastructure.Webhooks;

/// <summary>Maps a relayed domain event's type name to its subscribable webhook event type.</summary>
internal static class WebhookEventTypeMap
{
    /// <summary>Resolves the webhook event type for an event name, or <see langword="null"/> when it is not subscribable.</summary>
    public static WebhookEventType? FromEventName(string eventName) => eventName switch
    {
        nameof(RentalApplicationSubmittedEvent) => WebhookEventType.ApplicationSubmitted,
        nameof(RentalApplicationAcceptedEvent) => WebhookEventType.ApplicationAccepted,
        nameof(RentalApplicationRejectedEvent) => WebhookEventType.ApplicationRejected,
        nameof(ContractCreatedEvent) => WebhookEventType.ContractCreated,
        _ => null,
    };
}
