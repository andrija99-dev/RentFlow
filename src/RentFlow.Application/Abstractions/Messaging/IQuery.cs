using MediatR;

namespace RentFlow.Application.Abstractions.Messaging;

/// <summary>
/// Marker for a query — a read-only request that returns data without changing state.
/// </summary>
/// <typeparam name="TResponse">The type of the data returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>;
