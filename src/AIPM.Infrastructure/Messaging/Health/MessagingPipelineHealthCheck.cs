using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AIPM.Infrastructure.Messaging.Health;

/// <summary>
/// Readiness check for platform messaging publish/consume pipeline.
/// </summary>
public sealed class MessagingPipelineHealthCheck : IHealthCheck
{
    private readonly MessagingPipelineHealthState _state;

    /// <summary>Initializes health check.</summary>
    public MessagingPipelineHealthCheck(MessagingPipelineHealthState state)
    {
        _state = state;
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _state.IsHealthy
                ? HealthCheckResult.Healthy("messaging pipeline healthy")
                : HealthCheckResult.Unhealthy("messaging pipeline has unresolved failures"));
    }
}
