using System.ComponentModel.DataAnnotations;

namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// Deployment profile configuration (ADR-SAD-005).
/// </summary>
public sealed class DeploymentOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Deployment";

    /// <summary>Active profile: saas, dedicated, or airgapped.</summary>
    [Required]
    [RegularExpression("^(?i)(saas|dedicated|airgapped)$")]
    public string Profile { get; init; } = "saas";

    /// <summary>Profile capability flags.</summary>
    [Required]
    public DeploymentCapabilities Capabilities { get; init; } = new();
}
