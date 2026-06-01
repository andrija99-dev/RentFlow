using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RentFlow.Application.Exceptions;
using RentFlow.Domain.Common;

namespace RentFlow.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs unexpected exceptions before letting them propagate.
/// Expected, intentionally-thrown exceptions (validation, domain-rule, not-found and
/// conflict) are left to the global exception handler and are not logged here as errors.
/// </summary>
/// <typeparam name="TRequest">The request type flowing through the pipeline.</typeparam>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger) =>
        _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception) when (exception is not (
            ValidationException or DomainException or NotFoundException or ConflictException))
        {
            _logger.LogError(exception, "Unhandled exception for request {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}
