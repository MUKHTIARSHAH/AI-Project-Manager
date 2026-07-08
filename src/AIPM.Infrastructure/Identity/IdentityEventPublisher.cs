using AIPM.Application.Identity;
using AIPM.Infrastructure.Messaging;

namespace AIPM.Infrastructure.Identity;

/// <summary>
/// Publishes BC-10 integration events over the existing message bus.
/// </summary>
public sealed class IdentityEventPublisher : IIdentityEventPublisher
{
    private readonly IMessageBus _messageBus;

    /// <summary>Initializes publisher.</summary>
    public IdentityEventPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <inheritdoc />
    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        => _messageBus.PublishAsync(message, cancellationToken);
}
