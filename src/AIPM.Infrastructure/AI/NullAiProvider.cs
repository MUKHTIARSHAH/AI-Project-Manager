using AIPM.Application.AI;
using AIPM.Application.AI.Contracts;

namespace AIPM.Infrastructure.AI;

/// <summary>
/// Stub AI provider for contract and pipeline validation (no external integration).
/// </summary>
public sealed class NullAiProvider : IAiProvider
{
    /// <inheritdoc />
    public string ProviderKey => "stub";

    /// <inheritdoc />
    public Task<AiCompletionResponse> CompleteAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new AiCompletionResponse(
            request.Provider,
            request.Model,
            "stub-response",
            DateTimeOffset.UtcNow);
        return Task.FromResult(response);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiCompletionRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return new AiStreamChunk(request.Provider, request.Model, "stub-", false);
        await Task.Yield();
        yield return new AiStreamChunk(request.Provider, request.Model, "response", true);
    }
}
