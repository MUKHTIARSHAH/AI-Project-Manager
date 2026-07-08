using AIPM.Application.AI.Contracts;

namespace AIPM.Application.AI;

/// <summary>
/// High-level provider pipeline abstraction for completion and streaming.
/// </summary>
public interface IAiProviderPipeline
{
    /// <summary>Runs a provider completion request.</summary>
    Task<AiCompletionResponse> CompleteAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Runs a provider streaming request.</summary>
    IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default);
}
