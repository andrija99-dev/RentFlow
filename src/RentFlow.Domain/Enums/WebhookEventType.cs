namespace RentFlow.Domain.Enums;

/// <summary>
/// Categories of domain occurrence that external systems can subscribe to via
/// webhooks.
/// </summary>
public enum WebhookEventType
{
    /// <summary>A tenant submitted a rental application.</summary>
    ApplicationSubmitted = 0,

    /// <summary>An owner accepted a rental application.</summary>
    ApplicationAccepted = 1,

    /// <summary>An owner rejected a rental application.</summary>
    ApplicationRejected = 2,

    /// <summary>A contract was generated for an accepted application.</summary>
    ContractCreated = 3,
}
