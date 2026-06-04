using RentFlow.Domain.Common;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;

namespace RentFlow.UnitTests.Domain;

public sealed class WebhookSubscriptionTests
{
    private static WebhookSubscription New() =>
        WebhookSubscription.Create(
            Guid.CreateVersion7(),
            "https://example.com/hook",
            "secret",
            WebhookEventType.ApplicationAccepted);

    [Fact]
    public void Create_IsActiveAndTrimsUrl()
    {
        var subscription = WebhookSubscription.Create(
            Guid.CreateVersion7(),
            "  https://example.com/hook  ",
            "secret",
            WebhookEventType.ApplicationAccepted);

        Assert.True(subscription.IsActive);
        Assert.Equal("https://example.com/hook", subscription.TargetUrl);
    }

    [Fact]
    public void Create_WithEmptyOwner_Throws() =>
        Assert.Throws<DomainException>(() =>
            WebhookSubscription.Create(Guid.Empty, "https://x.com", "s", WebhookEventType.ApplicationSubmitted));

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("/relative/path")]
    public void Create_WithNonAbsoluteUrl_Throws(string url) =>
        Assert.Throws<DomainException>(() =>
            WebhookSubscription.Create(Guid.CreateVersion7(), url, "s", WebhookEventType.ApplicationSubmitted));

    [Fact]
    public void Create_WithBlankSecret_Throws() =>
        Assert.Throws<DomainException>(() =>
            WebhookSubscription.Create(Guid.CreateVersion7(), "https://x.com", "  ", WebhookEventType.ApplicationSubmitted));

    [Fact]
    public void Deactivate_ThenActivate_TogglesIsActive()
    {
        var subscription = New();

        subscription.Deactivate();
        Assert.False(subscription.IsActive);

        subscription.Activate();
        Assert.True(subscription.IsActive);
    }

    [Fact]
    public void UpdateTargetUrl_WithInvalidUrl_Throws() =>
        Assert.Throws<DomainException>(() => New().UpdateTargetUrl("not-a-url"));
}
