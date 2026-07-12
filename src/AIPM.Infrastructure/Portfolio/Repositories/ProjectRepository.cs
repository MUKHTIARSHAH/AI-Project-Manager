using AIPM.Application.Portfolio;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Portfolio.Repositories;

/// <summary>
/// EF repository for BC-01 project aggregate.
/// Trace: AGG-004, FR-001, FR-004, ADR-SAD-008.
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

        await SyncScopeChangesAsync(project, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Projects.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == projectId, cancellationToken);

        if (record is null)
        {
            return null;
        }

        var scopeChanges = await _dbContext.ScopeChanges.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        return FromRecord(record, scopeChanges);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.Projects.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        if (records.Count == 0)
        {
            return [];
        }

        var projectIds = records.Select(x => x.Id).ToList();
        var scopeChanges = await _dbContext.ScopeChanges.AsNoTracking()
            .Where(x => x.TenantId == tenantId && projectIds.Contains(x.ProjectId))
            .ToListAsync(cancellationToken);

        var byProject = scopeChanges.GroupBy(x => x.ProjectId).ToDictionary(g => g.Key, g => g.ToList());
        return records
            .Select(r => FromRecord(r, byProject.GetValueOrDefault(r.Id) ?? []))
            .ToList();
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private async Task SyncScopeChangesAsync(ProjectAggregate project, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.ScopeChanges
            .Where(x => x.TenantId == project.TenantId && x.ProjectId == project.Id)
            .ToListAsync(cancellationToken);
        var existingById = existing.ToDictionary(x => x.Id);

        foreach (var scopeChange in project.ScopeChanges)
        {
            if (existingById.TryGetValue(scopeChange.Id, out var existingRow))
            {
                existingRow.Title = scopeChange.Title;
                existingRow.Description = scopeChange.Description;
                existingRow.AffectedRequirementCitation = scopeChange.AffectedRequirementCitation;
                existingRow.Status = scopeChange.Status.ToString();
                existingRow.DecidedAt = scopeChange.DecidedAt;
            }
            else
            {
                await _dbContext.ScopeChanges.AddAsync(ToScopeChangeRecord(scopeChange), cancellationToken);
            }
        }
    }

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

    private static ScopeChangeRecord ToScopeChangeRecord(ScopeChange scopeChange)
        => new()
        {
            Id = scopeChange.Id,
            ProjectId = scopeChange.ProjectId,
            TenantId = scopeChange.TenantId,
            Title = scopeChange.Title,
            Description = scopeChange.Description,
            AffectedRequirementCitation = scopeChange.AffectedRequirementCitation,
            Status = scopeChange.Status.ToString(),
            RecordedAt = scopeChange.RecordedAt,
            DecidedAt = scopeChange.DecidedAt
        };

    private static ProjectAggregate FromRecord(ProjectRecord record, IReadOnlyList<ScopeChangeRecord> scopeChanges)
        => ProjectAggregate.Rehydrate(
            record.Id,
            record.TenantId,
            record.ProgramId,
            record.WorkspaceId,
            record.OwnerUserId,
            record.Name,
            Enum.Parse<ProjectStatus>(record.Status),
            record.CreatedAt,
            record.ArchivedAt,
            scopeChanges.Select(FromScopeChangeRecord));

    private static ScopeChange FromScopeChangeRecord(ScopeChangeRecord record)
        => ScopeChange.Rehydrate(
            record.Id,
            record.ProjectId,
            record.TenantId,
            record.Title,
            record.Description,
            record.AffectedRequirementCitation,
            Enum.Parse<ScopeChangeStatus>(record.Status),
            record.RecordedAt,
            record.DecidedAt);
}
