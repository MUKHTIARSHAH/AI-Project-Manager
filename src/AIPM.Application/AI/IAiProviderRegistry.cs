namespace AIPM.Application.AI;

/// <summary>
/// Registry for configured AI providers.
/// </summary>
public interface IAiProviderRegistry
{
    /// <summary>Registers a provider factory by key.</summary>
    void Register(string providerKey, Func<IAiProvider> factory);

    /// <summary>Resolves a provider by key, if registered.</summary>
    IAiProvider? Resolve(string providerKey);

    /// <summary>Lists registered provider keys.</summary>
    IReadOnlyList<string> ListProviders();
}
