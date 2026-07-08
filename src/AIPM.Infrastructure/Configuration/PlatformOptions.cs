using System.ComponentModel.DataAnnotations;

namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// Platform-wide host configuration validated at startup.
/// </summary>
public sealed class PlatformOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Platform";

    /// <summary>Logical service name for logs and telemetry.</summary>
    [Required]
    [MinLength(1)]
    public string ServiceName { get; init; } = "aipm-host";

    /// <summary>Require tenant header on API requests (ADR-SAD-006).</summary>
    public bool RequireTenantHeader { get; init; }

    /// <summary>Allow unsigned plugins in development only.</summary>
    public bool AllowUnsignedPluginsDev { get; init; } = true;
}
