using AIPM.Application.Identity.Events;
using AIPM.Domain.Identity;
using AIPM.Domain.Primitives;

namespace AIPM.Application.Identity;

/// <summary>
/// Dispatches BC-10 domain events to integration events after successful persistence.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>Maps and publishes domain events raised by an aggregate.</summary>
    Task DispatchAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
}

/// <summary>Maps identity domain events to integration events.</summary>
public sealed class IdentityDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IIdentityEventPublisher _eventPublisher;

    /// <summary>Initializes dispatcher.</summary>
    public IdentityDomainEventDispatcher(IIdentityEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task DispatchAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in aggregate.DomainEvents)
        {
            switch (domainEvent)
            {
                case TenantProvisionedDomainEvent provisioned when aggregate is Tenant tenant:
                    await _eventPublisher.PublishAsync(new TenantProvisionedIntegrationEvent
                    {
                        TenantId = provisioned.TenantId,
                        Name = tenant.Name,
                        OccurredAt = provisioned.OccurredAt
                    }, cancellationToken);
                    break;
                case TenantSuspendedDomainEvent suspended when aggregate is Tenant tenant:
                    await _eventPublisher.PublishAsync(new TenantSuspendedIntegrationEvent
                    {
                        TenantId = suspended.TenantId,
                        Name = tenant.Name,
                        OccurredAt = suspended.OccurredAt
                    }, cancellationToken);
                    break;
            }
        }

        aggregate.ClearDomainEvents();
    }
}
