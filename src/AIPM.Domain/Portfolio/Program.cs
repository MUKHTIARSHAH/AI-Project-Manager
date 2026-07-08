using AIPM.Domain.Primitives;
using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Portfolio;

/// <summary>
/// AGG-003 Program aggregate.
/// Trace: FR-003, CAP-002, CON-007, DDL-001.
/// </summary>
public sealed class ProgramAggregate : AggregateRoot
{
    private ProgramAggregate(Guid id, Guid tenantId, Guid portfolioId, string name, DateTimeOffset createdAt)
    {
        Id = id;
        TenantId = tenantId;
        PortfolioId = portfolioId;
        Name = name;
        CreatedAt = createdAt;
    }

    /// <summary>Program identifier.</summary>
    public Guid Id { get; }

    /// <summary>Owning tenant identifier.</summary>
    public Guid TenantId { get; }

    /// <summary>Owning portfolio identifier.</summary>
    public Guid PortfolioId { get; }

    /// <summary>Program name.</summary>
    public string Name { get; }

    /// <summary>Creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// CMD-011 CreateProgram.
    /// </summary>
    public static ProgramAggregate Create(Guid tenantId, Guid portfolioId, string name)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ValidationError("TenantId is required.");
        }

        if (portfolioId == Guid.Empty)
        {
            throw new ValidationError("PortfolioId is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationError("Program name is required.");
        }

        var aggregate = new ProgramAggregate(Guid.NewGuid(), tenantId, portfolioId, name.Trim(), DateTimeOffset.UtcNow);
        aggregate.Raise(new ProgramCreatedDomainEvent(aggregate.Id, aggregate.TenantId, aggregate.PortfolioId, aggregate.Name));
        return aggregate;
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static ProgramAggregate Rehydrate(Guid id, Guid tenantId, Guid portfolioId, string name, DateTimeOffset createdAt)
        => new(id, tenantId, portfolioId, name, createdAt);
}

/// <summary>
/// EVT-011 ProgramCreated domain event.
/// </summary>
public sealed record ProgramCreatedDomainEvent(Guid ProgramId, Guid TenantId, Guid PortfolioId, string Name) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
