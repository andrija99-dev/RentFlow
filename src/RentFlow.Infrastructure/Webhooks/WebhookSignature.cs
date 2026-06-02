using System.Security.Cryptography;
using System.Text;

namespace RentFlow.Infrastructure.Webhooks;

/// <summary>Computes the HMAC-SHA256 signature receivers use to verify webhook payloads.</summary>
internal static class WebhookSignature
{
    /// <summary>Returns the lower-case hexadecimal HMAC-SHA256 of the payload keyed by the subscription secret.</summary>
    public static string Compute(string secret, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
