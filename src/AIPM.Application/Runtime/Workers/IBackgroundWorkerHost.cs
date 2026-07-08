namespace AIPM.Application.Runtime.Workers;

/// <summary>
/// Hosts background platform workers with graceful shutdown.
/// </summary>
public interface IBackgroundWorkerHost
{
    /// <summary>Schedules work on the worker host.</summary>
    Task ScheduleAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default);
}
