namespace AIPM.Application.Portfolio;

/// <summary>
/// Read-side projection repository for FR-122 portfolio aggregates.
/// Interim: live SQL rollups over BC-01 tables (not DU-5 materialized projections).
/// Trace: FR-122, ADR-SAD-008, ADR-005.
/// </summary>
public interface IPortfolioProjectionReadRepository
{
    /// <summary>Builds a tenant-scoped portfolio summary, or null when missing.</summary>
    Task<PortfolioSummaryDto?> GetPortfolioSummaryAsync(
        Guid tenantId,
        Guid portfolioId,
        CancellationToken cancellationToken = default);

    /// <summary>Builds a tenant-scoped program summary, or null when missing.</summary>
    Task<ProgramSummaryDto?> GetProgramSummaryAsync(
        Guid tenantId,
        Guid programId,
        CancellationToken cancellationToken = default);

    /// <summary>Builds a tenant-scoped project summary, or null when missing.</summary>
    Task<ProjectSummaryDto?> GetProjectSummaryAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default);
}
