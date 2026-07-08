using AIPM.Application.Runtime;
using AIPM.Application.Runtime.Events;

namespace AIPM.Host.Runtime;

/// <summary>
/// Runs the M2 EchoAgent vertical slice on startup (Development only).
/// </summary>
public sealed class PlatformRuntimeHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<PlatformRuntimeHostedService> _logger;

    /// <summary>Initializes hosted service.</summary>
    public PlatformRuntimeHostedService(
        IServiceProvider services,
        IHostEnvironment environment,
        ILogger<PlatformRuntimeHostedService> logger)
    {
        _services = services;
        _environment = environment;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_environment.IsEnvironment("Testing"))
        {
            return Task.CompletedTask;
        }

        var dispatcher = _services.GetRequiredService<IEventDispatcher>();
        dispatcher.Subscribe<AgentCompletedEvent>((evt, _) =>
        {
            _logger.LogInformation(
                "AgentCompleted event agent={AgentId} output={Output} correlation={CorrelationId} causation={CausationId}",
                evt.AgentId,
                evt.Output,
                evt.CorrelationId,
                evt.CausationId);
            return Task.CompletedTask;
        });

        if (!_environment.IsDevelopment())
        {
            return Task.CompletedTask;
        }

        var workerHost = _services.GetRequiredService<Application.Runtime.Workers.IBackgroundWorkerHost>();
        return workerHost.ScheduleAsync(async ct =>
        {
            using var scope = _services.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<PlatformRuntimeOrchestrator>();
            var result = await orchestrator.RunEchoAgentDemoAsync(ct);
            _logger.LogInformation("M2 runtime demo succeeded output={Output}", result.Output);
        }, cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
