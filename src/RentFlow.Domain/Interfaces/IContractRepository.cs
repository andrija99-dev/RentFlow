using RentFlow.Domain.Entities;

namespace RentFlow.Domain.Interfaces;

/// <summary>Write-side repository for the <see cref="Contract"/> aggregate.</summary>
public interface IContractRepository : IRepository<Contract>
{
    /// <summary>Retrieves the contract generated from a given application, if any.</summary>
    /// <param name="rentalApplicationId">The application's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract, or <see langword="null"/> if none exists.</returns>
    Task<Contract?> GetByApplicationAsync(Guid rentalApplicationId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves active contracts whose end date is on or before the given date.</summary>
    /// <param name="asOf">The date to evaluate expiry against.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contracts eligible for expiry.</returns>
    Task<IReadOnlyList<Contract>> GetExpiredActiveContractsAsync(DateOnly asOf, CancellationToken cancellationToken = default);
}
