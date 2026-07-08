namespace AIPM.Infrastructure.Messaging.Contracts;

/// <summary>
/// Base immutable and versionable contract for platform integration messages.
/// </summary>
public interface IPlatformMessageContract
{
    /// <summary>Unique message identifier for idempotency.</summary>
    Guid MessageId { get; }

    /// <summary>Contract schema version.</summary>
    string ContractVersion { get; }

    /// <summary>Correlation identifier spanning a flow.</summary>
    Guid CorrelationId { get; }

    /// <summary>Causation identifier for parent event/command.</summary>
    Guid CausationId { get; }

    /// <summary>UTC occurrence timestamp.</summary>
    DateTimeOffset OccurredAt { get; }
}
