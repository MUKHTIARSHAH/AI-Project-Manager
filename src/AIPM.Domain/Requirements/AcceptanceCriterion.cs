using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Requirements;

/// <summary>
/// CON-014 AcceptanceCriterion entity owned by AGG-005 Requirement.
/// Trace: FR-011, CAP-006, CON-014, AGG-005.
/// </summary>
public sealed class AcceptanceCriterion
{
    private AcceptanceCriterion(
        Guid id,
        Guid requirementId,
        Guid tenantId,
        string statement,
        int sortOrder)
    {
        Id = id;
        RequirementId = requirementId;
        TenantId = tenantId;
        Statement = statement;
        SortOrder = sortOrder;
    }

    /// <summary>Acceptance criterion identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning requirement identifier.</summary>
    public Guid RequirementId { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Testable condition text.</summary>
    public string Statement { get; }

    /// <summary>Display / evaluation order within the requirement.</summary>
    public int SortOrder { get; }

    /// <summary>Creates an acceptance criterion for intake.</summary>
    internal static AcceptanceCriterion Create(
        Guid requirementId,
        Guid tenantId,
        string statement,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(statement))
        {
            throw new ValidationError("Acceptance criterion statement is required.");
        }

        if (sortOrder < 0)
        {
            throw new ValidationError("Acceptance criterion sort order must be non-negative.");
        }

        return new AcceptanceCriterion(
            Guid.NewGuid(),
            requirementId,
            tenantId,
            statement.Trim(),
            sortOrder);
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static AcceptanceCriterion Rehydrate(
        Guid id,
        Guid requirementId,
        Guid tenantId,
        string statement,
        int sortOrder)
        => new(id, requirementId, tenantId, statement, sortOrder);
}
