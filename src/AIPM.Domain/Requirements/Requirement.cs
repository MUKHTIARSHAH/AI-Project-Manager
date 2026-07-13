using AIPM.Domain.Primitives;
using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Requirements;

/// <summary>
/// AGG-005 Requirement aggregate root (BC-02).
/// Trace: FR-010, FR-011, CAP-005, CAP-006, CON-013, CON-014, CMD-030, EVT-030, ADR-005.
/// </summary>
public sealed class RequirementAggregate : AggregateRoot
{
    private readonly List<AcceptanceCriterion> _acceptanceCriteria = [];

    private RequirementAggregate(
        Guid id,
        Guid tenantId,
        Guid projectId,
        string title,
        string statement,
        RequirementStatus status,
        bool parsed,
        DocumentMetadata? documentMetadata,
        DateTimeOffset createdAt,
        IEnumerable<AcceptanceCriterion>? acceptanceCriteria = null)
    {
        Id = id;
        TenantId = tenantId;
        ProjectId = projectId;
        Title = title;
        Statement = statement;
        Status = status;
        Parsed = parsed;
        DocumentMetadata = documentMetadata;
        CreatedAt = createdAt;
        if (acceptanceCriteria is not null)
        {
            _acceptanceCriteria.AddRange(acceptanceCriteria);
        }
    }

    /// <summary>Requirement identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Owning project identifier (BC-01 AGG-004).</summary>
    public Guid ProjectId { get; }

    /// <summary>Short requirement title.</summary>
    public string Title { get; }

    /// <summary>Requirement statement / body.</summary>
    public string Statement { get; }

    /// <summary>Canonical business lifecycle (CON-013). Independent of <see cref="Parsed"/>.</summary>
    public RequirementStatus Status { get; }

    /// <summary>
    /// Technical processing flag (CAP-006). Not a lifecycle state; must not be coupled to UnderReview.
    /// </summary>
    public bool Parsed { get; }

    /// <summary>Optional source document metadata (no binary storage).</summary>
    public DocumentMetadata? DocumentMetadata { get; }

    /// <summary>Creation time UTC.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Owned acceptance criteria (CON-014).</summary>
    public IReadOnlyList<AcceptanceCriterion> AcceptanceCriteria => _acceptanceCriteria.AsReadOnly();

    /// <summary>
    /// CMD-030 IntakeRequirement.
    /// Postcondition: status <see cref="RequirementStatus.Draft"/>; Parsed may be true when title+statement provided.
    /// Does not transition to UnderReview.
    /// </summary>
    public static RequirementAggregate Intake(
        Guid tenantId,
        Guid projectId,
        string title,
        string statement,
        IEnumerable<string>? acceptanceCriterionStatements = null,
        DocumentMetadata? documentMetadata = null)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ValidationError("TenantId is required.");
        }

        if (projectId == Guid.Empty)
        {
            throw new ValidationError("ProjectId is required.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationError("Requirement title is required.");
        }

        if (string.IsNullOrWhiteSpace(statement))
        {
            throw new ValidationError("Requirement statement is required.");
        }

        var trimmedTitle = title.Trim();
        var trimmedStatement = statement.Trim();
        if (trimmedTitle.Length > 200)
        {
            throw new ValidationError("Requirement title must be at most 200 characters.");
        }

        if (trimmedStatement.Length > 8000)
        {
            throw new ValidationError("Requirement statement must be at most 8000 characters.");
        }

        // Parsed is independent of status: text intake with title+statement is already human-structured.
        var parsed = true;

        var aggregate = new RequirementAggregate(
            Guid.NewGuid(),
            tenantId,
            projectId,
            trimmedTitle,
            trimmedStatement,
            RequirementStatus.Draft,
            parsed,
            documentMetadata,
            DateTimeOffset.UtcNow);

        if (acceptanceCriterionStatements is not null)
        {
            var sortOrder = 0;
            foreach (var criterionStatement in acceptanceCriterionStatements)
            {
                if (string.IsNullOrWhiteSpace(criterionStatement))
                {
                    continue;
                }

                aggregate._acceptanceCriteria.Add(
                    AcceptanceCriterion.Create(aggregate.Id, tenantId, criterionStatement, sortOrder++));
            }
        }

        aggregate.Raise(new RequirementIntakenDomainEvent(
            aggregate.Id,
            aggregate.TenantId,
            aggregate.ProjectId,
            aggregate.Title,
            aggregate.Status.ToString(),
            aggregate.Parsed));

        return aggregate;
    }

    /// <summary>
    /// Approves the requirement.
    /// </summary>
    public RequirementAggregate Approve()
    {
        if (!Parsed)
        {
            throw new ValidationError("Requirement must be parsed before approval.");
        }

        if (Status == RequirementStatus.Approved)
        {
            throw new ValidationError("Requirement is already approved.");
        }

        if (Status == RequirementStatus.Superseded)
        {
            throw new ValidationError("Cannot approve a superseded requirement.");
        }

        if (Status == RequirementStatus.Retired)
        {
            throw new ValidationError("Cannot approve a retired requirement.");
        }

        if (Status != RequirementStatus.Draft && Status != RequirementStatus.UnderReview)
        {
            throw new ValidationError($"Requirement cannot be approved from status '{Status}'.");
        }

        var approved = new RequirementAggregate(
            Id,
            TenantId,
            ProjectId,
            Title,
            Statement,
            RequirementStatus.Approved,
            Parsed,
            DocumentMetadata,
            CreatedAt,
            _acceptanceCriteria);

        approved.Raise(new RequirementApprovedDomainEvent(
            approved.Id,
            approved.TenantId,
            approved.ProjectId,
            approved.Title,
            approved.Status.ToString(),
            approved.Parsed));

        return approved;
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static RequirementAggregate Rehydrate(
        Guid id,
        Guid tenantId,
        Guid projectId,
        string title,
        string statement,
        RequirementStatus status,
        bool parsed,
        DocumentMetadata? documentMetadata,
        DateTimeOffset createdAt,
        IEnumerable<AcceptanceCriterion>? acceptanceCriteria = null)
        => new(
            id,
            tenantId,
            projectId,
            title,
            statement,
            status,
            parsed,
            documentMetadata,
            createdAt,
            acceptanceCriteria);
}

/// <summary>
/// Requirement business lifecycle (CON-013).
/// Parsed is a separate technical flag and must not be treated as a status.
/// Trace: FR-011, CAP-006, CON-013.
/// </summary>
public enum RequirementStatus
{
    /// <summary>Initial status after CMD-030 intake.</summary>
    Draft = 0,

    /// <summary>Submitted for review (not set by Slice 1 intake).</summary>
    UnderReview = 1,

    /// <summary>Approved for downstream consumption.</summary>
    Approved = 2,

    /// <summary>Replaced by a newer requirement.</summary>
    Superseded = 3,

    /// <summary>Terminal retired.</summary>
    Retired = 4
}

/// <summary>
/// EVT-030 RequirementIntaken domain event.
/// Catalog gap: Commands-Events-Catalog event table; Aggregate Catalog authorizes EVT-030.
/// Trace: CMD-030, FR-010, FR-011, AGG-005, ADR-SAD-004.
/// </summary>
public sealed record RequirementIntakenDomainEvent(
    Guid RequirementId,
    Guid TenantId,
    Guid ProjectId,
    string Title,
    string Status,
    bool Parsed) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// EVT-031 RequirementApproved domain event.
/// Trace: CMD-031, AGG-005, CAP-006, CON-013, ADR-SAD-004.
/// </summary>
public sealed record RequirementApprovedDomainEvent(
    Guid RequirementId,
    Guid TenantId,
    Guid ProjectId,
    string Title,
    string Status,
    bool Parsed) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
