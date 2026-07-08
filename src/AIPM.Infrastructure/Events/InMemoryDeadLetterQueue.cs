using AIPM.Application.Runtime.Events;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Events;

/// <summary>
/// In-memory dead-letter queue stub for M2 design validation.
/// </summary>
public sealed class InMemoryDeadLetterQueue : IDeadLetterQueue
{
    private readonly ILogger<InMemoryDeadLetterQueue> _logger;

    /// <summary>Initializes queue.</summary>
    public InMemoryDeadLetterQueue(ILogger<InMemoryDeadLetterQueue> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task EnqueueAsync(DeadLetterMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Dead-letter enqueued type={EventType} reason={Reason}",
            message.EventType,
            message.Reason);
        return Task.CompletedTask;
    }
}
