namespace RentFlow.Infrastructure.Messaging;

/// <summary>
/// Names of the RabbitMQ exchange and queues that make up RentFlow's event topology.
/// Domain events are relayed to a single topic exchange; each consumer binds its own
/// durable queue so deliveries survive broker restarts.
/// </summary>
internal static class MessagingTopology
{
    /// <summary>The topic exchange all relayed domain events are published to.</summary>
    public const string EventsExchange = "rentflow.events";

    /// <summary>The durable queue the webhook delivery consumer reads from.</summary>
    public const string WebhookQueue = "rentflow.webhooks";

    /// <summary>The durable queue the email notification consumer reads from.</summary>
    public const string EmailQueue = "rentflow.emails";

    /// <summary>The binding pattern every consumer queue uses (every event type).</summary>
    public const string AllEventsBindingKey = "#";
}
