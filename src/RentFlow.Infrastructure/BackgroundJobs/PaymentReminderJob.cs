using Microsoft.Extensions.Logging;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.BackgroundJobs;

/// <summary>
/// Recurring job that sweeps scheduled payments: it flags any still-pending payment
/// whose due date has passed as overdue and logs a reminder for those falling due
/// within the lead window. In a fuller system the reminders would be dispatched to
/// the email pipeline; here they are logged.
/// </summary>
internal sealed class PaymentReminderJob(
    IPaymentRepository payments,
    IUnitOfWork unitOfWork,
    ILogger<PaymentReminderJob> logger)
{
    /// <summary>The recurring job identifier registered with Hangfire.</summary>
    public const string RecurringJobId = "payment-reminders";

    /// <summary>How many days ahead a pending payment triggers a reminder.</summary>
    private const int ReminderLeadDays = 7;

    private readonly IPaymentRepository _payments = payments;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<PaymentReminderJob> _logger = logger;

    /// <summary>Runs a single reminder-and-overdue sweep across the due payments.</summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var horizon = today.AddDays(ReminderLeadDays);

        var duePayments = await _payments.GetDuePaymentsAsync(horizon, cancellationToken).ConfigureAwait(false);
        if (duePayments.Count == 0)
        {
            return;
        }

        var newlyOverdue = 0;
        foreach (var payment in duePayments)
        {
            if (payment.DueDate < today)
            {
                payment.MarkAsOverdue();
                _payments.Update(payment);
                newlyOverdue++;

                _logger.LogWarning(
                    "Payment {PaymentId} for contract {ContractId} is overdue (due {DueDate}).",
                    payment.Id,
                    payment.ContractId,
                    payment.DueDate);
            }
            else
            {
                _logger.LogInformation(
                    "Reminder: payment {PaymentId} for contract {ContractId} is due on {DueDate}.",
                    payment.Id,
                    payment.ContractId,
                    payment.DueDate);
            }
        }

        if (newlyOverdue > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation(
            "Payment reminder sweep processed {ProcessedCount} payments and marked {OverdueCount} overdue.",
            duePayments.Count,
            newlyOverdue);
    }
}
