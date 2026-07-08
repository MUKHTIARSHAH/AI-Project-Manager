using AIPM.Application.Runtime.Agents;
using AIPM.Application.Runtime.Events;
using AIPM.Application.Runtime.Workflow;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AIPM.Application.Runtime.Commands;

/// <summary>
/// Command to execute a platform agent by capability.
/// </summary>
public sealed record ExecuteAgentCommand(string Capability, string Input) : IRequest<ExecuteAgentResponse>;

/// <summary>Response from agent execution.</summary>
public sealed record ExecuteAgentResponse(string AgentId, string Output, bool Success);

/// <summary>
/// Dispatches agent execution through workflow runtime and event dispatcher.
/// </summary>
public sealed class ExecuteAgentCommandHandler : IRequestHandler<ExecuteAgentCommand, ExecuteAgentResponse>
{
    private readonly IAgentRegistry _agentRegistry;
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IExecutionContextAccessor _contextAccessor;
    private readonly ILogger<ExecuteAgentCommandHandler> _logger;

    /// <summary>Initializes handler.</summary>
    public ExecuteAgentCommandHandler(
        IAgentRegistry agentRegistry,
        IWorkflowRuntime workflowRuntime,
        IEventDispatcher eventDispatcher,
        IExecutionContextAccessor contextAccessor,
        ILogger<ExecuteAgentCommandHandler> logger)
    {
        _agentRegistry = agentRegistry;
        _workflowRuntime = workflowRuntime;
        _eventDispatcher = eventDispatcher;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ExecuteAgentResponse> Handle(ExecuteAgentCommand request, CancellationToken cancellationToken)
    {
        var descriptor = _agentRegistry.ResolveByCapability(request.Capability)
            ?? throw new NotFoundError($"No agent registered for capability '{request.Capability}'.");

        var agent = _agentRegistry.ResolveAgent(descriptor.Id)
            ?? throw new NotFoundError($"Agent '{descriptor.Id}' could not be resolved.");

        _agentRegistry.SetLifecycle(descriptor.Id, AgentLifecycleState.Running);

        var workflowId = _workflowRuntime.Start($"execute-{descriptor.Id}");
        _logger.LogInformation(
            "Executing agent {AgentId} for capability {Capability} workflow {WorkflowId}",
            descriptor.Id,
            request.Capability,
            workflowId.Value);

        try
        {
            var result = await _workflowRuntime.ExecuteAsync(
                workflowId,
                async ct =>
                {
                    var execution = await agent.ExecuteAsync(new AgentExecutionRequest(request.Input), ct);
                    if (!execution.Success)
                    {
                        throw new InvalidOperationException($"Agent '{descriptor.Id}' failed.");
                    }

                    return execution.Output;
                },
                cancellationToken);

            var correlationId = _contextAccessor.Current?.CorrelationId.Value ?? Guid.NewGuid();
            await _eventDispatcher.PublishAsync(
                new AgentCompletedEvent(
                    descriptor.Id,
                    result.Output ?? string.Empty,
                    correlationId,
                    correlationId,
                    DateTimeOffset.UtcNow),
                cancellationToken);

            _agentRegistry.SetLifecycle(descriptor.Id, AgentLifecycleState.Idle);
            return new ExecuteAgentResponse(descriptor.Id, result.Output ?? string.Empty, true);
        }
        catch
        {
            _agentRegistry.SetLifecycle(descriptor.Id, AgentLifecycleState.Failed);
            throw;
        }
    }
}
