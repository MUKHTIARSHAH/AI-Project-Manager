namespace AIPM.Infrastructure.Messaging.Idempotency;

/// <summary>
/// Stores processed message IDs to guarantee idempotent consumption.
/// </summary>
public interface IConsumerIdempotencyStore
{
    /// <summary>
    /// Marks a message as processed if unseen; returns false when already processed.
    /// </summary>
    bool TryMarkProcessed(Guid messageId);
}
