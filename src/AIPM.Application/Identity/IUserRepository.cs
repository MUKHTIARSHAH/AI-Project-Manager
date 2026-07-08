using AIPM.Domain.Identity;

namespace AIPM.Application.Identity;

/// <summary>User repository abstraction.</summary>
public interface IUserRepository
{
    /// <summary>Adds user.</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Finds user by id.</summary>
    Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Updates user aggregate state.</summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Lists users.</summary>
    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves pending changes.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
