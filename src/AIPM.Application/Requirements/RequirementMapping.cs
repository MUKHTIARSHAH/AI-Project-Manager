using AIPM.Domain.Requirements;

namespace AIPM.Application.Requirements;

/// <summary>
/// Maps AGG-005 to application DTOs.
/// Trace: FR-011, AGG-005, CON-013, CON-014.
/// </summary>
internal static class RequirementMapping
{
    /// <summary>Maps a full requirement DTO.</summary>
    public static RequirementDto ToDto(RequirementAggregate requirement)
        => new(
            requirement.Id,
            requirement.TenantId,
            requirement.ProjectId,
            requirement.Title,
            requirement.Statement,
            requirement.Status.ToString(),
            requirement.Parsed,
            requirement.DocumentMetadata?.Title,
            requirement.DocumentMetadata?.ContentType,
            requirement.DocumentMetadata?.UriOrName,
            requirement.CreatedAt,
            requirement.AcceptanceCriteria
                .OrderBy(x => x.SortOrder)
                .Select(x => new AcceptanceCriterionDto(x.Id, x.Statement, x.SortOrder))
                .ToList());

    /// <summary>Maps a list item DTO.</summary>
    public static RequirementListItemDto ToListItemDto(RequirementAggregate requirement)
        => new(
            requirement.Id,
            requirement.ProjectId,
            requirement.Title,
            requirement.Status.ToString(),
            requirement.Parsed,
            requirement.CreatedAt);
}
