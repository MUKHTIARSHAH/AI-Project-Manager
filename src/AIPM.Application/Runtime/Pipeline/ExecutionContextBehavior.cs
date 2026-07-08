using AIPM.SharedKernel.Execution;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIPM.Application.Runtime.Pipeline;

/// <summary>
/// Ensures an execution context exists for each MediatR request.
/// </summary>
public sealed class ExecutionContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IExecutionContextAccessor _accessor;
    private readonly ILogger<ExecutionContextBehavior<TRequest, TResponse>> _logger;

    /// <summary>Initializes behavior.</summary>
    public ExecutionContextBehavior(
        IExecutionContextAccessor accessor,
        ILogger<ExecutionContextBehavior<TRequest, TResponse>> logger)
    {
        _accessor = accessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_accessor.Current is not null)
        {
            return await next();
        }

        var context = RuntimeExecutionContext.Create(cancellationToken: cancellationToken);
        using (_accessor.Push(context))
        {
            _logger.LogDebug("Execution context established correlation={CorrelationId}", context.CorrelationId);
            return await next();
        }
    }
}
