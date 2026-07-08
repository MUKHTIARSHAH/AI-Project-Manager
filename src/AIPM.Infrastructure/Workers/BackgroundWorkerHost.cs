using AIPM.Application.Runtime.Workers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Workers;

/// <summary>
/// Schedules background work with graceful shutdown support.
/// </summary>
public sealed class BackgroundWorkerHost : IBackgroundWorkerHost, IHostedService
{
    private readonly ILogger<BackgroundWorkerHost> _logger;
    private readonly List<Func<CancellationToken, Task>> _queue = [];
    private readonly SemaphoreSlim _signal = new(0);
    private CancellationTokenSource? _cts;
    private Task? _workerTask;

    /// <summary>Initializes worker host.</summary>
    public BackgroundWorkerHost(ILogger<BackgroundWorkerHost> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task ScheduleAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (_queue)
        {
            _queue.Add(work);
        }

        _signal.Release();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _workerTask = Task.Run(() => ProcessQueueAsync(_cts.Token), CancellationToken.None);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts is not null)
        {
            await _cts.CancelAsync();
        }

        if (_workerTask is not null)
        {
            await _workerTask;
        }
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _signal.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            Func<CancellationToken, Task>? work = null;
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    work = _queue[0];
                    _queue.RemoveAt(0);
                }
            }

            if (work is null)
            {
                continue;
            }

            try
            {
                await work(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background worker task failed");
            }
        }
    }
}
