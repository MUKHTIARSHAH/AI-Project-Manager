using AIPM.Domain.Primitives;
using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Portfolio;

/// <summary>
/// AGG-004 Project aggregate.
/// Trace: FR-001, CAP-001, CON-008, CMD-020, CMD-021, EVT-020, EVT-021, ADR-005, IDL-001.
/// Workspace is referenced only (CON-003 / ENT-002); ownership remains outside BC-01 create path.
/// </summary>
public sealed class ProjectAggregate : AggregateRoot
{
    private ProjectAggregate(
        Guid id,
        Guid tenantId,
        Guid programId,
        Guid workspaceId,
        Guid ownerUserId,
        string name,
        ProjectStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset? archivedAt)
    {
        Id = id;
        TenantId = tenantId;
        ProgramId = programId;
        WorkspaceId = workspaceId;
        OwnerUserId = ownerUserId;
        Name = name;
        Status = status;
        CreatedAt = createdAt;
        ArchivedAt = archivedAt;
    }

    /// <summary>Project identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Owning program identifier (AGG-003).</summary>
    public Guid ProgramId { get; }

    /// <summary>Workspace reference (ENT-002 / CON-003) — not managed by BC-01.</summary>
    public Guid WorkspaceId { get; private set; }

    /// <summary>Required project owner user identifier.</summary>
    public Guid OwnerUserId { get; private set; }

    /// <summary>Display name unique per tenant.</summary>
    public string Name { get; private set; }

    /// <summary>Lifecycle status (CON-008).</summary>
    public ProjectStatus Status { get; private set; }

    /// <summary>Creation time UTC.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Archive time UTC when archived.</summary>
    public DateTimeOffset? ArchivedAt { get; private set; }

    /// <summary>Whether the project is read-only.</summary>
    public bool IsArchived => Status == ProjectStatus.Archived;

    /// <summary>
    /// CMD-020 CreateProject. Preconditions: Workspace exists (caller-enforced by reference id).
    /// Postcondition: Project Draft.
    /// </summary>
    public static ProjectAggregate Create(
        Guid tenantId,
        Guid programId,
        Guid workspaceId,
        Guid ownerUserId,
        string name)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ValidationError("TenantId is required.");
        }

        if (programId == Guid.Empty)
        {
            throw new ValidationError("ProgramId is required.");
        }

        if (workspaceId == Guid.Empty)
        {
            throw new ValidationError("WorkspaceId is required.");
        }

        if (ownerUserId == Guid.Empty)
        {
            throw new ValidationError("OwnerUserId is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationError("Project name is required.");
        }

        var aggregate = new ProjectAggregate(
            Guid.NewGuid(),
            tenantId,
            programId,
            workspaceId,
            ownerUserId,
            name.Trim(),
            ProjectStatus.Draft,
            DateTimeOffset.UtcNow,
            null);

        aggregate.Raise(new ProjectCreatedDomainEvent(
            aggregate.Id,
            aggregate.TenantId,
            aggregate.ProgramId,
            aggregate.WorkspaceId,
            aggregate.OwnerUserId,
            aggregate.Name));

        return aggregate;
    }

    /// <summary>
    /// FR-001 update path. Archived projects are read-only (CAP-001 / CON-008).
    /// </summary>
    public void Update(string name, Guid ownerUserId, Guid workspaceId)
    {
        EnsureMutable();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationError("Project name is required.");
        }

        if (ownerUserId == Guid.Empty)
        {
            throw new ValidationError("OwnerUserId is required.");
        }

        if (workspaceId == Guid.Empty)
        {
            throw new ValidationError("WorkspaceId is required.");
        }

        Name = name.Trim();
        OwnerUserId = ownerUserId;
        WorkspaceId = workspaceId;
    }

    /// <summary>
    /// CMD-021 ArchiveProject. Postcondition: Project Archived (read-only).
    /// </summary>
    public void Archive()
    {
        if (IsArchived)
        {
            throw new ValidationError("Project is already archived.");
        }

        Status = ProjectStatus.Archived;
        ArchivedAt = DateTimeOffset.UtcNow;
        Raise(new ProjectArchivedDomainEvent(Id, TenantId, Name));
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static ProjectAggregate Rehydrate(
        Guid id,
        Guid tenantId,
        Guid programId,
        Guid workspaceId,
        Guid ownerUserId,
        string name,
        ProjectStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset? archivedAt)
        => new(id, tenantId, programId, workspaceId, ownerUserId, name, status, createdAt, archivedAt);

    private void EnsureMutable()
    {
        if (IsArchived)
        {
            throw new ValidationError("Archived projects are read-only.");
        }
    }
}

/// <summary>
/// Project lifecycle status (CON-008).
/// Trace: FR-001, CAP-001.
/// </summary>
public enum ProjectStatus
{
    /// <summary>Initial status after CMD-020.</summary>
    Draft = 0,

    /// <summary>Active delivery.</summary>
    Active = 1,

    /// <summary>Temporarily paused.</summary>
    OnHold = 2,

    /// <summary>Delivery completed.</summary>
    Completed = 3,

    /// <summary>Terminal archived (read-only).</summary>
    Archived = 4
}

/// <summary>
/// EVT-020 ProjectCreated domain event.
/// Trace: CMD-020, FR-001, AGG-004.
/// </summary>
public sealed record ProjectCreatedDomainEvent(
    Guid ProjectId,
    Guid TenantId,
    Guid ProgramId,
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// EVT-021 ProjectArchived domain event.
/// Trace: CMD-021, FR-001, AGG-004.
/// </summary>
public sealed record ProjectArchivedDomainEvent(Guid ProjectId, Guid TenantId, string Name) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
