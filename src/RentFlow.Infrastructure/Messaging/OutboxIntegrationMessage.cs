namespace RentFlow.Infrastructure.Messaging;

/// <summary>
/// The envelope published to the message bus for a relayed outbox message. Carries
/// the event type and its serialized payload so downstream consumers (webhook
/// delivery, email notifications) can route and forward it without sharing the
/// domain event types.
/// </summary>
/// <param name="Id">The originating event identifier (also the deduplication key).</param>
/// <param name="Type">The short name of the originating domain event type.</param>
/// <param name="Payload">The serialized event payload (JSON).</param>
/// <param name="OccurredOnUtc">The UTC time at which the event occurred.</param>
internal sealed record OutboxIntegrationMessage(
    Guid Id,
    string Type,
    string Payload,
    DateTime OccurredOnUtc);
