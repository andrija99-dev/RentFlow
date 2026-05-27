using MediatR;
using Microsoft.Extensions.Logging;

namespace RentFlow.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs the start and successful completion of each request,
/// providing a consistent audit trail across all commands and queries.
/// </summary>
/// <typeparam name="TRequest">The request type flowing through the pipeline.</typeparam>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initializes the behavior with a logger.</summary>
    /// <param name="logger">The logger used to record request handling.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", requestName);

        return response;
    }
}
