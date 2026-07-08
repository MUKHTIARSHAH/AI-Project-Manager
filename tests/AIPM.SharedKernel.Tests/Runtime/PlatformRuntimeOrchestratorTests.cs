using AIPM.Application;
using AIPM.Application.Runtime;
using AIPM.Application.Runtime.Events;
using AIPM.Infrastructure.Events;
using AIPM.Plugins;
using AIPM.SharedKernel.Execution;
using AIPM.Workflow;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.SharedKernel.Tests.Runtime;

/// <summary>
/// End-to-end M2 platform runtime tests (EchoAgent vertical slice).
/// </summary>
public sealed class PlatformRuntimeOrchestratorTests
{
    /// <summary>Full echo agent flow returns Hello.</summary>
    [Fact]
    public async Task RunEchoAgentDemo_ReturnsHello()
    {
        var services = BuildServices(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "plugins"));
        using var scope = services.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<PlatformRuntimeOrchestrator>();

        var result = await orchestrator.RunEchoAgentDemoAsync(CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Output.Should().Be("Hello");
        result.AgentId.Should().Be("echo-agent");
    }

    private static ServiceProvider BuildServices(string pluginsPath)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IExecutionContextAccessor, AsyncLocalExecutionContextAccessor>();
        services.AddSingleton<IDeadLetterQueue, InMemoryDeadLetterQueue>();
        services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();
        services.AddApplication();
        services.AddWorkflow();
        services.AddPlugins(o => o.ScanPaths = [Path.GetFullPath(pluginsPath)]);
        return services.BuildServiceProvider();
    }
}
