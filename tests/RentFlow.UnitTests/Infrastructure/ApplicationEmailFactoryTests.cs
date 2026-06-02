using RentFlow.Domain.Events;
using RentFlow.Infrastructure.Email;

namespace RentFlow.UnitTests.Infrastructure;

public sealed class ApplicationEmailFactoryTests
{
    [Theory]
    [InlineData(nameof(RentalApplicationSubmittedEvent))]
    [InlineData(nameof(RentalApplicationAcceptedEvent))]
    [InlineData(nameof(RentalApplicationRejectedEvent))]
    public void Build_ForApplicationEvents_AddressesTenantAndMentionsProperty(string eventType)
    {
        var message = ApplicationEmailFactory.Build(eventType, "tim@x.com", "Tim", "Sunny Loft");

        Assert.NotNull(message);
        Assert.Equal("tim@x.com", message!.To);
        Assert.Contains("Tim", message.Body);
        Assert.Contains("Sunny Loft", message.Body);
        Assert.False(string.IsNullOrWhiteSpace(message.Subject));
    }

    [Fact]
    public void Build_AcceptedEvent_SubjectReflectsAcceptance()
    {
        var message = ApplicationEmailFactory.Build(
            nameof(RentalApplicationAcceptedEvent), "tim@x.com", "Tim", "Sunny Loft");

        Assert.Contains("accepted", message!.Subject, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(nameof(ContractCreatedEvent))]
    [InlineData("UnknownEvent")]
    public void Build_ForUnhandledEvents_ReturnsNull(string eventType) =>
        Assert.Null(ApplicationEmailFactory.Build(eventType, "tim@x.com", "Tim", "Sunny Loft"));
}
