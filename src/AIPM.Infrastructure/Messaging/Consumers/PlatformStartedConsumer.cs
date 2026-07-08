using AIPM.Infrastructure.Messaging.DeadLetter;
using AIPM.Infrastructure.Messaging.Health;
using AIPM.Infrastructure.Messaging.Idempotency;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer for platform startup lifecycle events.
/// </summary>
public sealed class PlatformStartedConsumer : IConsumer<PlatformStartedEvent>
{
    private readonly ILogger<PlatformStartedConsumer> _logger;
    private readonly IConsumerIdempotencyStore _idempotencyStore;
    private readonly IMessagingDeadLetterSink _deadLetterSink;
    private readonly MessagingPipelineHealthState _healthState;

    /// <summary>Initializes consumer.</summary>
    public PlatformStartedConsumer(
        ILogger<PlatformStartedConsumer> logger,
        IConsumerIdempotencyStore idempotencyStore,
        IMessagingDeadLetterSink deadLetterSink,
        MessagingPipelineHealthState healthState)
    {
        _logger = logger;
        _idempotencyStore = idempotencyStore;
        _deadLetterSink = deadLetterSink;
        _healthState = healthState;
    }

    /// <inheritdoc />
    public async Task Consume(ConsumeContext<PlatformStartedEvent> context)
    {
        var message = context.Message;
        using var activity = MessagingTelemetry.ActivitySource.StartActivity("messaging.consume.platform-started");
        activity?.SetTag("messaging.message_id", message.MessageId);
        activity?.SetTag("messaging.correlation_id", message.CorrelationId);
        activity?.SetTag("messaging.causation_id", message.CausationId);
        activity?.SetTag("messaging.contract_version", message.ContractVersion);

        if (!_idempotencyStore.TryMarkProcessed(message.MessageId))
        {
            _logger.LogInformation(
                "Skipping duplicate PlatformStarted message id={MessageId} correlation={CorrelationId}",
                message.MessageId,
                message.CorrelationId);
            return;
        }

        try
        {
            _logger.LogInformation(
                "Consumed PlatformStarted service={ServiceName} startedAt={StartedAt} correlation={CorrelationId}",
                message.ServiceName,
                message.StartedAt,
                message.CorrelationId);
            _healthState.ReportSuccess();
        }
        catch (Exception ex)
        {
            _healthState.ReportFailure();
            await _deadLetterSink.EnqueueAsync(
                new MessagingDeadLetter(
                    message.MessageId,
                    nameof(PlatformStartedEvent),
                    string.Empty,
                    ex.Message,
                    DateTimeOffset.UtcNow),
                context.CancellationToken);
            throw;
        }
    }
}
