using AIPM.Infrastructure.Messaging.Contracts;
using AIPM.Infrastructure.Messaging.Health;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AIPM.Infrastructure.Messaging;

/// <summary>
/// MassTransit-backed message bus implementation.
/// </summary>
public sealed class MassTransitMessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitMessageBus> _logger;
    private readonly MessagingPipelineHealthState _healthState;

    /// <summary>Initializes message bus.</summary>
    public MassTransitMessageBus(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitMessageBus> logger,
        MessagingPipelineHealthState healthState)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _healthState = healthState;
    }

    /// <inheritdoc />
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        using var activity = MessagingTelemetry.ActivitySource.StartActivity("messaging.publish");
        if (message is IPlatformMessageContract contract)
        {
            activity?.SetTag("messaging.message_id", contract.MessageId);
            activity?.SetTag("messaging.correlation_id", contract.CorrelationId);
            activity?.SetTag("messaging.causation_id", contract.CausationId);
            activity?.SetTag("messaging.contract_version", contract.ContractVersion);
        }

        try
        {
            await _publishEndpoint.Publish(message, cancellationToken);
            _healthState.ReportSuccess();
            _logger.LogInformation("Published message type={MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _healthState.ReportFailure();
            _logger.LogError(ex, "Failed to publish message type={MessageType}", typeof(T).Name);
            throw;
        }
    }
}
