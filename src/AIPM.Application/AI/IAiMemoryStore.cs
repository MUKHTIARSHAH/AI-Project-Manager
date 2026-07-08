using AIPM.Application.AI.Contracts;

namespace AIPM.Application.AI;

/// <summary>
/// Provider-agnostic memory access contract.
/// </summary>
public interface IAiMemoryStore
{
    /// <summary>Reads a memory item by namespace/key.</summary>
    Task<AiMemoryItem?> ReadAsync(AiMemoryQuery query, CancellationToken cancellationToken = default);

    /// <summary>Writes or updates a memory item.</summary>
    Task WriteAsync(AiMemoryItem item, CancellationToken cancellationToken = default);
}
