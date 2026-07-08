using AIPM.Domain.Portfolio;

namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 program repository contract.
/// Trace: AGG-003, FR-003, ADR-SAD-008.
/// </summary>
public interface IProgramRepository
{
    /// <summary>Adds a new program aggregate.</summary>
    Task AddAsync(ProgramAggregate program, CancellationToken cancellationToken = default);

    /// <summary>Finds a tenant-scoped program by identifier.</summary>
    Task<ProgramAggregate?> FindAsync(Guid tenantId, Guid programId, CancellationToken cancellationToken = default);

    /// <summary>Lists all programs under a tenant.</summary>
    Task<IReadOnlyList<ProgramAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Saves pending repository changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
