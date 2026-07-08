using AIPM.Application.Portfolio;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Portfolio.Repositories;

/// <summary>
/// EF repository for BC-01 project aggregate.
/// Trace: AGG-004, FR-001, ADR-SAD-008.
/// </summary>
public sealed class ProjectRepository : IProjectRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public ProjectRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(ProjectAggregate project, CancellationToken cancellationToken = default)
    {
        await _dbContext.Projects.AddAsync(ToRecord(project), cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(ProjectAggregate project, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Projects
            .FirstOrDefaultAsync(x => x.TenantId == project.TenantId && x.Id == project.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Project '{project.Id}' was not found for update.");

        record.WorkspaceId = project.WorkspaceId;
        record.OwnerUserId = project.OwnerUserId;
        record.Name = project.Name;
        record.Status = project.Status.ToString();
        record.ArchivedAt = project.ArchivedAt;
    }

    /// <inheritdoc />
    public async Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Projects.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == projectId, cancellationToken);

        return record is null ? null : FromRecord(record);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => (await _dbContext.Projects.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken))
            .Select(FromRecord)
            .ToList();

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private static ProjectRecord ToRecord(ProjectAggregate project)
        => new()
        {
            Id = project.Id,
            TenantId = project.TenantId,
            ProgramId = project.ProgramId,
            WorkspaceId = project.WorkspaceId,
            OwnerUserId = project.OwnerUserId,
            Name = project.Name,
            Status = project.Status.ToString(),
            CreatedAt = project.CreatedAt,
            ArchivedAt = project.ArchivedAt
        };

    private static ProjectAggregate FromRecord(ProjectRecord record)
        => ProjectAggregate.Rehydrate(
            record.Id,
            record.TenantId,
            record.ProgramId,
            record.WorkspaceId,
            record.OwnerUserId,
            record.Name,
            Enum.Parse<ProjectStatus>(record.Status),
            record.CreatedAt,
            record.ArchivedAt);
}
