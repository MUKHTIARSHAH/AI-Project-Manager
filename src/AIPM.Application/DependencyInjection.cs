using AIPM.Application.Identity;
using AIPM.Application.Runtime.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.Application;

/// <summary>
/// Application layer dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers application services.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ExecutionContextBehavior<,>));
        });

        services.AddScoped<Runtime.PlatformRuntimeOrchestrator>();
        services.AddScoped<AI.IAiProviderPipeline, AI.AiProviderPipeline>();
        services.AddScoped<ITenantScope, ExecutionContextTenantScope>();
        services.AddScoped<IDomainEventDispatcher, IdentityDomainEventDispatcher>();
        return services;
    }
}
