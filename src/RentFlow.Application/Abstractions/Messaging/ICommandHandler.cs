using MediatR;

namespace RentFlow.Application.Abstractions.Messaging;

/// <summary>Handles a command that returns no value.</summary>
/// <typeparam name="TCommand">The command type handled.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand;

/// <summary>Handles a command that returns a result.</summary>
/// <typeparam name="TCommand">The command type handled.</typeparam>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;
