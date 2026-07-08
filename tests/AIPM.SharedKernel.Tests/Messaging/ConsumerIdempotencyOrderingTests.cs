using AIPM.Infrastructure.Messaging.Idempotency;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Messaging;

/// <summary>
/// Regression tests for consumer idempotency ordering (H-04).
/// </summary>
public sealed class ConsumerIdempotencyOrderingTests
{
    /// <summary>Messages remain retryable until marked processed after success.</summary>
    [Fact]
    public void ProcessingOrder_AllowsRetryUntilMarkedProcessed()
    {
        var store = new InMemoryConsumerIdempotencyStore();
        var messageId = Guid.NewGuid();

        store.HasBeenProcessed(messageId).Should().BeFalse();

        // Handler failure path: do not mark processed.
        store.HasBeenProcessed(messageId).Should().BeFalse();

        store.MarkProcessed(messageId);
        store.HasBeenProcessed(messageId).Should().BeTrue();
        store.HasBeenProcessed(messageId).Should().BeTrue();
    }

    /// <summary>MarkProcessed is idempotent for duplicate deliveries.</summary>
    [Fact]
    public void MarkProcessed_IsIdempotent()
    {
        var store = new InMemoryConsumerIdempotencyStore();
        var messageId = Guid.NewGuid();

        store.MarkProcessed(messageId);
        store.MarkProcessed(messageId);

        store.HasBeenProcessed(messageId).Should().BeTrue();
        store.TryMarkProcessed(messageId).Should().BeFalse();
    }
}
