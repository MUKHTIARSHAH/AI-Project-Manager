using AIPM.Application.AI.Contracts;

namespace AIPM.Application.AI;

/// <summary>
/// Provider-independent language model provider contract.
/// </summary>
public interface IAiProvider
{
    /// <summary>Provider identifier key.</summary>
    string ProviderKey { get; }

    /// <summary>Produces a completion response.</summary>
    Task<AiCompletionResponse> CompleteAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Streams completion chunks.</summary>
    IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default);
}
