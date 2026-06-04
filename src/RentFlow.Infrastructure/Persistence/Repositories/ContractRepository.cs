using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IContractRepository" />
internal sealed class ContractRepository(RentFlowDbContext dbContext)
    : Repository<Contract>(dbContext), IContractRepository
{
    /// <inheritdoc />
    public Task<Contract?> GetByApplicationAsync(Guid rentalApplicationId, CancellationToken cancellationToken = default) =>
        Set.FirstOrDefaultAsync(c => c.RentalApplicationId == rentalApplicationId, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Contract>> GetExpiredActiveContractsAsync(DateOnly asOf, CancellationToken cancellationToken = default) =>
        await Set
            .Where(c => c.Status == ContractStatus.Active && c.EndDate <= asOf)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
