using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Common;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IRepository{TAggregate}" />
internal abstract class Repository<TAggregate>(RentFlowDbContext dbContext) : IRepository<TAggregate>
    where TAggregate : AggregateRoot
{
    protected RentFlowDbContext DbContext { get; } = dbContext;

    protected DbSet<TAggregate> Set => DbContext.Set<TAggregate>();

    /// <inheritdoc />
    public virtual Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Set.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default) =>
        await Set.AddAsync(aggregate, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public void Update(TAggregate aggregate) => Set.Update(aggregate);

    /// <inheritdoc />
    public void Remove(TAggregate aggregate) => Set.Remove(aggregate);
}
