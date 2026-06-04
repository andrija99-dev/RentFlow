namespace RentFlow.Application.Abstractions.Email;

/// <summary>
/// Sends transactional emails. Implementations fail open: when no SMTP host is
/// configured the messages are logged rather than delivered, so development never
/// depends on a live mail server.
/// </summary>
public interface IEmailSender
{
    /// <summary>Delivers a single email message.</summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
