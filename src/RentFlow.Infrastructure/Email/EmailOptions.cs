namespace RentFlow.Infrastructure.Email;

/// <summary>SMTP delivery settings, bound from the "Email" configuration section.</summary>
public sealed class EmailOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Email";

    /// <summary>The SMTP host. When empty, emails are logged instead of sent.</summary>
    public string? Host { get; set; }

    /// <summary>The SMTP port.</summary>
    public int Port { get; set; } = 587;

    /// <summary>Whether to negotiate STARTTLS on connect.</summary>
    public bool UseStartTls { get; set; } = true;

    /// <summary>The SMTP user name, when authentication is required.</summary>
    public string? UserName { get; set; }

    /// <summary>The SMTP password, when authentication is required.</summary>
    public string? Password { get; set; }

    /// <summary>The sender address shown on outgoing mail.</summary>
    public string FromAddress { get; set; } = "no-reply@rentflow.local";

    /// <summary>The sender display name shown on outgoing mail.</summary>
    public string FromName { get; set; } = "RentFlow";
}
