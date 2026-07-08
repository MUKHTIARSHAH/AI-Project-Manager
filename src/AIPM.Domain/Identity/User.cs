using AIPM.Domain.Primitives;

namespace AIPM.Domain.Identity;

/// <summary>
/// User aggregate for BC-10 identity responsibility.
/// </summary>
public sealed class User : AggregateRoot
{
    private readonly List<RoleAssignment> _roleAssignments = [];

    private User()
    {
    }

    /// <summary>User identifier.</summary>
    public Guid Id { get; private set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Email address.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Assigned role references.</summary>
    public IReadOnlyCollection<RoleAssignment> RoleAssignments => _roleAssignments.AsReadOnly();

    /// <summary>Creates a new user in a tenant.</summary>
    public static User Create(Guid tenantId, string email)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId must be set.", nameof(tenantId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant()
        };
    }

    /// <summary>Rehydrates user with pre-existing assignments.</summary>
    public static User Rehydrate(Guid id, Guid tenantId, string email, IEnumerable<RoleAssignment> assignments)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("UserId must be set.", nameof(id));
        }

        var user = Create(tenantId, email);
        user.Id = id;
        user._roleAssignments.Clear();
        user._roleAssignments.AddRange(assignments);
        return user;
    }

    /// <summary>Assigns a role to this user.</summary>
    public void AssignRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException("RoleId must be set.", nameof(roleId));
        }

        if (_roleAssignments.Any(x => x.RoleId == roleId))
        {
            return;
        }

        _roleAssignments.Add(new RoleAssignment(Id, roleId, DateTimeOffset.UtcNow));
    }
}

/// <summary>Role assignment relation.</summary>
/// <param name="UserId">User identifier.</param>
/// <param name="RoleId">Role identifier.</param>
/// <param name="AssignedAt">UTC assignment timestamp.</param>
public sealed record RoleAssignment(Guid UserId, Guid RoleId, DateTimeOffset AssignedAt);
