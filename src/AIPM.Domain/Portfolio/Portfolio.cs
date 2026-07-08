using AIPM.Domain.Primitives;
using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Portfolio;

/// <summary>
/// AGG-002 Portfolio aggregate.
/// Trace: FR-003, FR-122, CAP-002, CON-006, IDL-001.
/// </summary>
public sealed class PortfolioAggregate : AggregateRoot
{
    private PortfolioAggregate(Guid id, Guid tenantId, string name, DateTimeOffset createdAt)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        CreatedAt = createdAt;
    }

    /// <summary>Portfolio identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Display name unique per tenant.</summary>
    public string Name { get; private set; }

    /// <summary>Creation time in UTC.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// CMD-010 CreatePortfolio.
    /// </summary>
    public static PortfolioAggregate Create(Guid tenantId, string name)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ValidationError("TenantId is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationError("Portfolio name is required.");
        }

        var aggregate = new PortfolioAggregate(Guid.NewGuid(), tenantId, name.Trim(), DateTimeOffset.UtcNow);
        aggregate.Raise(new PortfolioCreatedDomainEvent(aggregate.Id, aggregate.TenantId, aggregate.Name));
        return aggregate;
    }

    /// <summary>Rehydrates an existing aggregate from persistence.</summary>
    public static PortfolioAggregate Rehydrate(Guid id, Guid tenantId, string name, DateTimeOffset createdAt)
        => new(id, tenantId, name, createdAt);
}

/// <summary>
/// EVT-010 domain event for created portfolio.
/// Trace: CMD-010, FR-003, FR-122.
/// </summary>
public sealed record PortfolioCreatedDomainEvent(Guid PortfolioId, Guid TenantId, string Name) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
