using Microsoft.Extensions.Logging;
using RentFlow.Application.Abstractions.Email;

namespace RentFlow.Infrastructure.Email;

/// <summary>
/// Fallback email sender used when no SMTP host is configured. It logs the message
/// rather than delivering it, so development and demos work without a mail server.
/// </summary>
internal sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger = logger;

    /// <inheritdoc />
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email (no SMTP configured) to {To} | Subject: {Subject}\n{Body}",
            message.To,
            message.Subject,
            message.Body);

        return Task.CompletedTask;
    }
}
