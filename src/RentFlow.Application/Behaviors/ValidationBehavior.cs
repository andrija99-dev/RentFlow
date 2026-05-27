using FluentValidation;
using MediatR;

namespace RentFlow.Application.Behaviors;

/// <summary>
/// Pipeline behavior that runs all FluentValidation validators registered for the
/// incoming request before it reaches its handler. Aggregated failures are thrown
/// as a <see cref="ValidationException"/>.
/// </summary>
/// <typeparam name="TRequest">The request type flowing through the pipeline.</typeparam>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Initializes the behavior with the validators registered for <typeparamref name="TRequest"/>.</summary>
    /// <param name="validators">The validators to run; may be empty.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
