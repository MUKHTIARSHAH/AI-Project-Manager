using AIPM.Application;
using AIPM.Application.AI;
using AIPM.Application.AI.Contracts;
using AIPM.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.SharedKernel.Tests.AI;

/// <summary>
/// Tests for provider-independent AI foundation contracts and DI pipeline.
/// </summary>
public sealed class AiProviderPipelineTests
{
    /// <summary>Stub provider is registered and returns deterministic completion.</summary>
    [Fact]
    public async Task CompleteAsync_UsesRegisteredStubProvider()
    {
        var services = BuildServices();
        using var scope = services.CreateScope();
        var pipeline = scope.ServiceProvider.GetRequiredService<IAiProviderPipeline>();

        var response = await pipeline.CompleteAsync(
            new AiCompletionRequest(
                Provider: "stub",
                Model: "stub-model",
                Prompt: new AiPrompt("system", "user")),
            CancellationToken.None);

        response.Provider.Should().Be("stub");
        response.Model.Should().Be("stub-model");
        response.Text.Should().Be("stub-response");
    }

    /// <summary>Provider registry lists registered providers.</summary>
    [Fact]
    public void Registry_ListsStubProvider()
    {
        var services = BuildServices();
        using var scope = services.CreateScope();
        var registry = scope.ServiceProvider.GetRequiredService<IAiProviderRegistry>();

        registry.ListProviders().Should().Contain("stub");
    }

    private static ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(new ConfigurationBuilder().AddInMemoryCollection().Build());
        return services.BuildServiceProvider();
    }
}
