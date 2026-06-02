using Hangfire;

namespace RentFlow.Infrastructure.BackgroundJobs;

/// <summary>
/// Registers RentFlow's recurring Hangfire jobs. Called once during start-up after
/// the job storage and server are configured.
/// </summary>
public static class BackgroundJobScheduler
{
    /// <summary>Adds (or updates) the recurring payment-reminder and contract-expiry jobs.</summary>
    /// <param name="recurringJobs">The Hangfire recurring job manager.</param>
    public static void ScheduleRecurringJobs(IRecurringJobManager recurringJobs)
    {
        recurringJobs.AddOrUpdate<PaymentReminderJob>(
            PaymentReminderJob.RecurringJobId,
            job => job.RunAsync(CancellationToken.None),
            Cron.Daily());

        recurringJobs.AddOrUpdate<ContractExpiryJob>(
            ContractExpiryJob.RecurringJobId,
            job => job.RunAsync(CancellationToken.None),
            Cron.Daily());
    }
}
