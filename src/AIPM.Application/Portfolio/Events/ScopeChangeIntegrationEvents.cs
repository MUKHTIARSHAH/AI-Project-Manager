namespace AIPM.Application.Portfolio.Events;

/// <summary>
/// ScopeChangeRecorded integration contract (CMD-022).
/// Catalog gap: Commands-Events-Catalog has no EVT-022; named for FR-004 / CON-011.
/// Trace: AGG-004, CMD-022, FR-004, CAP-004, ADR-SAD-004.
/// </summary>
public sealed record ScopeChangeRecordedIntegrationEvent
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

    /// <summary>Scope change identifier.</summary>
    public Guid ScopeChangeId { get; init; }

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Scope change title.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Scope change description.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Optional affected-requirement citation text.</summary>
    public string? AffectedRequirementCitation { get; init; }
}

/// <summary>
/// ScopeChangeApproved integration contract (FR-004 approval trail).
/// Trace: AGG-004, FR-004, CAP-004, CON-011, ADR-SAD-004.
/// </summary>
public sealed record ScopeChangeApprovedIntegrationEvent
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

    /// <summary>Scope change identifier.</summary>
    public Guid ScopeChangeId { get; init; }

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Scope change title.</summary>
    public string Title { get; init; } = string.Empty;
}

/// <summary>
/// ScopeChangeRejected integration contract (FR-004 approval trail).
/// Trace: AGG-004, FR-004, CAP-004, CON-011, ADR-SAD-004.
/// </summary>
public sealed record ScopeChangeRejectedIntegrationEvent
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

    /// <summary>Scope change identifier.</summary>
    public Guid ScopeChangeId { get; init; }

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Scope change title.</summary>
    public string Title { get; init; } = string.Empty;
}

/// <summary>
/// ScopeChangeImplemented integration contract (CON-011 lifecycle).
/// Trace: AGG-004, FR-004, CAP-004, CON-011, ADR-SAD-004.
/// </summary>
public sealed record ScopeChangeImplementedIntegrationEvent
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

    /// <summary>Scope change identifier.</summary>
    public Guid ScopeChangeId { get; init; }

    /// <summary>Project identifier.</summary>
    public Guid ProjectId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Scope change title.</summary>
    public string Title { get; init; } = string.Empty;
}
