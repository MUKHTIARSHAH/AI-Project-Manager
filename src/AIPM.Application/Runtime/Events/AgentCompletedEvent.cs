namespace AIPM.Application.Runtime.Events;

/// <summary>
/// Published when a platform agent completes execution.
/// </summary>
public sealed record AgentCompletedEvent(
    string AgentId,
    string Output,
    Guid CorrelationId,
    Guid CausationId,
    DateTimeOffset OccurredAt) : IPlatformEvent;
