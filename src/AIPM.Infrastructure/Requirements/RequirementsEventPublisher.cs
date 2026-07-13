using AIPM.Application.Requirements;
using AIPM.Infrastructure.Messaging;

namespace AIPM.Infrastructure.Requirements;

/// <summary>
/// Publishes BC-02 Requirements integration events over the message bus.
/// Trace: EVT-030, ADR-SAD-004.
/// </summary>
public sealed class RequirementsEventPublisher : IRequirementsEventPublisher
{
    private readonly IMessageBus _messageBus;

    /// <summary>Initializes publisher.</summary>
    public RequirementsEventPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <inheritdoc />
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        => _messageBus.PublishAsync(message, cancellationToken);
}
