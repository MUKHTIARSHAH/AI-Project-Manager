using AIPM.Application.Runtime.Events;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Events;

/// <summary>
/// In-process event dispatcher with simple retry on handler failure.
/// </summary>
public sealed class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = [];
    private readonly IDeadLetterQueue _deadLetterQueue;
    private readonly ILogger<InMemoryEventDispatcher> _logger;
    private readonly object _lock = new();

    /// <summary>Initializes dispatcher.</summary>
    public InMemoryEventDispatcher(IDeadLetterQueue deadLetterQueue, ILogger<InMemoryEventDispatcher> logger)
    {
        _deadLetterQueue = deadLetterQueue;
        _logger = logger;
    }

    /// <inheritdoc />
    public void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler)
        where TEvent : class, IPlatformEvent
    {
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list))
            {
                list = [];
                _handlers[typeof(TEvent)] = list;
            }

            list.Add(handler);
        }
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent platformEvent, CancellationToken cancellationToken = default)
        where TEvent : class, IPlatformEvent
    {
        List<Delegate> handlers;
        lock (_lock)
        {
            handlers = _handlers.TryGetValue(typeof(TEvent), out var list)
                ? list.ToList()
                : [];
        }

        foreach (var handler in handlers)
        {
            await DispatchWithRetryAsync(
                () => ((Func<TEvent, CancellationToken, Task>)handler)(platformEvent, cancellationToken),
                typeof(TEvent).Name,
                cancellationToken);
        }
    }

    private async Task DispatchWithRetryAsync(Func<Task> dispatch, string eventType, CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await dispatch();
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Event handler failed attempt {Attempt} for {EventType}", attempt, eventType);
                await Task.Delay(TimeSpan.FromMilliseconds(50 * attempt), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event handler failed permanently for {EventType}", eventType);
                await _deadLetterQueue.EnqueueAsync(
                    new DeadLetterMessage(eventType, string.Empty, ex.Message, DateTimeOffset.UtcNow),
                    cancellationToken);
            }
        }
    }
}
