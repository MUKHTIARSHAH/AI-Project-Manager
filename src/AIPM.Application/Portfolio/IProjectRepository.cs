using AIPM.Domain.Portfolio;

namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 project repository contract.
/// Trace: AGG-004, FR-001, ADR-SAD-008.
/// </summary>
public interface IProjectRepository
{
    /// <summary>Adds a new project aggregate.</summary>
    Task AddAsync(ProjectAggregate project, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing project aggregate.</summary>
    Task UpdateAsync(ProjectAggregate project, CancellationToken cancellationToken = default);

    /// <summary>Finds a tenant-scoped project by identifier.</summary>
    Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>Lists all projects under a tenant.</summary>
    Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Saves pending repository changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
