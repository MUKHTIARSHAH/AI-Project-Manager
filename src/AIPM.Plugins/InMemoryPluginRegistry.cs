namespace AIPM.Plugins;

/// <summary>
/// In-memory plugin registry for Phase 1.
/// </summary>
public sealed class InMemoryPluginRegistry : IPluginRegistry
{
    private readonly List<PluginDescriptor> _plugins = [];

    /// <inheritdoc />
    public void Register(PluginDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        _plugins.Add(descriptor);
    }

    /// <inheritdoc />
    public IReadOnlyList<PluginDescriptor> List() => _plugins.AsReadOnly();
}
