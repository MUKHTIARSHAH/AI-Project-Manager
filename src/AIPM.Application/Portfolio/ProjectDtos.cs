namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 project DTO.
/// Trace: FR-001, AGG-004, CON-008.
/// </summary>
public sealed record ProjectDto(
    Guid Id,
    Guid TenantId,
    Guid ProgramId,
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ArchivedAt);
