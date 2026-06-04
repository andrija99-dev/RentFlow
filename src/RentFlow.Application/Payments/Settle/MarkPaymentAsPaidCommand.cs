using RentFlow.Application.Abstractions.Messaging;

namespace RentFlow.Application.Payments.Settle;

public sealed record MarkPaymentAsPaidCommand(Guid PaymentId) : ICommand;
