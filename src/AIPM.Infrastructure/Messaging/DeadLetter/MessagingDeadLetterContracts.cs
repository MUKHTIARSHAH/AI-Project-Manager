namespace AIPM.Infrastructure.Messaging.DeadLetter;

/// <summary>
/// Dead-letter payload for failed message consumption.
/// </summary>
public sealed record MessagingDeadLetter(
    Guid MessageId,
    string MessageType,
    string Payload,
    string Reason,
    DateTimeOffset FailedAt);

/// <summary>
/// Production-ready abstraction for dead-letter handling.
/// </summary>
public interface IMessagingDeadLetterSink
{
    /// <summary>Enqueues a failed message for later inspection/replay.</summary>
    Task EnqueueAsync(MessagingDeadLetter deadLetter, CancellationToken cancellationToken = default);
}
