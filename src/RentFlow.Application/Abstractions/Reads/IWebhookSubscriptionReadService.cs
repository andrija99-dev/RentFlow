using RentFlow.Application.Webhooks.Common;

namespace RentFlow.Application.Abstractions.Reads;

/// <summary>
/// The read side of the webhook subscription feature. Serves subscription
/// projections via Dapper, never exposing the signing secret (which is shown only
/// once, at creation).
/// </summary>
public interface IWebhookSubscriptionReadService
{
    /// <summary>Loads a single subscription.</summary>
    /// <param name="id">The subscription identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The subscription, or <see langword="null"/> when none exists.</returns>
    Task<WebhookSubscriptionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists every subscription registered by a given owner, newest first.</summary>
    /// <param name="ownerId">The owner identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The owner's subscriptions.</returns>
    Task<IReadOnlyList<WebhookSubscriptionResponse>> GetByOwnerAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default);
}
