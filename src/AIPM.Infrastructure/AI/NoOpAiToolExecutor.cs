using AIPM.Application.AI;
using AIPM.Application.AI.Contracts;

namespace AIPM.Infrastructure.AI;

/// <summary>
/// No-op tool executor for M6 contract validation.
/// </summary>
public sealed class NoOpAiToolExecutor : IAiToolExecutor
{
    /// <inheritdoc />
    public Task<AiToolResult> ExecuteAsync(AiToolCall call, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AiToolResult(call.CallId, "{}", true));
    }
}
