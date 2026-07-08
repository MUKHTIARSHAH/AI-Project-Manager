using AIPM.Application.AI;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.Infrastructure.AI;

/// <summary>
/// Registers provider-independent AI abstractions and stub provider pipeline.
/// </summary>
public static class AiProviderRegistrationExtensions
{
    /// <summary>
    /// Adds AI abstraction dependencies without integrating external providers.
    /// </summary>
    public static IServiceCollection AddAiProviderFoundation(this IServiceCollection services)
    {
        services.AddSingleton<IAiProviderRegistry, InMemoryAiProviderRegistry>();
        services.AddSingleton<NullAiProvider>();
        services.AddSingleton<IAiProvider>(sp => sp.GetRequiredService<NullAiProvider>());
        services.AddSingleton<IAiToolExecutor, NoOpAiToolExecutor>();
        services.AddSingleton<IAiMemoryStore, InMemoryAiMemoryStore>();

        return services;
    }
}
