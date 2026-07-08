namespace AIPM.Domain.Primitives;

/// <summary>
/// Base type for aggregate roots. Business aggregates added in Phase 2 per DM-AIPM-001.
/// </summary>
public abstract class AggregateRoot
{
    /// <summary>Domain events raised by this aggregate.</summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Uncommitted domain events.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Raises a domain event.</summary>
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Clears domain events after persistence.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Marker for domain events. Full catalog in folder 07.
/// </summary>
public interface IDomainEvent
{
    /// <summary>When the event occurred (UTC).</summary>
    DateTimeOffset OccurredAt { get; }
}
