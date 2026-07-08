using AIPM.Plugins.Manifests;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Runtime;

/// <summary>
/// Plugin manifest validation tests.
/// </summary>
public sealed class PluginManifestValidatorTests
{
    /// <summary>Valid manifest passes.</summary>
    [Fact]
    public void Validate_ValidManifest_HasNoErrors()
    {
        var validator = new PluginManifestValidator();
        var manifest = new PluginManifest
        {
            Id = "echo-agent",
            Name = "EchoAgent",
            Version = "1.0.0",
            MinPlatformVersion = "0.1.0",
            Capabilities = ["echo"],
            BuiltInAgent = "echo"
        };

        validator.Validate(manifest).Should().BeEmpty();
    }

    /// <summary>Missing id fails validation.</summary>
    [Fact]
    public void Validate_MissingId_ReturnsError()
    {
        var validator = new PluginManifestValidator();
        var manifest = new PluginManifest { Name = "X", Capabilities = ["a"] };
        validator.Validate(manifest).Should().NotBeEmpty();
    }
}
