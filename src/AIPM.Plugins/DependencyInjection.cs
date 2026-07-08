using AIPM.Application.Runtime.Agents;
using AIPM.Application.Runtime.Plugins;
using AIPM.Plugins.Agents;
using AIPM.Plugins.Loading;
using AIPM.Plugins.Manifests;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.Plugins;

/// <summary>
/// Plugin system dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers plugin infrastructure.</summary>
    public static IServiceCollection AddPlugins(this IServiceCollection services, Action<PluginLoaderOptions>? configure = null)
    {
        services.AddSingleton<IPluginRegistry, InMemoryPluginRegistry>();
        services.AddSingleton<IAgentRegistry, InMemoryAgentRegistry>();
        services.AddSingleton<IPluginManifestValidator, PluginManifestValidator>();
        services.AddSingleton<IPluginLoader, PluginLoader>();

        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<PluginLoaderOptions>(_ => { });
        }

        return services;
    }
}
