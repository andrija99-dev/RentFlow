namespace RentFlow.Domain.Interfaces;

/// <summary>
/// Coordinates the atomic commit of all changes made within a single business
/// transaction and triggers dispatch of any domain events raised along the way.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists all pending changes as a single atomic operation.</summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
