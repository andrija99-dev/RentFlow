using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RentFlow.Application.Behaviors;

/// <summary>
/// Pipeline behavior that times each request and logs a warning when handling
/// exceeds a threshold, helping surface slow operations.
/// </summary>
/// <typeparam name="TRequest">The request type flowing through the pipeline.</typeparam>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long LongRunningThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer = new();

    /// <summary>Initializes the behavior with a logger.</summary>
    /// <param name="logger">The logger used to record slow requests.</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Restart();
        var response = await next();
        _timer.Stop();

        if (_timer.ElapsedMilliseconds > LongRunningThresholdMs)
        {
            _logger.LogWarning(
                "Long-running request: {RequestName} took {ElapsedMilliseconds} ms",
                typeof(TRequest).Name,
                _timer.ElapsedMilliseconds);
        }

        return response;
    }
}
