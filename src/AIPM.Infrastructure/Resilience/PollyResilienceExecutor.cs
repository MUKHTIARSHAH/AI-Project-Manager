using AIPM.Application.Runtime.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace AIPM.Infrastructure.Resilience;

/// <summary>
/// Polly-based resilience executor with retry, timeout, and circuit breaker.
/// </summary>
public sealed class PollyResilienceExecutor : IResilienceExecutor
{
    private readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new Polly.Retry.RetryStrategyOptions
        {
            MaxRetryAttempts = 2,
            Delay = TimeSpan.FromMilliseconds(100)
        })
        .AddTimeout(new TimeoutStrategyOptions { Timeout = TimeSpan.FromSeconds(30) })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            MinimumThroughput = 2,
            BreakDuration = TimeSpan.FromSeconds(5)
        })
        .Build();

    /// <inheritdoc />
    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
        => await _pipeline.ExecuteAsync(async ct => await action(ct), cancellationToken);
}
