using RentFlow.Application.Contracts.Common;

namespace RentFlow.Application.Abstractions.Reads;

/// <summary>
/// The read side of the contract feature. Serves contract projections (joined with
/// their application and property) straight from the database via Dapper, bypassing
/// the EF Core write model.
/// </summary>
public interface IContractReadService
{
    /// <summary>Loads a single contract together with its property and tenant details.</summary>
    /// <param name="id">The contract identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract, or <see langword="null"/> when none exists.</returns>
    Task<ContractResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Reads only the owner identifier of the property a contract belongs to, for cheap ownership checks.</summary>
    /// <param name="id">The contract identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The owner's identifier, or <see langword="null"/> when no such contract exists.</returns>
    Task<Guid?> GetOwnerIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists every contract belonging to a given tenant, newest first.</summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The tenant's contracts.</returns>
    Task<IReadOnlyList<ContractResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>Lists every contract generated for a given property, newest first.</summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The property's contracts.</returns>
    Task<IReadOnlyList<ContractResponse>> GetByPropertyAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default);
}
