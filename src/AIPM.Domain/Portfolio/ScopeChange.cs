using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Portfolio;

/// <summary>
/// CON-011 ScopeChange entity owned by AGG-004 Project.
/// Catalog gap: Domain-Tactical-Catalog has no ENT-### for ScopeChange; owned child of AGG-004 (ADR-GOV-007).
/// Trace: FR-004, CAP-004, CON-011, CMD-022, ADR-003.
/// </summary>
public sealed class ScopeChange
{
    private ScopeChange(
        Guid id,
        Guid projectId,
        Guid tenantId,
        string title,
        string description,
        string? affectedRequirementCitation,
        ScopeChangeStatus status,
        DateTimeOffset recordedAt,
        DateTimeOffset? decidedAt)
    {
        Id = id;
        ProjectId = projectId;
        TenantId = tenantId;
        Title = title;
        Description = description;
        AffectedRequirementCitation = affectedRequirementCitation;
        Status = status;
        RecordedAt = recordedAt;
        DecidedAt = decidedAt;
    }

    /// <summary>Scope change identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning project identifier.</summary>
    public Guid ProjectId { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Short title for the proposed change.</summary>
    public string Title { get; private set; }

    /// <summary>Detailed description of the scope delta.</summary>
    public string Description { get; private set; }

    /// <summary>
    /// Optional citation of affected requirement ids/text (CON-011 quality rule).
    /// Not a FK to BC-02 Requirement.
    /// </summary>
    public string? AffectedRequirementCitation { get; private set; }

    /// <summary>Lifecycle status (CON-011).</summary>
    public ScopeChangeStatus Status { get; private set; }

    /// <summary>When the change was recorded (UTC).</summary>
    public DateTimeOffset RecordedAt { get; }

    /// <summary>When approved or rejected (UTC).</summary>
    public DateTimeOffset? DecidedAt { get; private set; }

    /// <summary>Creates a Proposed scope change (CMD-022).</summary>
    internal static ScopeChange CreateProposed(
        Guid projectId,
        Guid tenantId,
        string title,
        string description,
        string? affectedRequirementCitation)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationError("Scope change title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ValidationError("Scope change description is required.");
        }

        var citation = string.IsNullOrWhiteSpace(affectedRequirementCitation)
            ? null
            : affectedRequirementCitation.Trim();

        return new ScopeChange(
            Guid.NewGuid(),
            projectId,
            tenantId,
            title.Trim(),
            description.Trim(),
            citation,
            ScopeChangeStatus.Proposed,
            DateTimeOffset.UtcNow,
            null);
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static ScopeChange Rehydrate(
        Guid id,
        Guid projectId,
        Guid tenantId,
        string title,
        string description,
        string? affectedRequirementCitation,
        ScopeChangeStatus status,
        DateTimeOffset recordedAt,
        DateTimeOffset? decidedAt)
        => new(id, projectId, tenantId, title, description, affectedRequirementCitation, status, recordedAt, decidedAt);

    /// <summary>Proposed → Approved.</summary>
    internal void Approve()
    {
        EnsureStatus(ScopeChangeStatus.Proposed, "approve");
        Status = ScopeChangeStatus.Approved;
        DecidedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Proposed → Rejected.</summary>
    internal void Reject()
    {
        EnsureStatus(ScopeChangeStatus.Proposed, "reject");
        Status = ScopeChangeStatus.Rejected;
        DecidedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Approved → Implemented.</summary>
    internal void MarkImplemented()
    {
        EnsureStatus(ScopeChangeStatus.Approved, "implement");
        Status = ScopeChangeStatus.Implemented;
    }

    private void EnsureStatus(ScopeChangeStatus expected, string action)
    {
        if (Status != expected)
        {
            throw new ValidationError(
                $"Cannot {action} scope change '{Id}' in status '{Status}'; expected '{expected}'.");
        }
    }
}

/// <summary>
/// CON-011 ScopeChange lifecycle stages.
/// Trace: FR-004, CAP-004.
/// </summary>
public enum ScopeChangeStatus
{
    /// <summary>Initial status after CMD-022 RecordScopeChange.</summary>
    Proposed = 0,

    /// <summary>Approved for implementation.</summary>
    Approved = 1,

    /// <summary>Rejected; terminal for this request.</summary>
    Rejected = 2,

    /// <summary>Approved change has been implemented.</summary>
    Implemented = 3
}
