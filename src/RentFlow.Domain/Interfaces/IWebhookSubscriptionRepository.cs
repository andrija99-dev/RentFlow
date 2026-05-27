using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;

namespace RentFlow.Domain.Interfaces;

/// <summary>Write-side repository for the <see cref="WebhookSubscription"/> aggregate.</summary>
public interface IWebhookSubscriptionRepository : IRepository<WebhookSubscription>
{
    /// <summary>Retrieves the active subscriptions interested in a given event type.</summary>
    /// <param name="eventType">The event type to match.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The active subscriptions for the event type.</returns>
    Task<IReadOnlyList<WebhookSubscription>> GetActiveByEventTypeAsync(WebhookEventType eventType, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all subscriptions registered by a given owner.</summary>
    /// <param name="ownerId">The owner's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The owner's subscriptions.</returns>
    Task<IReadOnlyList<WebhookSubscription>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
