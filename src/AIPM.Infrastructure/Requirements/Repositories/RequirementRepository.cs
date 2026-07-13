using AIPM.Application.Requirements;
using AIPM.Domain.Requirements;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Requirements.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Requirements.Repositories;

/// <summary>
/// EF repository for BC-02 requirement aggregate.
/// Trace: AGG-005, FR-010, FR-011, ADR-SAD-008, ADR-005.
/// </summary>
public sealed class RequirementRepository : IRequirementRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public RequirementRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default)
    {
        await _dbContext.Requirements.AddAsync(ToRecord(requirement), cancellationToken);
        foreach (var criterion in requirement.AcceptanceCriteria)
        {
            await _dbContext.AcceptanceCriteria.AddAsync(ToAcceptanceCriterionRecord(criterion), cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Requirements
            .FirstOrDefaultAsync(x => x.TenantId == requirement.TenantId && x.Id == requirement.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Requirement '{requirement.Id}' was not found for update.");

        record.Title = requirement.Title;
        record.Statement = requirement.Statement;
        record.Status = requirement.Status.ToString();
        record.Parsed = requirement.Parsed;
        record.DocumentTitle = requirement.DocumentMetadata?.Title;
        record.DocumentContentType = requirement.DocumentMetadata?.ContentType;
        record.DocumentUriOrName = requirement.DocumentMetadata?.UriOrName;

        await SyncAcceptanceCriteriaAsync(requirement, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RequirementAggregate?> FindAsync(
        Guid tenantId,
        Guid requirementId,
        CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Requirements.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == requirementId, cancellationToken);

        if (record is null)
        {
            return null;
        }

        var criteria = await _dbContext.AcceptanceCriteria.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.RequirementId == requirementId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        return FromRecord(record, criteria);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RequirementAggregate>> ListByProjectAsync(
        Guid tenantId,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.Requirements.AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        if (records.Count == 0)
        {
            return [];
        }

        // Order in-memory: SQLite (tests) cannot ORDER BY DateTimeOffset in SQL.
        records = records.OrderBy(x => x.CreatedAt).ThenBy(x => x.Title).ToList();

        var requirementIds = records.Select(x => x.Id).ToList();
        var criteria = await _dbContext.AcceptanceCriteria.AsNoTracking()
            .Where(x => x.TenantId == tenantId && requirementIds.Contains(x.RequirementId))
            .ToListAsync(cancellationToken);

        var byRequirement = criteria.GroupBy(x => x.RequirementId).ToDictionary(g => g.Key, g => g.ToList());
        return records
            .Select(r => FromRecord(r, byRequirement.GetValueOrDefault(r.Id) ?? []))
            .ToList();
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);

    private async Task SyncAcceptanceCriteriaAsync(RequirementAggregate requirement, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.AcceptanceCriteria
            .Where(x => x.TenantId == requirement.TenantId && x.RequirementId == requirement.Id)
            .ToListAsync(cancellationToken);
        var existingById = existing.ToDictionary(x => x.Id);
        var currentIds = requirement.AcceptanceCriteria.Select(x => x.Id).ToHashSet();

        foreach (var criterion in requirement.AcceptanceCriteria)
        {
            if (existingById.TryGetValue(criterion.Id, out var existingRow))
            {
                existingRow.Statement = criterion.Statement;
                existingRow.SortOrder = criterion.SortOrder;
            }
            else
            {
                await _dbContext.AcceptanceCriteria.AddAsync(ToAcceptanceCriterionRecord(criterion), cancellationToken);
            }
        }

        foreach (var orphan in existing.Where(x => !currentIds.Contains(x.Id)))
        {
            _dbContext.AcceptanceCriteria.Remove(orphan);
        }
    }

    private static RequirementRecord ToRecord(RequirementAggregate requirement)
        => new()
        {
            Id = requirement.Id,
            TenantId = requirement.TenantId,
            ProjectId = requirement.ProjectId,
            Title = requirement.Title,
            Statement = requirement.Statement,
            Status = requirement.Status.ToString(),
            Parsed = requirement.Parsed,
            DocumentTitle = requirement.DocumentMetadata?.Title,
            DocumentContentType = requirement.DocumentMetadata?.ContentType,
            DocumentUriOrName = requirement.DocumentMetadata?.UriOrName,
            CreatedAt = requirement.CreatedAt
        };

    private static AcceptanceCriterionRecord ToAcceptanceCriterionRecord(AcceptanceCriterion criterion)
        => new()
        {
            Id = criterion.Id,
            RequirementId = criterion.RequirementId,
            TenantId = criterion.TenantId,
            Statement = criterion.Statement,
            SortOrder = criterion.SortOrder
        };

    private static RequirementAggregate FromRecord(
        RequirementRecord record,
        IReadOnlyList<AcceptanceCriterionRecord> criteria)
        => RequirementAggregate.Rehydrate(
            record.Id,
            record.TenantId,
            record.ProjectId,
            record.Title,
            record.Statement,
            Enum.Parse<RequirementStatus>(record.Status),
            record.Parsed,
            DocumentMetadata.Rehydrate(record.DocumentTitle, record.DocumentContentType, record.DocumentUriOrName),
            record.CreatedAt,
            criteria.Select(FromAcceptanceCriterionRecord));

    private static AcceptanceCriterion FromAcceptanceCriterionRecord(AcceptanceCriterionRecord record)
        => AcceptanceCriterion.Rehydrate(
            record.Id,
            record.RequirementId,
            record.TenantId,
            record.Statement,
            record.SortOrder);
}
