namespace RentFlow.Application.Abstractions.Email;

/// <summary>A plain-text email to be delivered to a single recipient.</summary>
/// <param name="To">The recipient email address.</param>
/// <param name="Subject">The email subject line.</param>
/// <param name="Body">The plain-text email body.</param>
public sealed record EmailMessage(string To, string Subject, string Body);
