namespace AIPM.Application.Runtime.Resilience;

/// <summary>
/// Applies retry, timeout, and circuit-breaker policies to runtime operations.
/// </summary>
public interface IResilienceExecutor
{
    /// <summary>Executes an action with resilience policies.</summary>
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken);
}
