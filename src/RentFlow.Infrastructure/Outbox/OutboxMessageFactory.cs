using System.Text.Json;
using RentFlow.Domain.Common;
using RentFlow.Infrastructure.Messaging;

namespace RentFlow.Infrastructure.Outbox;

/// <summary>Builds <see cref="OutboxMessage"/> rows from raised domain events.</summary>
internal static class OutboxMessageFactory
{
    /// <summary>Serializes a domain event into a persistable outbox message.</summary>
    public static OutboxMessage Create(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();

        return new OutboxMessage
        {
            Id = domainEvent.EventId,
            Type = eventType.Name,
            Payload = JsonSerializer.Serialize(domainEvent, eventType, MessagingJson.Options),
            OccurredOnUtc = domainEvent.OccurredOnUtc,
        };
    }
}
