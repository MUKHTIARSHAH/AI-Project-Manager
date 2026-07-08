namespace AIPM.Infrastructure.Identity.Persistence.Models;

/// <summary>Tenant persistence record.</summary>
public sealed class TenantRecord
{
    /// <summary>Tenant id.</summary>
    public Guid Id { get; set; }
    /// <summary>Tenant name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Tenant status.</summary>
    public string Status { get; set; } = string.Empty;
}

/// <summary>User persistence record.</summary>
public sealed class UserRecord
{
    /// <summary>User id.</summary>
    public Guid Id { get; set; }
    /// <summary>Tenant id.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Email address.</summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>Role persistence record.</summary>
public sealed class RoleRecord
{
    /// <summary>Role id.</summary>
    public Guid Id { get; set; }
    /// <summary>Tenant id.</summary>
    public Guid TenantId { get; set; }
    /// <summary>Role name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Fail closed flag.</summary>
    public bool FailClosed { get; set; } = true;
}

/// <summary>Role assignment record.</summary>
public sealed class RoleAssignmentRecord
{
    /// <summary>User id.</summary>
    public Guid UserId { get; set; }
    /// <summary>Role id.</summary>
    public Guid RoleId { get; set; }
    /// <summary>Assignment time.</summary>
    public DateTimeOffset AssignedAt { get; set; }
}

/// <summary>Permission assignment record.</summary>
public sealed class PermissionAssignmentRecord
{
    /// <summary>Role id.</summary>
    public Guid RoleId { get; set; }
    /// <summary>Permission code.</summary>
    public string PermissionCode { get; set; } = string.Empty;
    /// <summary>Assignment time.</summary>
    public DateTimeOffset AssignedAt { get; set; }
}
