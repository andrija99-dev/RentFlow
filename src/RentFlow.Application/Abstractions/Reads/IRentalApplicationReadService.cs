using RentFlow.Application.RentalApplications.Common;

namespace RentFlow.Application.Abstractions.Reads;

/// <summary>
/// The read side of the rental-application feature. Serves application projections
/// (joined with their property) straight from the database via Dapper, bypassing the
/// EF Core write model.
/// </summary>
public interface IRentalApplicationReadService
{
    /// <summary>Loads a single application together with its property details.</summary>
    /// <param name="id">The application identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The application, or <see langword="null"/> when none exists.</returns>
    Task<RentalApplicationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists every application submitted for a given property, newest first.</summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The applications for the property.</returns>
    Task<IReadOnlyList<RentalApplicationResponse>> GetByPropertyAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);

    /// <summary>Lists every application submitted by a given tenant, newest first.</summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The tenant's applications.</returns>
    Task<IReadOnlyList<RentalApplicationResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
