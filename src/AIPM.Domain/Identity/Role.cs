using AIPM.Domain.Primitives;

namespace AIPM.Domain.Identity;

/// <summary>
/// Role aggregate for BC-10 identity and access.
/// </summary>
public sealed class Role : AggregateRoot
{
    private readonly List<PermissionAssignment> _permissions = [];

    private Role()
    {
    }

    /// <summary>Role identifier.</summary>
    public Guid Id { get; private set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Role name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Identity policy metadata for this role.</summary>
    public IdentityPolicy IdentityPolicy { get; private set; } = IdentityPolicy.FailClosedDefault;

    /// <summary>Permission assignments.</summary>
    public IReadOnlyCollection<PermissionAssignment> PermissionAssignments => _permissions.AsReadOnly();

    /// <summary>Creates a role in a tenant.</summary>
    public static Role Create(Guid tenantId, string name)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId must be set.", nameof(tenantId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name.Trim()
        };
    }

    /// <summary>Rehydrates role with assigned permissions.</summary>
    public static Role Rehydrate(
        Guid id,
        Guid tenantId,
        string name,
        IdentityPolicy identityPolicy,
        IEnumerable<PermissionAssignment> permissions)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("RoleId must be set.", nameof(id));
        }

        var role = Create(tenantId, name);
        role.Id = id;
        role.IdentityPolicy = identityPolicy;
        role._permissions.Clear();
        role._permissions.AddRange(permissions);
        return role;
    }

    /// <summary>Adds a permission assignment to role.</summary>
    public void AssignPermission(Permission permission)
    {
        if (_permissions.Any(x => x.Permission.Code.Equals(permission.Code, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        _permissions.Add(new PermissionAssignment(Id, permission, DateTimeOffset.UtcNow));
    }
}

/// <summary>
/// Permission value object.
/// </summary>
public sealed record Permission
{
    /// <summary>Permission code string.</summary>
    public string Code { get; }

    /// <summary>Creates permission value object.</summary>
    public Permission(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code.Trim().ToLowerInvariant();
    }
}

/// <summary>Permission assignment relation.</summary>
/// <param name="RoleId">Role identifier.</param>
/// <param name="Permission">Assigned permission.</param>
/// <param name="AssignedAt">UTC assignment timestamp.</param>
public sealed record PermissionAssignment(Guid RoleId, Permission Permission, DateTimeOffset AssignedAt);

/// <summary>Identity policy shape (fail closed by default).</summary>
/// <param name="FailClosed">When true, deny unless explicitly permitted.</param>
public sealed record IdentityPolicy(bool FailClosed)
{
    /// <summary>Default policy setting.</summary>
    public static IdentityPolicy FailClosedDefault => new(true);
}
