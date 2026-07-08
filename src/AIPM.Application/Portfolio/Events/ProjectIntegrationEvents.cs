namespace AIPM.Application.Portfolio.Events;

/// <summary>
/// EVT-020 ProjectCreated integration contract.
/// Trace: AGG-004, CMD-020, FR-001, ADR-SAD-004.
/// </summary>
public sealed record ProjectCreatedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>Event occurrence time.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Program identifier.</summary>
    public Guid ProgramId { get; init; }

    /// <summary>Workspace reference identifier.</summary>
    public Guid WorkspaceId { get; init; }

    /// <summary>Owner user identifier.</summary>
    public Guid OwnerUserId { get; init; }

    /// <summary>Project name.</summary>
    public string Name { get; init; } = string.Empty;
}

/// <summary>
/// EVT-021 ProjectArchived integration contract.
/// Trace: AGG-004, CMD-021, FR-001, ADR-SAD-004.
/// </summary>
public sealed record ProjectArchivedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>Event occurrence time.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Project name.</summary>
    public string Name { get; init; } = string.Empty;
}
