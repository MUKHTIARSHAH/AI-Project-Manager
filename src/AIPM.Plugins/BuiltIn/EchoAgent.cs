using AIPM.Application.Runtime.Agents;

namespace AIPM.Plugins.BuiltIn;

/// <summary>
/// Dummy echo agent for M2 runtime validation — no AI.
/// </summary>
public sealed class EchoAgent : IPlatformAgent
{
    /// <inheritdoc />
    public string AgentId => "echo-agent";

    /// <inheritdoc />
    public Task<AgentExecutionResult> ExecuteAsync(AgentExecutionRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new AgentExecutionResult(true, request.Input));
    }
}
