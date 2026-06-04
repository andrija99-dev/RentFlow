using RentFlow.Infrastructure.Webhooks;

namespace RentFlow.UnitTests.Infrastructure;

public sealed class WebhookSignatureTests
{
    [Fact]
    public void Compute_IsDeterministic()
    {
        var first = WebhookSignature.Compute("secret", "{\"id\":1}");
        var second = WebhookSignature.Compute("secret", "{\"id\":1}");

        Assert.Equal(first, second);
    }

    [Fact]
    public void Compute_ProducesLowercaseHexSha256()
    {
        var signature = WebhookSignature.Compute("secret", "payload");

        Assert.Equal(64, signature.Length);
        Assert.Equal(signature.ToLowerInvariant(), signature);
        Assert.All(signature, c => Assert.Contains(c, "0123456789abcdef"));
    }

    [Fact]
    public void Compute_DifferentSecret_ProducesDifferentSignature()
    {
        var a = WebhookSignature.Compute("secret-a", "payload");
        var b = WebhookSignature.Compute("secret-b", "payload");

        Assert.NotEqual(a, b);
    }
}
