using MediatR;

namespace RentFlow.Application.Abstractions.Messaging;

/// <summary>
/// Marker for a command — a request that changes state and returns no value.
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Marker for a command that changes state and returns a result.
/// </summary>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>;
