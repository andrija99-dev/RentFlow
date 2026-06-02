namespace RentFlow.Application.Webhooks.Common;

/// <summary>Validation helpers for webhook target URLs.</summary>
internal static class WebhookUrl
{
    /// <summary>Returns <see langword="true"/> when the value is an absolute http or https URL.</summary>
    public static bool IsHttpAbsolute(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
