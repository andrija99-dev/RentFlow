using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Payments.Common;

namespace RentFlow.Application.Payments.GetMine;

public sealed record GetMyPaymentsQuery : IQuery<IReadOnlyList<PaymentResponse>>;
