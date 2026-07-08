using AIPM.Plugins;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Plugins;

/// <summary>
/// Tests for plugin registry.
/// </summary>
public sealed class PluginRegistryTests
{
    /// <summary>Register and list plugins.</summary>
    [Fact]
    public void Register_AddsPlugin()
    {
        var registry = new InMemoryPluginRegistry();
        registry.Register(new PluginDescriptor("p1", "Test", "1.0.0", ["cap.a"]));
        registry.List().Should().HaveCount(1);
        registry.List()[0].Id.Should().Be("p1");
    }
}
