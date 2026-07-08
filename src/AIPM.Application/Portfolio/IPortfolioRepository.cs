using AIPM.Domain.Portfolio;

namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 portfolio repository contract.
/// Trace: AGG-002, FR-003, ADR-SAD-008.
/// </summary>
public interface IPortfolioRepository
{
    /// <summary>Adds a new portfolio aggregate.</summary>
    Task AddAsync(PortfolioAggregate portfolio, CancellationToken cancellationToken = default);

    /// <summary>Finds a tenant-scoped portfolio by identifier.</summary>
    Task<PortfolioAggregate?> FindAsync(Guid tenantId, Guid portfolioId, CancellationToken cancellationToken = default);

    /// <summary>Lists all portfolios owned by tenant.</summary>
    Task<IReadOnlyList<PortfolioAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Saves pending repository changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
