using System.Collections.Concurrent;
using AIPM.Application.AI;
using AIPM.Application.AI.Contracts;

namespace AIPM.Infrastructure.AI;

/// <summary>
/// In-memory memory store abstraction for M6 foundation.
/// </summary>
public sealed class InMemoryAiMemoryStore : IAiMemoryStore
{
    private readonly ConcurrentDictionary<string, AiMemoryItem> _items = new();

    /// <inheritdoc />
    public Task<AiMemoryItem?> ReadAsync(AiMemoryQuery query, CancellationToken cancellationToken = default)
    {
        var key = $"{query.Namespace}:{query.Key}";
        _items.TryGetValue(key, out var item);
        return Task.FromResult(item);
    }

    /// <inheritdoc />
    public Task WriteAsync(AiMemoryItem item, CancellationToken cancellationToken = default)
    {
        var key = $"{item.Namespace}:{item.Key}";
        _items[key] = item with { UpdatedAt = DateTimeOffset.UtcNow };
        return Task.CompletedTask;
    }
}
