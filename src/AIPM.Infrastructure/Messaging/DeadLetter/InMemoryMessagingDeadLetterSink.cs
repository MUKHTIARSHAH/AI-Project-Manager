using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Messaging.DeadLetter;

/// <summary>
/// In-memory dead-letter implementation for platform messaging.
/// </summary>
public sealed class InMemoryMessagingDeadLetterSink : IMessagingDeadLetterSink
{
    private readonly ConcurrentQueue<MessagingDeadLetter> _queue = new();
    private readonly ILogger<InMemoryMessagingDeadLetterSink> _logger;

    /// <summary>Initializes sink.</summary>
    public InMemoryMessagingDeadLetterSink(ILogger<InMemoryMessagingDeadLetterSink> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task EnqueueAsync(MessagingDeadLetter deadLetter, CancellationToken cancellationToken = default)
    {
        _queue.Enqueue(deadLetter);
        _logger.LogError(
            "Message dead-lettered id={MessageId} type={MessageType} reason={Reason}",
            deadLetter.MessageId,
            deadLetter.MessageType,
            deadLetter.Reason);
        return Task.CompletedTask;
    }
}
