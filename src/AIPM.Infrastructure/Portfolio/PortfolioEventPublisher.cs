using AIPM.Application.Portfolio;
using AIPM.Infrastructure.Messaging;

namespace AIPM.Infrastructure.Portfolio;

/// <summary>
/// Publishes BC-01 integration events over the message bus.
/// Trace: EVT-010, ADR-SAD-004.
/// </summary>
public sealed class PortfolioEventPublisher : IPortfolioEventPublisher
{
    private readonly IMessageBus _messageBus;

    /// <summary>Initializes publisher.</summary>
    public PortfolioEventPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <inheritdoc />
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        => _messageBus.PublishAsync(message, cancellationToken);
}
