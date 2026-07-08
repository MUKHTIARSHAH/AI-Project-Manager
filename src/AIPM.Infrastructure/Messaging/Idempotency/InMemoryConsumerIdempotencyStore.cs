using System.Collections.Concurrent;

namespace AIPM.Infrastructure.Messaging.Idempotency;

/// <summary>
/// In-memory idempotency store for message consumer processing.
/// </summary>
public sealed class InMemoryConsumerIdempotencyStore : IConsumerIdempotencyStore
{
    private readonly ConcurrentDictionary<Guid, byte> _processed = new();

    /// <inheritdoc />
    public bool TryMarkProcessed(Guid messageId)
        => _processed.TryAdd(messageId, 0);
}
