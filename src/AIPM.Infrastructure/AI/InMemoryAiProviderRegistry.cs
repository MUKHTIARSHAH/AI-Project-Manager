using AIPM.Application.AI;

namespace AIPM.Infrastructure.AI;

/// <summary>
/// In-memory AI provider registry for platform provider pipeline.
/// </summary>
public sealed class InMemoryAiProviderRegistry : IAiProviderRegistry
{
    private readonly Dictionary<string, Func<IAiProvider>> _providers =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Initializes registry and pre-registers known providers.</summary>
    public InMemoryAiProviderRegistry(IEnumerable<IAiProvider> providers)
    {
        foreach (var provider in providers)
        {
            Register(provider.ProviderKey, () => provider);
        }
    }

    /// <inheritdoc />
    public void Register(string providerKey, Func<IAiProvider> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(providerKey);
        ArgumentNullException.ThrowIfNull(factory);
        _providers[providerKey] = factory;
    }

    /// <inheritdoc />
    public IAiProvider? Resolve(string providerKey)
    {
        return _providers.TryGetValue(providerKey, out var factory) ? factory() : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> ListProviders() => _providers.Keys.OrderBy(x => x).ToList();
}
