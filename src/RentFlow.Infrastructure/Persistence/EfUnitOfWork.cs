using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence;

/// <inheritdoc />
internal sealed class EfUnitOfWork(RentFlowDbContext dbContext) : IUnitOfWork
{
    private readonly RentFlowDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
