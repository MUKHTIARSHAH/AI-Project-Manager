namespace AIPM.Plugins;

/// <summary>
/// Metadata for a registered plugin (agent or extension).
/// </summary>
public sealed record PluginDescriptor(
    string Id,
    string Name,
    string Version,
    IReadOnlyList<string> Capabilities);
