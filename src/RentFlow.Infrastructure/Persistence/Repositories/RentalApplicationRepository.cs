using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IRentalApplicationRepository" />
internal sealed class RentalApplicationRepository(RentFlowDbContext dbContext)
    : Repository<RentalApplication>(dbContext), IRentalApplicationRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RentalApplication>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default) =>
        await Set
            .Where(a => a.PropertyId == propertyId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IReadOnlyList<RentalApplication>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await Set
            .Where(a => a.TenantId == tenantId)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public Task<bool> HasPendingApplicationAsync(Guid propertyId, Guid tenantId, CancellationToken cancellationToken = default) =>
        Set.AnyAsync(
            a => a.PropertyId == propertyId
                && a.TenantId == tenantId
                && a.Status == ApplicationStatus.Pending,
            cancellationToken);
}
