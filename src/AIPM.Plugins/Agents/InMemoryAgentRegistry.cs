using AIPM.Application.Runtime.Agents;

namespace AIPM.Plugins.Agents;

/// <summary>
/// In-memory agent registry.
/// </summary>
public sealed class InMemoryAgentRegistry : IAgentRegistry
{
    private readonly Dictionary<string, (AgentDescriptor Descriptor, Func<IPlatformAgent> Factory)> _agents = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public void Register(AgentDescriptor descriptor, Func<IPlatformAgent> factory)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(factory);
        _agents[descriptor.Id] = (descriptor with { Lifecycle = AgentLifecycleState.Registered }, factory);
    }

    /// <inheritdoc />
    public AgentDescriptor? ResolveByCapability(string capability)
    {
        return _agents.Values
            .Select(x => x.Descriptor)
            .FirstOrDefault(d => d.Capabilities.Contains(capability, StringComparer.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public IPlatformAgent? ResolveAgent(string agentId)
    {
        return _agents.TryGetValue(agentId, out var entry) ? entry.Factory() : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<AgentDescriptor> List()
        => _agents.Values.Select(x => x.Descriptor).ToList();

    /// <inheritdoc />
    public void SetLifecycle(string agentId, AgentLifecycleState state)
    {
        if (_agents.TryGetValue(agentId, out var entry))
        {
            _agents[agentId] = (entry.Descriptor with { Lifecycle = state }, entry.Factory);
        }
    }
}
