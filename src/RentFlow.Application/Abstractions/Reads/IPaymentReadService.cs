using RentFlow.Application.Payments.Common;

namespace RentFlow.Application.Abstractions.Reads;

/// <summary>
/// The read side of the payment feature. Serves payment projections (joined with
/// their contract, application and property) straight from the database via Dapper,
/// bypassing the EF Core write model.
/// </summary>
public interface IPaymentReadService
{
    /// <summary>Loads a single payment together with its property and tenant details.</summary>
    /// <param name="id">The payment identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The payment, or <see langword="null"/> when none exists.</returns>
    Task<PaymentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists every payment scheduled for a given contract, earliest due date first.</summary>
    /// <param name="contractId">The contract identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract's payments.</returns>
    Task<IReadOnlyList<PaymentResponse>> GetByContractAsync(
        Guid contractId,
        CancellationToken cancellationToken = default);

    /// <summary>Lists every payment owed by a given tenant, earliest due date first.</summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The tenant's payments.</returns>
    Task<IReadOnlyList<PaymentResponse>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
