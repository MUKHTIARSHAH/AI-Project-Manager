using AIPM.Application.Portfolio;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Portfolio.Repositories;

/// <summary>
/// EF repository for BC-01 portfolio aggregate.
/// Trace: AGG-002, FR-003, ADR-SAD-008.
/// </summary>
public sealed class PortfolioRepository : IPortfolioRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public PortfolioRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(PortfolioAggregate portfolio, CancellationToken cancellationToken = default)
    {
        await _dbContext.Portfolios.AddAsync(new PortfolioRecord
        {
            Id = portfolio.Id,
            TenantId = portfolio.TenantId,
            Name = portfolio.Name,
            CreatedAt = portfolio.CreatedAt
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PortfolioAggregate?> FindAsync(Guid tenantId, Guid portfolioId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Portfolios.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Id == portfolioId, cancellationToken);

        return record is null
            ? null
            : PortfolioAggregate.Rehydrate(record.Id, record.TenantId, record.Name, record.CreatedAt);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PortfolioAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        => (await _dbContext.Portfolios.AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken))
            .Select(x => PortfolioAggregate.Rehydrate(x.Id, x.TenantId, x.Name, x.CreatedAt))
            .ToList();

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
