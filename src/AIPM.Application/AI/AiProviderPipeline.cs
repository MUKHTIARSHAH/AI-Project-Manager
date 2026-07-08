using AIPM.Application.AI.Contracts;
using AIPM.SharedKernel.Errors;
using Microsoft.Extensions.Logging;

namespace AIPM.Application.AI;

/// <summary>
/// Default provider pipeline implementation using registry resolution.
/// </summary>
public sealed class AiProviderPipeline : IAiProviderPipeline
{
    private readonly IAiProviderRegistry _registry;
    private readonly ILogger<AiProviderPipeline> _logger;

    /// <summary>Initializes provider pipeline.</summary>
    public AiProviderPipeline(
        IAiProviderRegistry registry,
        ILogger<AiProviderPipeline> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AiCompletionResponse> CompleteAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var provider = _registry.Resolve(request.Provider)
            ?? throw new NotFoundError($"AI provider '{request.Provider}' is not registered.");

        _logger.LogInformation(
            "AI pipeline completion provider={Provider} model={Model}",
            request.Provider,
            request.Model);

        return await provider.CompleteAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        var provider = _registry.Resolve(request.Provider)
            ?? throw new NotFoundError($"AI provider '{request.Provider}' is not registered.");

        _logger.LogInformation(
            "AI pipeline streaming provider={Provider} model={Model}",
            request.Provider,
            request.Model);

        return provider.StreamAsync(request, cancellationToken);
    }
}
