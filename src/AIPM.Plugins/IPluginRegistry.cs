namespace AIPM.Plugins;

/// <summary>
/// Registry for plugins — stub implementation for Phase 1 (IAD §4.3).
/// </summary>
public interface IPluginRegistry
{
    /// <summary>Registers a plugin descriptor.</summary>
    void Register(PluginDescriptor descriptor);

    /// <summary>Lists registered plugins.</summary>
    IReadOnlyList<PluginDescriptor> List();
}
