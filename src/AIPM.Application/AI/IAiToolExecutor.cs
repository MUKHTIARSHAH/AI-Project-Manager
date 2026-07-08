using AIPM.Application.AI.Contracts;

namespace AIPM.Application.AI;

/// <summary>
/// Executes tool calls requested by provider pipeline.
/// </summary>
public interface IAiToolExecutor
{
    /// <summary>Executes the requested tool call.</summary>
    Task<AiToolResult> ExecuteAsync(AiToolCall call, CancellationToken cancellationToken = default);
}
