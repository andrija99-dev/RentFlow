using MediatR;

namespace RentFlow.Application.Abstractions.Messaging;

/// <summary>Handles a query and returns its data.</summary>
/// <typeparam name="TQuery">The query type handled.</typeparam>
/// <typeparam name="TResponse">The type of the data returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
