using RentFlow.Application.Properties.Create;
using RentFlow.Application.Webhooks.Common;
using RentFlow.Application.Webhooks.Create;
using RentFlow.Domain.Enums;

namespace RentFlow.UnitTests.Application;

public sealed class ValidatorTests
{
    [Fact]
    public void CreatePropertyValidator_AcceptsValidCommand()
    {
        var validator = new CreatePropertyCommandValidator();
        var command = new CreatePropertyCommand("Loft", "Nice place", "12 Main St", "Lisbon", "1100", "PT", 950m, "EUR");

        Assert.True(validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData("", "EUR", 950)]
    [InlineData("Loft", "EU", 950)]
    [InlineData("Loft", "EUR", 0)]
    [InlineData("Loft", "EUR", -5)]
    public void CreatePropertyValidator_RejectsInvalidCommand(string title, string currency, decimal price)
    {
        var validator = new CreatePropertyCommandValidator();
        var command = new CreatePropertyCommand(title, "desc", "12 Main St", "Lisbon", "1100", currency, price, currency);

        Assert.False(validator.Validate(command).IsValid);
    }

    [Fact]
    public void CreateWebhookValidator_AcceptsHttpsUrl()
    {
        var validator = new CreateWebhookSubscriptionCommandValidator();
        var command = new CreateWebhookSubscriptionCommand("https://example.com/hook", WebhookEventType.ApplicationAccepted);

        Assert.True(validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    [InlineData("/relative")]
    public void CreateWebhookValidator_RejectsNonHttpUrl(string url)
    {
        var validator = new CreateWebhookSubscriptionCommandValidator();
        var command = new CreateWebhookSubscriptionCommand(url, WebhookEventType.ApplicationAccepted);

        Assert.False(validator.Validate(command).IsValid);
    }

    [Fact]
    public void CreateWebhookValidator_RejectsUndefinedEventType()
    {
        var validator = new CreateWebhookSubscriptionCommandValidator();
        var command = new CreateWebhookSubscriptionCommand("https://example.com/hook", (WebhookEventType)99);

        Assert.False(validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData("https://example.com/hook", true)]
    [InlineData("http://localhost:5000/hook", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("not-a-url", false)]
    [InlineData(null, false)]
    public void WebhookUrl_IsHttpAbsolute_ClassifiesCorrectly(string? url, bool expected) =>
        Assert.Equal(expected, WebhookUrl.IsHttpAbsolute(url));
}
