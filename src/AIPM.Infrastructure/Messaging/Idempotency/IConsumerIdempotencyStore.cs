namespace AIPM.Infrastructure.Messaging.Idempotency;

/// <summary>
/// Stores processed message IDs to guarantee idempotent consumption.
/// </summary>
public interface IConsumerIdempotencyStore
{
    /// <summary>Returns true when the message was already processed successfully.</summary>
    bool HasBeenProcessed(Guid messageId);

    /// <summary>Marks a message as processed after successful handling.</summary>
    void MarkProcessed(Guid messageId);

    /// <summary>
    /// Marks a message as processed if unseen; returns false when already processed.
    /// </summary>
    bool TryMarkProcessed(Guid messageId);
}
