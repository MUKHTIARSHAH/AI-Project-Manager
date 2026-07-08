using AIPM.Infrastructure.Messaging.Contracts;

namespace AIPM.Infrastructure.Messaging;

/// <summary>
/// Platform lifecycle event — no business payload in Phase 1.
/// </summary>
public sealed record PlatformStartedEvent : IPlatformMessageContract
{
    /// <summary>Stable message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = PlatformMessageContractVersions.V1;

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>When the platform started.</summary>
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>When the message occurred.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Service name.</summary>
    public string ServiceName { get; init; } = "aipm-host";
}
