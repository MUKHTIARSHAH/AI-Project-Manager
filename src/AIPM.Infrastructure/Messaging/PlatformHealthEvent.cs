using AIPM.Infrastructure.Messaging.Contracts;

namespace AIPM.Infrastructure.Messaging;

/// <summary>
/// Platform messaging health snapshot event.
/// </summary>
public sealed record PlatformHealthEvent : IPlatformMessageContract
{
    /// <summary>Stable message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = PlatformMessageContractVersions.V1;

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>UTC occurrence timestamp.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Service name.</summary>
    public string ServiceName { get; init; } = "aipm-host";

    /// <summary>Current status value.</summary>
    public string Status { get; init; } = "ready";
}
