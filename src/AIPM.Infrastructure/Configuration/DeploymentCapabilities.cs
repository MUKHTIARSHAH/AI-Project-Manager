namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// Capability flags toggled per deployment profile (ADR-SAD-005).
/// </summary>
public sealed class DeploymentCapabilities
{
    /// <summary>Allow outbound public internet egress.</summary>
    public bool PublicEgress { get; init; } = true;

    /// <summary>Use shared multi-tenant agent pools.</summary>
    public bool SharedAgentPools { get; init; } = true;

    /// <summary>Enable cloud-hosted LLM routing (disabled in air-gapped).</summary>
    public bool CloudLlmRoutes { get; init; } = true;

    /// <summary>Dedicated compute pools for tenant isolation.</summary>
    public bool DedicatedCompute { get; init; }

    /// <summary>Customer-managed encryption keys.</summary>
    public bool CustomerManagedKeys { get; init; }

    /// <summary>Store-and-forward for offline updates.</summary>
    public bool StoreAndForwardUpdates { get; init; }
}
