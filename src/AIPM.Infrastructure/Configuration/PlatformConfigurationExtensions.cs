using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// Registers validated platform configuration options.
/// </summary>
public static class PlatformConfigurationExtensions
{
    /// <summary>Binds and validates deployment, platform, and telemetry options.</summary>
    public static IServiceCollection AddPlatformConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DeploymentOptions>()
            .Bind(configuration.GetSection(DeploymentOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<PlatformOptions>()
            .Bind(configuration.GetSection(PlatformOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<OpenTelemetryOptions>()
            .Bind(configuration.GetSection(OpenTelemetryOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
