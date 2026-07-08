using System.Diagnostics;
using System.Text.Json;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AIPM.Host.Http;

/// <summary>
/// Maps domain and application exceptions to RFC 7807 Problem Details.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private const string _problemTypeBase = "https://aipm.dev/problems/";
    private readonly IExecutionContextAccessor _contextAccessor;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>Initializes handler.</summary>
    public GlobalExceptionHandler(
        IExecutionContextAccessor contextAccessor,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
        _environment = environment;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, typeSuffix, title, detail) = MapException(exception);
        var correlationId = _contextAccessor.Current?.CorrelationId.Value.ToString()
            ?? Activity.Current?.TraceId.ToString()
            ?? httpContext.TraceIdentifier;

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled error correlation={CorrelationId}", correlationId);
        }
        else
        {
            _logger.LogWarning(exception, "Client error {Status} correlation={CorrelationId}", status, correlationId);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = _problemTypeBase + typeSuffix,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["correlationId"] = correlationId;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problem), cancellationToken);
        return true;
    }

    private (int Status, string TypeSuffix, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationError e => (StatusCodes.Status400BadRequest, "validation-error", "Validation Error", e.Message),
            NotFoundError e => (StatusCodes.Status404NotFound, "not-found", "Not Found", e.Message),
            ConflictError e => (StatusCodes.Status409Conflict, "conflict", "Conflict", e.Message),
            UnauthorizedError e => (StatusCodes.Status401Unauthorized, "unauthorized", "Unauthorized", e.Message),
            ForbiddenError e => (StatusCodes.Status403Forbidden, "forbidden", "Forbidden", e.Message),
            InfrastructureError e => (StatusCodes.Status503ServiceUnavailable, "infrastructure-error", "Service Unavailable", e.Message),
            InternalError e => (StatusCodes.Status500InternalServerError, "internal-error", "Internal Server Error", e.Message),
            OperationCanceledException => (StatusCodes.Status400BadRequest, "request-cancelled", "Request Cancelled", "The request was cancelled."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "internal-error",
                "Internal Server Error",
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
        };
    }
}
