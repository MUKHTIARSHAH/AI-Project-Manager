using AIPM.Application.Runtime.Agents;

namespace AIPM.Application.Runtime.Contracts;

/// <summary>
/// Contract constants for the Phase 1 agent SDK surface.
/// </summary>
public static class AgentSdkContract
{
    /// <summary>
    /// Semantic version of the public agent-type catalog contract.
    /// </summary>
    public const string SchemaVersion = "1.0.0";
}

/// <summary>
/// Public catalog contract for a registered agent type.
/// </summary>
public sealed record AgentTypeContract(
    string Id,
    string Name,
    string Version,
    IReadOnlyList<string> Capabilities,
    string Lifecycle);

/// <summary>
/// API response contract for the agent-type catalog.
/// </summary>
public sealed record AgentTypeCatalogResponse(
    string SchemaVersion,
    IReadOnlyList<AgentTypeContract> AgentTypes);

/// <summary>
/// Maps internal registry descriptors to public catalog contracts.
/// </summary>
public static class AgentTypeContractMapper
{
    /// <summary>
    /// Converts an internal descriptor to a public contract model.
    /// </summary>
    public static AgentTypeContract ToContract(AgentDescriptor descriptor)
        => new(
            descriptor.Id,
            descriptor.Name,
            descriptor.Version,
            descriptor.Capabilities,
            descriptor.Lifecycle.ToString());
}
