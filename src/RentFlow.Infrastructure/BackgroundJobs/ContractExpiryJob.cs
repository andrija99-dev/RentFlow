using Microsoft.Extensions.Logging;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.BackgroundJobs;

/// <summary>
/// Recurring job that transitions active contracts whose term has ended to the
/// expired state, so the contract lifecycle advances without manual intervention.
/// </summary>
internal sealed class ContractExpiryJob(
    IContractRepository contracts,
    IUnitOfWork unitOfWork,
    ILogger<ContractExpiryJob> logger)
{
    /// <summary>The recurring job identifier registered with Hangfire.</summary>
    public const string RecurringJobId = "contract-expiry-checks";

    private readonly IContractRepository _contracts = contracts;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ContractExpiryJob> _logger = logger;

    /// <summary>Expires every active contract whose end date is on or before today.</summary>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var expiredContracts = await _contracts.GetExpiredActiveContractsAsync(today, cancellationToken)
            .ConfigureAwait(false);
        if (expiredContracts.Count == 0)
        {
            return;
        }

        foreach (var contract in expiredContracts)
        {
            contract.Expire();
            _contracts.Update(contract);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Expired {ContractCount} contracts whose term ended on or before {AsOf}.",
            expiredContracts.Count,
            today);
    }
}
