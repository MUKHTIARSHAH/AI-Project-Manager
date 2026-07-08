using System.ComponentModel.DataAnnotations;

namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// OpenTelemetry exporter configuration.
/// </summary>
public sealed class OpenTelemetryOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "OpenTelemetry";

    /// <summary>Service name reported to collectors.</summary>
    [Required]
    [MinLength(1)]
    public string ServiceName { get; init; } = "aipm-host";

    /// <summary>Optional OTLP endpoint URI.</summary>
    public string? OtlpEndpoint { get; init; }
}
