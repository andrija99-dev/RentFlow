using RentFlow.Domain.Enums;
using RentFlow.Domain.Events;
using RentFlow.Infrastructure.Webhooks;

namespace RentFlow.UnitTests.Infrastructure;

public sealed class WebhookEventTypeMapTests
{
    [Theory]
    [InlineData(nameof(RentalApplicationSubmittedEvent), WebhookEventType.ApplicationSubmitted)]
    [InlineData(nameof(RentalApplicationAcceptedEvent), WebhookEventType.ApplicationAccepted)]
    [InlineData(nameof(RentalApplicationRejectedEvent), WebhookEventType.ApplicationRejected)]
    [InlineData(nameof(ContractCreatedEvent), WebhookEventType.ContractCreated)]
    public void FromEventName_MapsKnownEvents(string eventName, WebhookEventType expected) =>
        Assert.Equal(expected, WebhookEventTypeMap.FromEventName(eventName));

    [Theory]
    [InlineData("SomethingElseEvent")]
    [InlineData("")]
    public void FromEventName_ReturnsNullForUnknown(string eventName) =>
        Assert.Null(WebhookEventTypeMap.FromEventName(eventName));
}
