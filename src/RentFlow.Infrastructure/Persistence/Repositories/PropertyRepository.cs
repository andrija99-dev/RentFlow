using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IPropertyRepository" />
internal sealed class PropertyRepository(RentFlowDbContext dbContext)
    : Repository<Property>(dbContext), IPropertyRepository
{
    /// <inheritdoc />
    public override Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Set.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<Property?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default) =>
        Set.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Property>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default) =>
        await Set
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
