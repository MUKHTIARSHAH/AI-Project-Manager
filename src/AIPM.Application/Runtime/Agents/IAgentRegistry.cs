namespace AIPM.Application.Runtime.Agents;

/// <summary>
/// Registry for platform agents resolved by capability.
/// </summary>
public interface IAgentRegistry
{
    /// <summary>Registers an agent factory.</summary>
    void Register(AgentDescriptor descriptor, Func<IPlatformAgent> factory);

    /// <summary>Resolves the first agent matching a capability.</summary>
    AgentDescriptor? ResolveByCapability(string capability);

    /// <summary>Gets an agent instance by identifier.</summary>
    IPlatformAgent? ResolveAgent(string agentId);

    /// <summary>Lists registered agents.</summary>
    IReadOnlyList<AgentDescriptor> List();

    /// <summary>Updates agent lifecycle state.</summary>
    void SetLifecycle(string agentId, AgentLifecycleState state);
}

/// <summary>Registered agent metadata.</summary>
public sealed record AgentDescriptor(
    string Id,
    string Name,
    string Version,
    IReadOnlyList<string> Capabilities,
    AgentLifecycleState Lifecycle);

/// <summary>Agent lifecycle states.</summary>
public enum AgentLifecycleState
{
    /// <summary>Discovered but not active.</summary>
    Discovered = 0,

    /// <summary>Registered and ready.</summary>
    Registered = 1,

    /// <summary>Currently executing.</summary>
    Running = 2,

    /// <summary>Idle after execution.</summary>
    Idle = 3,

    /// <summary>Failed registration or execution.</summary>
    Failed = 4
}
