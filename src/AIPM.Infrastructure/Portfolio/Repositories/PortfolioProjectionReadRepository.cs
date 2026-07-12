using AIPM.Application.Portfolio;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Portfolio.Repositories;

/// <summary>
/// Live SQL rollup repository for FR-122 portfolio aggregates (interim Option A).
/// Trace: FR-122, ADR-SAD-008, ADR-005.
/// </summary>
public sealed class PortfolioProjectionReadRepository : IPortfolioProjectionReadRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public PortfolioProjectionReadRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<PortfolioSummaryDto?> GetPortfolioSummaryAsync(
        Guid tenantId,
        Guid portfolioId,
        CancellationToken cancellationToken = default)
    {
        var portfolio = await _dbContext.Portfolios.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == portfolioId, cancellationToken);
        if (portfolio is null)
        {
            return null;
        }

        var programCount = await _dbContext.Programs.AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.PortfolioId == portfolioId, cancellationToken);

        var statusCounts = await (
            from project in _dbContext.Projects.AsNoTracking()
            join program in _dbContext.Programs.AsNoTracking() on project.ProgramId equals program.Id
            where project.TenantId == tenantId
                  && program.TenantId == tenantId
                  && program.PortfolioId == portfolioId
            group project by project.Status into g
            select new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = ToStatusBucket(statusCounts.Select(x => (x.Status, x.Count)));
        return new PortfolioSummaryDto(
            portfolio.Id,
            portfolio.Name,
            programCount,
            counts.Total,
            counts.Draft,
            counts.Active,
            counts.OnHold,
            counts.Completed,
            counts.Archived);
    }

    /// <inheritdoc />
    public async Task<ProgramSummaryDto?> GetProgramSummaryAsync(
        Guid tenantId,
        Guid programId,
        CancellationToken cancellationToken = default)
    {
        var program = await _dbContext.Programs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == programId, cancellationToken);
        if (program is null)
        {
            return null;
        }

        var statusCounts = await _dbContext.Projects.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.ProgramId == programId)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = ToStatusBucket(statusCounts.Select(x => (x.Status, x.Count)));
        return new ProgramSummaryDto(
            program.Id,
            program.Name,
            program.PortfolioId,
            counts.Total,
            counts.Draft,
            counts.Active,
            counts.OnHold,
            counts.Completed,
            counts.Archived);
    }

    /// <inheritdoc />
    public async Task<ProjectSummaryDto?> GetProjectSummaryAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var row = await (
            from project in _dbContext.Projects.AsNoTracking()
            join program in _dbContext.Programs.AsNoTracking() on project.ProgramId equals program.Id
            where project.TenantId == tenantId
                  && program.TenantId == tenantId
                  && project.Id == projectId
            select new
            {
                project.Id,
                project.Name,
                project.ProgramId,
                program.PortfolioId,
                project.OwnerUserId,
                project.WorkspaceId,
                project.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return null;
        }

        var scopeChangeCount = await _dbContext.ScopeChanges.AsNoTracking()
            .CountAsync(x => x.TenantId == tenantId && x.ProjectId == projectId, cancellationToken);

        return new ProjectSummaryDto(
            row.Id,
            row.Name,
            row.ProgramId,
            row.PortfolioId,
            row.OwnerUserId,
            row.WorkspaceId,
            row.Status,
            scopeChangeCount);
    }

    private static StatusBucket ToStatusBucket(IEnumerable<(string Status, int Count)> rows)
    {
        var draft = 0;
        var active = 0;
        var onHold = 0;
        var completed = 0;
        var archived = 0;

        foreach (var row in rows)
        {
            if (string.Equals(row.Status, nameof(ProjectStatus.Draft), StringComparison.Ordinal))
            {
                draft += row.Count;
            }
            else if (string.Equals(row.Status, nameof(ProjectStatus.Active), StringComparison.Ordinal))
            {
                active += row.Count;
            }
            else if (string.Equals(row.Status, nameof(ProjectStatus.OnHold), StringComparison.Ordinal))
            {
                onHold += row.Count;
            }
            else if (string.Equals(row.Status, nameof(ProjectStatus.Completed), StringComparison.Ordinal))
            {
                completed += row.Count;
            }
            else if (string.Equals(row.Status, nameof(ProjectStatus.Archived), StringComparison.Ordinal))
            {
                archived += row.Count;
            }
        }

        return new StatusBucket(draft, active, onHold, completed, archived);
    }

    private sealed record StatusBucket(int Draft, int Active, int OnHold, int Completed, int Archived)
    {
        public int Total => Draft + Active + OnHold + Completed + Archived;
    }
}
