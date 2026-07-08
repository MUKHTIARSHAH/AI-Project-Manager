using AIPM.Application.Runtime.Commands;
using AIPM.Application.Runtime.Plugins;
using AIPM.SharedKernel.Execution;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIPM.Application.Runtime;

/// <summary>
/// Orchestrates the M2 EchoAgent vertical slice demonstration.
/// </summary>
public sealed class PlatformRuntimeOrchestrator
{
    private readonly IPluginLoader _pluginLoader;
    private readonly IMediator _mediator;
    private readonly IExecutionContextAccessor _contextAccessor;
    private readonly ILogger<PlatformRuntimeOrchestrator> _logger;

    /// <summary>Initializes orchestrator.</summary>
    public PlatformRuntimeOrchestrator(
        IPluginLoader pluginLoader,
        IMediator mediator,
        IExecutionContextAccessor contextAccessor,
        ILogger<PlatformRuntimeOrchestrator> logger)
    {
        _pluginLoader = pluginLoader;
        _mediator = mediator;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Runs plugin discovery → agent execution → event publish flow.
    /// </summary>
    public async Task<ExecuteAgentResponse> RunEchoAgentDemoAsync(CancellationToken cancellationToken)
    {
        var loadResult = await _pluginLoader.LoadAsync(cancellationToken);
        _logger.LogInformation(
            "Plugin loader discovered {Discovered} registered {Registered}",
            loadResult.Discovered,
            loadResult.Registered);

        if (loadResult.Errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join("; ", loadResult.Errors));
        }

        using (_contextAccessor.Push(RuntimeExecutionContext.Create(cancellationToken: cancellationToken)))
        {
            var response = await _mediator.Send(new ExecuteAgentCommand("echo", "Hello"), cancellationToken);
            _logger.LogInformation(
                "EchoAgent demo completed agent={AgentId} output={Output}",
                response.AgentId,
                response.Output);
            return response;
        }
    }
}
