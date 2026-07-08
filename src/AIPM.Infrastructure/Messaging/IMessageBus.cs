namespace AIPM.Infrastructure.Messaging;

/// <summary>
/// Abstraction for publishing integration events via message bus (RabbitMQ via MassTransit).
/// </summary>
public interface IMessageBus
{
    /// <summary>Publishes a message to the bus.</summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
