namespace AIPM.Application.Runtime.Plugins;

/// <summary>
/// Discovers and validates plugin manifests from configured paths.
/// </summary>
public interface IPluginLoader
{
    /// <summary>Discovers plugins and registers agents.</summary>
    Task<PluginLoadResult> LoadAsync(CancellationToken cancellationToken = default);
}

/// <summary>Result of a plugin load operation.</summary>
public sealed record PluginLoadResult(int Discovered, int Registered, IReadOnlyList<string> Errors);
