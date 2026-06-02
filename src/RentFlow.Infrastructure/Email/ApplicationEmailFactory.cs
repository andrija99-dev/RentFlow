using RentFlow.Application.Abstractions.Email;
using RentFlow.Domain.Events;

namespace RentFlow.Infrastructure.Email;

/// <summary>Builds the tenant-facing notification email for a rental-application event.</summary>
internal static class ApplicationEmailFactory
{
    /// <summary>
    /// Builds the email for a given event type, or returns <see langword="null"/> when
    /// the event is not one the tenant is notified about.
    /// </summary>
    public static EmailMessage? Build(string eventType, string toEmail, string firstName, string propertyTitle)
    {
        var greeting = $"Hi {firstName},";

        return eventType switch
        {
            nameof(RentalApplicationSubmittedEvent) => new EmailMessage(
                toEmail,
                "We received your rental application",
                $"{greeting}\n\nWe've received your application for \"{propertyTitle}\". "
                + "The owner will review it and you'll hear from us once there's a decision.\n\n— RentFlow"),

            nameof(RentalApplicationAcceptedEvent) => new EmailMessage(
                toEmail,
                "Your rental application was accepted",
                $"{greeting}\n\nGreat news — your application for \"{propertyTitle}\" has been accepted. "
                + "Your rental contract is being prepared and will be available in your account shortly.\n\n— RentFlow"),

            nameof(RentalApplicationRejectedEvent) => new EmailMessage(
                toEmail,
                "Update on your rental application",
                $"{greeting}\n\nThank you for your interest in \"{propertyTitle}\". "
                + "Unfortunately, the owner did not accept your application this time. "
                + "Browse other listings any time on RentFlow.\n\n— RentFlow"),

            _ => null,
        };
    }
}
