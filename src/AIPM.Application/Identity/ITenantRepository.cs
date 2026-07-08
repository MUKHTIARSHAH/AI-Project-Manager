using AIPM.Domain.Identity;

namespace AIPM.Application.Identity;

/// <summary>
/// Tenant repository abstraction.
/// </summary>
public interface ITenantRepository
{
    /// <summary>Adds tenant.</summary>
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>Finds tenant by id.</summary>
    Task<Tenant?> FindAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Updates tenant aggregate state.</summary>
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>Lists tenants.</summary>
    Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves pending changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
