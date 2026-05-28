using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IWebhookSubscriptionRepository" />
internal sealed class WebhookSubscriptionRepository(RentFlowDbContext dbContext)
    : Repository<WebhookSubscription>(dbContext), IWebhookSubscriptionRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<WebhookSubscription>> GetActiveByEventTypeAsync(WebhookEventType eventType, CancellationToken cancellationToken = default) =>
        await Set
            .Where(w => w.EventType == eventType && w.IsActive)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IReadOnlyList<WebhookSubscription>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default) =>
        await Set
            .Where(w => w.OwnerId == ownerId)
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
