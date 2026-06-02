using RentFlow.Application.Abstractions.Messaging;
using RentFlow.Application.Payments.Common;

namespace RentFlow.Application.Payments.GetForContract;

public sealed record GetPaymentsForContractQuery(Guid ContractId) : IQuery<IReadOnlyList<PaymentResponse>>;
