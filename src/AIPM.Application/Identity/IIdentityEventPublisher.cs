namespace AIPM.Application.Identity;

/// <summary>Publishes BC-10 integration events.</summary>
public interface IIdentityEventPublisher
{
    /// <summary>Publishes an integration event.</summary>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
