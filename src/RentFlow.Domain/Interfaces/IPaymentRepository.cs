using RentFlow.Domain.Entities;

namespace RentFlow.Domain.Interfaces;

/// <summary>Write-side repository for the <see cref="Payment"/> aggregate.</summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>Retrieves all payments scheduled for a given contract.</summary>
    /// <param name="contractId">The contract's identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract's payments.</returns>
    Task<IReadOnlyList<Payment>> GetByContractAsync(Guid contractId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves pending payments due on or before the given date (used by reminder jobs).</summary>
    /// <param name="dueOnOrBefore">The cut-off due date.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The pending payments due by the given date.</returns>
    Task<IReadOnlyList<Payment>> GetDuePaymentsAsync(DateOnly dueOnOrBefore, CancellationToken cancellationToken = default);
}
