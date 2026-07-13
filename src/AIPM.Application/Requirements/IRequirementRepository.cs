using AIPM.Domain.Requirements;

namespace AIPM.Application.Requirements;

/// <summary>
/// BC-02 requirement repository contract.
/// Trace: AGG-005, FR-010, FR-011, ADR-SAD-008, ADR-005.
/// </summary>
public interface IRequirementRepository
{
    /// <summary>Adds a new requirement aggregate.</summary>
    Task AddAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing requirement aggregate.</summary>
    Task UpdateAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default);

    /// <summary>Finds a tenant-scoped requirement by identifier.</summary>
    Task<RequirementAggregate?> FindAsync(Guid tenantId, Guid requirementId, CancellationToken cancellationToken = default);

    /// <summary>Lists requirements under a tenant project.</summary>
    Task<IReadOnlyList<RequirementAggregate>> ListByProjectAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default);

    /// <summary>Saves pending repository changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
