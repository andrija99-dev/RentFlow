using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Domain.Entities;

/// <summary>
/// An owner's registration to receive HTTP callbacks when a particular kind of
/// event occurs. The secret is used to sign delivered payloads (HMAC) so
/// receivers can verify authenticity.
/// </summary>
public sealed class WebhookSubscription : AggregateRoot
{
    private WebhookSubscription(
        Guid id,
        Guid ownerId,
        string targetUrl,
        string secret,
        WebhookEventType eventType) : base(id)
    {
        OwnerId = ownerId;
        TargetUrl = targetUrl;
        Secret = secret;
        EventType = eventType;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private WebhookSubscription()
    {
    }

    public Guid OwnerId { get; private set; }

    public string TargetUrl { get; private set; } = null!;

    public string Secret { get; private set; } = null!;

    public WebhookEventType EventType { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>Registers a new active webhook subscription.</summary>
    /// <exception cref="DomainException">Thrown when the owner, URL or secret is missing or the URL is not absolute.</exception>
    public static WebhookSubscription Create(Guid ownerId, string targetUrl, string secret, WebhookEventType eventType)
    {
        if (ownerId == Guid.Empty)
        {
            throw new DomainException("A webhook subscription must have an owner.");
        }

        if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out _))
        {
            throw new DomainException("Webhook target URL must be an absolute URL.");
        }

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new DomainException("Webhook signing secret is required.");
        }

        return new WebhookSubscription(Guid.CreateVersion7(), ownerId, targetUrl.Trim(), secret, eventType);
    }

    /// <summary>Updates the delivery URL.</summary>
    /// <exception cref="DomainException">Thrown when the URL is not absolute.</exception>
    public void UpdateTargetUrl(string targetUrl)
    {
        if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out _))
        {
            throw new DomainException("Webhook target URL must be an absolute URL.");
        }

        TargetUrl = targetUrl.Trim();
    }

    /// <summary>Resumes deliveries for the subscription.</summary>
    public void Activate() => IsActive = true;

    /// <summary>Suspends deliveries for the subscription.</summary>
    public void Deactivate() => IsActive = false;
}
