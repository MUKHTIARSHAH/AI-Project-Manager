namespace AIPM.Application.Portfolio;

/// <summary>
/// Response for FR-005 project cloning.
/// Trace: FR-005, CAP-001, CON-008, AGG-004.
/// </summary>
public sealed record CloneProjectResponse(
    Guid SourceProjectId,
    Guid Id,
    Guid TenantId,
    Guid ProgramId,
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ArchivedAt);
