using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AIPM.Infrastructure.Configuration;

/// <summary>
/// Loads deployment profile overlays from <c>deploy/profiles/</c>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the ADR-SAD-005 deployment profile JSON overlay when present.
    /// </summary>
    public static IConfigurationBuilder AddDeploymentProfile(
        this IConfigurationBuilder builder,
        IHostEnvironment environment,
        string contentRootPath)
    {
        var bootstrap = builder.Build();
        var profile = bootstrap[DeploymentOptions.SectionName + ":Profile"] ?? "saas";
        var profileFile = Path.Combine(
            contentRootPath,
            "..",
            "..",
            "deploy",
            "profiles",
            $"{profile.ToLowerInvariant()}.json");

        if (File.Exists(profileFile))
        {
            builder.AddJsonFile(profileFile, optional: false, reloadOnChange: true);
        }

        return builder;
    }
}
