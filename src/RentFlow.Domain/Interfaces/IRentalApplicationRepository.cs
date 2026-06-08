using RentFlow.Domain.Entities;

namespace RentFlow.Domain.Interfaces;

/// <summary>Write-side repository for the <see cref="RentalApplication"/> aggregate.</summary>
public interface IRentalApplicationRepository : IRepository<RentalApplication>
{
    /// <summary>Retrieves all applications submitted for a given property.</summary>
    /// <param name="propertyId">The property's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The applications for the property.</returns>
    Task<IReadOnlyList<RentalApplication>> GetByPropertyAsync(Guid propertyId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all applications submitted by a given tenant.</summary>
    /// <param name="tenantId">The tenant's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The applications submitted by the tenant.</returns>
    Task<IReadOnlyList<RentalApplication>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Determines whether a tenant already has a pending application for a property.</summary>
    /// <param name="propertyId">The property's identifier.</param>
    /// <param name="tenantId">The tenant's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><see langword="true"/> if a pending application exists; otherwise <see langword="false"/>.</returns>
    Task<bool> HasPendingApplicationAsync(Guid propertyId, Guid tenantId, CancellationToken cancellationToken = default);
}
