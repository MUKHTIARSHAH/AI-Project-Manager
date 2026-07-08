namespace AIPM.Application.Runtime.Agents;

/// <summary>
/// Platform agent contract — no LLM; dummy agents implement this for M2.
/// </summary>
public interface IPlatformAgent
{
    /// <summary>Unique agent identifier.</summary>
    string AgentId { get; }

    /// <summary>Executes the agent with the given input.</summary>
    Task<AgentExecutionResult> ExecuteAsync(AgentExecutionRequest request, CancellationToken cancellationToken);
}

/// <summary>Agent execution request.</summary>
public sealed record AgentExecutionRequest(string Input);

/// <summary>Agent execution result.</summary>
public sealed record AgentExecutionResult(bool Success, string Output);
