using RentFlow.Domain.Events;
using RentFlow.Infrastructure.Outbox;

namespace RentFlow.UnitTests.Infrastructure;

public sealed class OutboxMessageFactoryTests
{
    [Fact]
    public void Create_CopiesIdentityTypeAndTimestamp()
    {
        var domainEvent = new RentalApplicationAcceptedEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7());

        var message = OutboxMessageFactory.Create(domainEvent);

        Assert.Equal(domainEvent.EventId, message.Id);
        Assert.Equal(nameof(RentalApplicationAcceptedEvent), message.Type);
        Assert.Equal(domainEvent.OccurredOnUtc, message.OccurredOnUtc);
        Assert.Null(message.ProcessedOnUtc);
    }

    [Fact]
    public void Create_SerializesPayloadWithEventData()
    {
        var tenantId = Guid.CreateVersion7();
        var domainEvent = new RentalApplicationAcceptedEvent(Guid.CreateVersion7(), Guid.CreateVersion7(), tenantId);

        var message = OutboxMessageFactory.Create(domainEvent);

        Assert.Contains(tenantId.ToString(), message.Payload);
    }
}
