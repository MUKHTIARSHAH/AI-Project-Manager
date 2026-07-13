namespace AIPM.Application.Requirements;

/// <summary>
/// BC-02 requirement DTO.
/// Trace: FR-010, FR-011, AGG-005, CON-013, CON-014.
/// </summary>
public sealed record RequirementDto(
    Guid Id,
    Guid TenantId,
    Guid ProjectId,
    string Title,
    string Statement,
    string Status,
    bool Parsed,
    string? DocumentTitle,
    string? DocumentContentType,
    string? DocumentUriOrName,
    DateTimeOffset CreatedAt,
    IReadOnlyList<AcceptanceCriterionDto> AcceptanceCriteria);

/// <summary>
/// Acceptance criterion DTO (CON-014).
/// Trace: FR-011, CON-014, AGG-005.
/// </summary>
public sealed record AcceptanceCriterionDto(
    Guid Id,
    string Statement,
    int SortOrder);

/// <summary>
/// List item DTO for project-scoped requirement listings.
/// Trace: FR-011, AGG-005, ADR-SAD-008.
/// </summary>
public sealed record RequirementListItemDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Status,
    bool Parsed,
    DateTimeOffset CreatedAt);
