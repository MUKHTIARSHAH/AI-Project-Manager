using AIPM.Application.Portfolio;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Portfolio.Repositories;

/// <summary>
/// EF repository for BC-01 program aggregate.
/// Trace: AGG-003, FR-003, ADR-SAD-008.
/// </summary>
public sealed class ProgramRepository : IProgramRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public ProgramRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(ProgramAggregate program, CancellationToken cancellationToken = default)
    {
        await _dbContext.Programs.AddAsync(new ProgramRecord
        {
            Id = program.Id,
            TenantId = program.TenantId,
            PortfolioId = program.PortfolioId,
            Name = program.Name,
            CreatedAt = program.CreatedAt
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ProgramAggregate?> FindAsync(Guid tenantId, Guid programId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Programs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == programId, cancellationToken);

        return record is null
            ? null
            : ProgramAggregate.Rehydrate(record.Id, record.TenantId, record.PortfolioId, record.Name, record.CreatedAt);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProgramAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => (await _dbContext.Programs.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken))
            .Select(x => ProgramAggregate.Rehydrate(x.Id, x.TenantId, x.PortfolioId, x.Name, x.CreatedAt))
            .ToList();

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
