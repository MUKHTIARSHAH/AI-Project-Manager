using AIPM.Domain.Identity;

namespace AIPM.Application.Identity;

/// <summary>Role repository abstraction.</summary>
public interface IRoleRepository
{
    /// <summary>Adds role.</summary>
    Task AddAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>Finds role by id.</summary>
    Task<Role?> FindAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>Updates role aggregate state.</summary>
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>Lists roles.</summary>
    Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Lists roles for a tenant.</summary>
    Task<IReadOnlyList<Role>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>Saves pending changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
