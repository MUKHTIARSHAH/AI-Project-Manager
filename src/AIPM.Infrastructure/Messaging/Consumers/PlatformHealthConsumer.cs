using AIPM.Infrastructure.Messaging.DeadLetter;
using AIPM.Infrastructure.Messaging.Health;
using AIPM.Infrastructure.Messaging.Idempotency;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer for platform health lifecycle events.
/// </summary>
public sealed class PlatformHealthConsumer : IConsumer<PlatformHealthEvent>
{
    private readonly ILogger<PlatformHealthConsumer> _logger;
    private readonly IConsumerIdempotencyStore _idempotencyStore;
    private readonly IMessagingDeadLetterSink _deadLetterSink;
    private readonly MessagingPipelineHealthState _healthState;

    /// <summary>Initializes consumer.</summary>
    public PlatformHealthConsumer(
        ILogger<PlatformHealthConsumer> logger,
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
    public async Task Consume(ConsumeContext<PlatformHealthEvent> context)
    {
        var message = context.Message;
        using var activity = MessagingTelemetry.ActivitySource.StartActivity("messaging.consume.platform-health");
        activity?.SetTag("messaging.message_id", message.MessageId);
        activity?.SetTag("messaging.correlation_id", message.CorrelationId);
        activity?.SetTag("messaging.causation_id", message.CausationId);
        activity?.SetTag("messaging.contract_version", message.ContractVersion);

        if (_idempotencyStore.HasBeenProcessed(message.MessageId))
        {
            _logger.LogInformation(
                "Skipping duplicate PlatformHealth message id={MessageId}",
                message.MessageId);
            return;
        }

        try
        {
            _logger.LogInformation(
                "Consumed PlatformHealth service={ServiceName} status={Status} correlation={CorrelationId}",
                message.ServiceName,
                message.Status,
                message.CorrelationId);
            _healthState.ReportSuccess();
            _idempotencyStore.MarkProcessed(message.MessageId);
        }
        catch (Exception ex)
        {
            _healthState.ReportFailure();
            await _deadLetterSink.EnqueueAsync(
                new MessagingDeadLetter(
                    message.MessageId,
                    nameof(PlatformHealthEvent),
                    string.Empty,
                    ex.Message,
                    DateTimeOffset.UtcNow),
                context.CancellationToken);
            throw;
        }
    }
}
