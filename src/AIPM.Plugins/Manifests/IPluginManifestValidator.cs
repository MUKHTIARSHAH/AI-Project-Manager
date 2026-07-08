namespace AIPM.Plugins.Manifests;

/// <summary>
/// Validates plugin manifests before registration.
/// </summary>
public interface IPluginManifestValidator
{
    /// <summary>Validates a manifest; returns errors if invalid.</summary>
    IReadOnlyList<string> Validate(PluginManifest manifest);
}
