namespace AIPM.Application.Runtime.Events;

/// <summary>
/// Marker for platform runtime events (not business domain events).
/// </summary>
public interface IPlatformEvent
{
    /// <summary>When the event occurred.</summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>Correlation identifier.</summary>
    Guid CorrelationId { get; }

    /// <summary>Causation identifier for parent action/message.</summary>
    Guid CausationId { get; }
}

/// <summary>
/// Dispatches platform events to in-process subscribers with retry.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>Publishes an event to subscribers.</summary>
    Task PublishAsync<TEvent>(TEvent platformEvent, CancellationToken cancellationToken = default)
        where TEvent : class, IPlatformEvent;

    /// <summary>Subscribes a handler for an event type.</summary>
    void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
        where TEvent : class, IPlatformEvent;
}

/// <summary>
/// Dead-letter queue for failed event dispatch (design port — M2 stub).
/// </summary>
public interface IDeadLetterQueue
{
    /// <summary>Enqueues a message that could not be processed.</summary>
    Task EnqueueAsync(DeadLetterMessage message, CancellationToken cancellationToken = default);
}

/// <summary>Dead-letter payload.</summary>
public sealed record DeadLetterMessage(
    string EventType,
    string Payload,
    string Reason,
    DateTimeOffset FailedAt);
