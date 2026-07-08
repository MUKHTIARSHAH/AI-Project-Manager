using System.Text.Json;
using AIPM.Application.Runtime.Agents;
using AIPM.Application.Runtime.Plugins;
using AIPM.Plugins.BuiltIn;
using AIPM.Plugins.Manifests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIPM.Plugins.Loading;

/// <summary>
/// Options for plugin discovery paths.
/// </summary>
public sealed class PluginLoaderOptions
{
    /// <summary>Root paths to scan for plugin manifests.</summary>
    public string[] ScanPaths { get; set; } = ["plugins"];
}

/// <summary>
/// Discovers manifest.json files and registers built-in agents.
/// </summary>
public sealed class PluginLoader : IPluginLoader
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IPluginManifestValidator _validator;
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IAgentRegistry _agentRegistry;
    private readonly PluginLoaderOptions _options;
    private readonly ILogger<PluginLoader> _logger;

    /// <summary>Initializes loader.</summary>
    public PluginLoader(
        IPluginManifestValidator validator,
        IPluginRegistry pluginRegistry,
        IAgentRegistry agentRegistry,
        IOptions<PluginLoaderOptions> options,
        ILogger<PluginLoader> logger)
    {
        _validator = validator;
        _pluginRegistry = pluginRegistry;
        _agentRegistry = agentRegistry;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<PluginLoadResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var discovered = 0;
        var registered = 0;

        foreach (var root in _options.ScanPaths)
        {
            if (!Directory.Exists(root))
            {
                _logger.LogWarning("Plugin scan path not found: {Path}", root);
                continue;
            }

            foreach (var manifestPath in Directory.EnumerateFiles(root, "manifest.json", SearchOption.AllDirectories))
            {
                cancellationToken.ThrowIfCancellationRequested();
                discovered++;

                try
                {
                    var json = File.ReadAllText(manifestPath);
                    var manifest = JsonSerializer.Deserialize<PluginManifest>(json, _jsonOptions);
                    if (manifest is null)
                    {
                        errors.Add($"Could not parse {manifestPath}");
                        continue;
                    }

                    var validationErrors = _validator.Validate(manifest);
                    if (validationErrors.Count > 0)
                    {
                        errors.AddRange(validationErrors.Select(e => $"{manifestPath}: {e}"));
                        continue;
                    }

                    _pluginRegistry.Register(new PluginDescriptor(
                        manifest.Id,
                        manifest.Name,
                        manifest.Version,
                        manifest.Capabilities));

                    if (TryCreateAgentFactory(manifest, out var factory))
                    {
                        _agentRegistry.Register(
                            new AgentDescriptor(
                                manifest.Id,
                                manifest.Name,
                                manifest.Version,
                                manifest.Capabilities,
                                AgentLifecycleState.Discovered),
                            factory);
                        registered++;
                        _logger.LogInformation("Registered agent {AgentId} from {Path}", manifest.Id, manifestPath);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{manifestPath}: {ex.Message}");
                }
            }
        }

        return Task.FromResult(new PluginLoadResult(discovered, registered, errors));
    }

    private static bool TryCreateAgentFactory(PluginManifest manifest, out Func<IPlatformAgent> factory)
    {
        factory = manifest.BuiltInAgent?.ToLowerInvariant() switch
        {
            "echo" or "echoagent" => () => new EchoAgent(),
            _ => () => throw new NotSupportedException($"Unknown built-in agent '{manifest.BuiltInAgent}'")
        };

        return !string.IsNullOrWhiteSpace(manifest.BuiltInAgent);
    }
}
