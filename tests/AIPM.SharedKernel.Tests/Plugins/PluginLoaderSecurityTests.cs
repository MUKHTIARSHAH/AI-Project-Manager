using AIPM.Plugins;
using AIPM.Plugins.Agents;
using AIPM.Plugins.Loading;
using AIPM.Plugins.Manifests;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AIPM.SharedKernel.Tests.Plugins;

/// <summary>
/// Plugin loader hardening tests (H-07).
/// </summary>
public sealed class PluginLoaderSecurityTests
{
    [Fact]
    public async Task LoadAsync_RejectsUnsignedPlugins_WhenDisabled()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"aipm-plugins-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempRoot);
        var manifestPath = Path.Combine(tempRoot, "manifest.json");
        await File.WriteAllTextAsync(manifestPath, """
            {
              "id": "unsigned-agent",
              "name": "Unsigned",
              "version": "1.0.0",
              "capabilities": ["echo"],
              "builtInAgent": "echo"
            }
            """);

        try
        {
            var loader = new PluginLoader(
                new PluginManifestValidator(),
                new InMemoryPluginRegistry(),
                new InMemoryAgentRegistry(),
                Options.Create(new PluginLoaderOptions
                {
                    ScanPaths = [tempRoot],
                    AllowUnsignedPlugins = false
                }),
                NullLogger<PluginLoader>.Instance);

            var result = await loader.LoadAsync();
            result.Registered.Should().Be(0);
            result.Errors.Should().ContainSingle(e => e.Contains("unsigned plugin rejected", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Directory.Delete(tempRoot, recursive: true);
        }
    }
}
