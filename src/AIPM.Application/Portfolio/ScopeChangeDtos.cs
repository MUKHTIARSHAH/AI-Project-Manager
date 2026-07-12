namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 scope change DTO (CON-011).
/// Trace: FR-004, CAP-004, AGG-004, CMD-022.
/// </summary>
public sealed record ScopeChangeDto(
    Guid Id,
    Guid ProjectId,
    Guid TenantId,
    string Title,
    string Description,
    string? AffectedRequirementCitation,
    string Status,
    DateTimeOffset RecordedAt,
    DateTimeOffset? DecidedAt);
