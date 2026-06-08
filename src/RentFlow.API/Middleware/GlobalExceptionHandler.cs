using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Application.Exceptions;
using RentFlow.Domain.Common;

namespace RentFlow.API.Middleware;

/// <summary>
/// Translates unhandled exceptions into RFC 7807 <see cref="ProblemDetails"/>
/// responses, mapping the application's exception types to their HTTP status codes.
/// Unrecognized exceptions become a 500 without leaking internal detail.
/// </summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = MapToProblemDetails(exception);

        if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Request to {Path} failed: {Title} ({Status})",
                httpContext.Request.Path,
                problemDetails.Title,
                problemDetails.Status);
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }

    private static ProblemDetails MapToProblemDetails(Exception exception) => exception switch
    {
        ValidationException validation => CreateValidationProblem(validation),
        NotFoundException => Create(StatusCodes.Status404NotFound, "Resource not found", exception.Message),
        ConflictException => Create(StatusCodes.Status409Conflict, "Conflict", exception.Message),
        ForbiddenAccessException => Create(StatusCodes.Status403Forbidden, "Forbidden", exception.Message),
        AuthenticationException => Create(StatusCodes.Status401Unauthorized, "Authentication failed", exception.Message),
        DomainException => Create(StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
        _ => Create(
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred",
            "An unexpected error occurred while processing the request."),
    };

    private static ProblemDetails Create(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail,
    };

    private static ProblemDetails CreateValidationProblem(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
        };
    }
}
