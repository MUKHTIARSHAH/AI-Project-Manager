namespace AIPM.Infrastructure.Messaging.Health;

/// <summary>
/// Captures messaging publish/consume health signals for readiness checks.
/// </summary>
public sealed class MessagingPipelineHealthState
{
    private long _lastSuccessTicks = DateTimeOffset.UtcNow.UtcTicks;
    private long _lastFailureTicks;

    /// <summary>Records successful publish/consume operation.</summary>
    public void ReportSuccess()
        => Interlocked.Exchange(ref _lastSuccessTicks, DateTimeOffset.UtcNow.UtcTicks);

    /// <summary>Records failed publish/consume operation.</summary>
    public void ReportFailure()
        => Interlocked.Exchange(ref _lastFailureTicks, DateTimeOffset.UtcNow.UtcTicks);

    /// <summary>True when failures do not exceed latest success.</summary>
    public bool IsHealthy
        => Interlocked.Read(ref _lastFailureTicks) <= Interlocked.Read(ref _lastSuccessTicks);
}
