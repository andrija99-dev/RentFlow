using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Domain.Interfaces;

namespace RentFlow.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IPaymentRepository" />
internal sealed class PaymentRepository(RentFlowDbContext dbContext)
    : Repository<Payment>(dbContext), IPaymentRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Payment>> GetByContractAsync(Guid contractId, CancellationToken cancellationToken = default) =>
        await Set
            .Where(p => p.ContractId == contractId)
            .OrderBy(p => p.DueDate)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Payment>> GetDuePaymentsAsync(DateOnly dueOnOrBefore, CancellationToken cancellationToken = default) =>
        await Set
            .Where(p => p.Status == PaymentStatus.Pending && p.DueDate <= dueOnOrBefore)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
