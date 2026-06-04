using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Payments.Common;

namespace RentFlow.Application.Payments.GetById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
