namespace AIPM.Application.Requirements.Events;

/// <summary>
/// EVT-030 RequirementIntaken integration contract.
/// Catalog gap: Commands-Events-Catalog event table; Aggregate Catalog authorizes EVT-030.
/// Trace: AGG-005, CMD-030, FR-010, FR-011, ADR-SAD-004, ADR-005.
/// </summary>
public sealed record RequirementIntakenIntegrationEvent
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

    /// <summary>Requirement identifier.</summary>
    public Guid RequirementId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Owning project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Requirement title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Business lifecycle status name (Draft after intake).</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>Technical Parsed flag (independent of status).</summary>
    public bool Parsed { get; init; }
}

/// <summary>
/// EVT-031 RequirementApproved integration contract.
/// Trace: CMD-031, AGG-005, EVT-031, ADR-SAD-004, ADR-005.
/// </summary>
public sealed record RequirementApprovedIntegrationEvent
{
    public Guid MessageId { get; init; } = Guid.NewGuid();
    public string ContractVersion { get; init; } = "1.0";
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CausationId { get; init; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public Guid RequirementId { get; init; }
    public Guid TenantId { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool Parsed { get; init; }
}
