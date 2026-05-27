using RentFlow.Domain.Common;

namespace RentFlow.Domain.Interfaces;

/// <summary>
/// Generic persistence abstraction for an aggregate root. Write-side operations
/// only; complex read queries are served by Dapper-based read repositories.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type managed by the repository.</typeparam>
public interface IRepository<TAggregate>
    where TAggregate : AggregateRoot
{
    /// <summary>Retrieves an aggregate by its identifier.</summary>
    /// <param name="id">The aggregate's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The aggregate, or <see langword="null"/> if none exists.</returns>
    Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new aggregate to the underlying store.</summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing aggregate as modified.</summary>
    /// <param name="aggregate">The aggregate to update.</param>
    void Update(TAggregate aggregate);

    /// <summary>Removes an aggregate from the underlying store.</summary>
    /// <param name="aggregate">The aggregate to remove.</param>
    void Remove(TAggregate aggregate);
}
