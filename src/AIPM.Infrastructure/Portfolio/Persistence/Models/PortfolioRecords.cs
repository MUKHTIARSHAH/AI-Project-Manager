namespace AIPM.Infrastructure.Portfolio.Persistence.Models;

/// <summary>Portfolio persistence record (AGG-002).</summary>
public sealed class PortfolioRecord
{
    /// <summary>Portfolio identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Portfolio name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Program persistence record (AGG-003).</summary>
public sealed class ProgramRecord
{
    /// <summary>Program identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Owning portfolio identifier.</summary>
    public Guid PortfolioId { get; set; }

    /// <summary>Program name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Project persistence record (AGG-004).</summary>
public sealed class ProjectRecord
{
    /// <summary>Project identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Owning program identifier.</summary>
    public Guid ProgramId { get; set; }

    /// <summary>Workspace reference identifier.</summary>
    public Guid WorkspaceId { get; set; }

    /// <summary>Owner user identifier.</summary>
    public Guid OwnerUserId { get; set; }

    /// <summary>Project name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Lifecycle status name.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Archive timestamp when archived.</summary>
    public DateTimeOffset? ArchivedAt { get; set; }
}
