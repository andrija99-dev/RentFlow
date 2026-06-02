using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentFlow.Infrastructure.Messaging;
using RentFlow.Infrastructure.Persistence;

namespace RentFlow.Infrastructure.BackgroundJobs;

/// <summary>
/// Recurring job that relays the transactional outbox to the message bus. It reads a
/// batch of unpublished messages oldest-first, publishes each one and stamps it
/// processed. It stops at the first failure so ordering is preserved and a downed
/// broker is retried on the next run rather than hammered.
/// </summary>
internal sealed class OutboxPublisherJob(
    RentFlowDbContext dbContext,
    IMessageBusPublisher publisher,
    ILogger<OutboxPublisherJob> logger)
{
    /// <summary>The recurring job identifier registered with Hangfire.</summary>
    public const string RecurringJobId = "outbox-publisher";

    private const int BatchSize = 50;

    private readonly RentFlowDbContext _dbContext = dbContext;
    private readonly IMessageBusPublisher _publisher = publisher;
    private readonly ILogger<OutboxPublisherJob> _logger = logger;

    /// <summary>Publishes a batch of pending outbox messages to the bus.</summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (!_publisher.IsEnabled)
        {
            return;
        }

        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (messages.Count == 0)
        {
            return;
        }

        var published = 0;
        foreach (var message in messages)
        {
            try
            {
                await _publisher
                    .PublishAsync(
                        new OutboxIntegrationMessage(message.Id, message.Type, message.Payload, message.OccurredOnUtc),
                        cancellationToken)
                    .ConfigureAwait(false);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
                published++;
            }
            catch (Exception ex)
            {
                message.Error = ex.Message;
                _logger.LogError(ex, "Failed to publish outbox message {MessageId} ({Type}).", message.Id, message.Type);
                break;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Outbox relay published {Published} of {Total} pending messages.", published, messages.Count);
    }
}
