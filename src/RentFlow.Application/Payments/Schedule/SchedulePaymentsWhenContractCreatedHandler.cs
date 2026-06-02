using Microsoft.Extensions.Logging;
using RentFlow.Application.DomainEvents;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Events;
using RentFlow.Domain.Interfaces;
using RentFlow.Domain.ValueObjects;

namespace RentFlow.Application.Payments.Schedule;

/// <summary>
/// Observer that schedules the monthly rent payments when a contract is created. One
/// pending payment is created for each month of the lease, due on the contract's
/// start date and every monthly anniversary thereafter. Idempotent: if payments
/// already exist for the contract it does nothing, so a re-published event never
/// produces a duplicate schedule.
/// </summary>
internal sealed class SchedulePaymentsWhenContractCreatedHandler(
    IContractRepository contracts,
    IPaymentRepository payments,
    IUnitOfWork unitOfWork,
    ILogger<SchedulePaymentsWhenContractCreatedHandler> logger)
    : IDomainEventHandler<ContractCreatedEvent>
{
    private readonly IContractRepository _contracts = contracts;
    private readonly IPaymentRepository _payments = payments;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SchedulePaymentsWhenContractCreatedHandler> _logger = logger;

    public async Task Handle(
        DomainEventNotification<ContractCreatedEvent> notification,
        CancellationToken cancellationToken)
    {
        var created = notification.DomainEvent;

        var existing = await _payments.GetByContractAsync(created.ContractId, cancellationToken)
            .ConfigureAwait(false);
        if (existing.Count > 0)
        {
            return;
        }

        var contract = await _contracts.GetByIdAsync(created.ContractId, cancellationToken)
            .ConfigureAwait(false);
        if (contract is null)
        {
            _logger.LogWarning(
                "Skipping payment scheduling for contract {ContractId}: the contract no longer exists.",
                created.ContractId);
            return;
        }

        var months = MonthsBetween(contract.StartDate, contract.EndDate);

        for (var month = 0; month < months; month++)
        {
            var dueDate = contract.StartDate.AddMonths(month);
            var amount = Money.Create(contract.MonthlyRent.Amount, contract.MonthlyRent.Currency);
            var payment = Payment.Create(contract.Id, dueDate, amount);

            await _payments.AddAsync(payment, cancellationToken).ConfigureAwait(false);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Scheduled {PaymentCount} payments for contract {ContractId}.",
            months,
            contract.Id);
    }

    private static int MonthsBetween(DateOnly start, DateOnly end) =>
        ((end.Year - start.Year) * 12) + (end.Month - start.Month);
}
