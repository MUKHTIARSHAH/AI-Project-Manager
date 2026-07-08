using System.Text.RegularExpressions;

namespace AIPM.Plugins.Manifests;

/// <summary>
/// Default manifest validator.
/// </summary>
public sealed partial class PluginManifestValidator : IPluginManifestValidator
{
    private static readonly Version _platformVersion = new(0, 1, 0);

    /// <inheritdoc />
    public IReadOnlyList<string> Validate(PluginManifest manifest)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(manifest.Id))
        {
            errors.Add("Manifest id is required.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Name))
        {
            errors.Add("Manifest name is required.");
        }

        if (!SemVerRegex().IsMatch(manifest.Version))
        {
            errors.Add($"Invalid version '{manifest.Version}'.");
        }

        if (!Version.TryParse(manifest.MinPlatformVersion, out var minVersion)
            || minVersion > _platformVersion)
        {
            errors.Add($"Plugin requires platform {manifest.MinPlatformVersion}; current is {_platformVersion}.");
        }

        if (manifest.Capabilities.Count == 0)
        {
            errors.Add("At least one capability is required.");
        }

        return errors;
    }

    [GeneratedRegex(@"^\d+\.\d+\.\d+")]
    private static partial Regex SemVerRegex();
}
