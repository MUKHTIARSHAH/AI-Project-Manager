namespace AIPM.Infrastructure.Requirements.Persistence.Models;

/// <summary>Requirement persistence record (AGG-005).</summary>
public sealed class RequirementRecord
{
    /// <summary>Requirement identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Owning project identifier.</summary>
    public Guid ProjectId { get; set; }

    /// <summary>Requirement title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Requirement statement.</summary>
    public string Statement { get; set; } = string.Empty;

    /// <summary>Business lifecycle status name.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Technical Parsed flag.</summary>
    public bool Parsed { get; set; }

    /// <summary>Optional document title.</summary>
    public string? DocumentTitle { get; set; }

    /// <summary>Optional document content type.</summary>
    public string? DocumentContentType { get; set; }

    /// <summary>Optional document URI or name.</summary>
    public string? DocumentUriOrName { get; set; }

    /// <summary>Creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; set; }
}

/// <summary>Acceptance criterion persistence record (CON-014).</summary>
public sealed class AcceptanceCriterionRecord
{
    /// <summary>Acceptance criterion identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Owning requirement identifier.</summary>
    public Guid RequirementId { get; set; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; set; }

    /// <summary>Testable condition text.</summary>
    public string Statement { get; set; } = string.Empty;

    /// <summary>Sort order within the requirement.</summary>
    public int SortOrder { get; set; }
}
