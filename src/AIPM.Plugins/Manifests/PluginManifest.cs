namespace AIPM.Plugins.Manifests;

/// <summary>
/// Plugin manifest schema (JSON on disk).
/// </summary>
public sealed class PluginManifest
{
    /// <summary>Unique plugin identifier.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Semantic version.</summary>
    public string Version { get; init; } = "1.0.0";

    /// <summary>Minimum platform version (semver).</summary>
    public string MinPlatformVersion { get; init; } = "0.1.0";

    /// <summary>Capabilities exposed by this plugin.</summary>
    public List<string> Capabilities { get; init; } = [];

    /// <summary>Built-in agent key when no external assembly is loaded.</summary>
    public string? BuiltInAgent { get; init; }

    /// <summary>Optional plugin signature for non-development environments.</summary>
    public string? Signature { get; init; }
}
