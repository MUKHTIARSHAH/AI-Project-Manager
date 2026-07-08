using AIPM.Infrastructure.Messaging.Idempotency;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Messaging;

/// <summary>
/// Tests for message-consumer idempotency behavior.
/// </summary>
public sealed class ConsumerIdempotencyStoreTests
{
    /// <summary>Store marks first processing and rejects duplicates.</summary>
    [Fact]
    public void TryMarkProcessed_IsIdempotent()
    {
        var store = new InMemoryConsumerIdempotencyStore();
        var id = Guid.NewGuid();

        store.TryMarkProcessed(id).Should().BeTrue();
        store.TryMarkProcessed(id).Should().BeFalse();
    }
}
