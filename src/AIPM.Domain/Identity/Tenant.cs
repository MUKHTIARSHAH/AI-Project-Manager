using AIPM.Domain.Primitives;

namespace AIPM.Domain.Identity;

/// <summary>
/// AGG-001 Tenant aggregate root (BC-10).
/// </summary>
public sealed class Tenant : AggregateRoot
{
    private Tenant()
    {
    }

    /// <summary>Tenant identifier.</summary>
    public Guid Id { get; private set; }

    /// <summary>Display name.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Tenant status.</summary>
    public TenantStatus Status { get; private set; }

    /// <summary>Creates a provisioned tenant.</summary>
    public static Tenant Provision(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Status = TenantStatus.Active
        };
        tenant.Raise(new TenantProvisionedDomainEvent(tenant.Id, DateTimeOffset.UtcNow));
        return tenant;
    }

    /// <summary>Rehydrates existing tenant state.</summary>
    public static Tenant Rehydrate(Guid id, string name, TenantStatus status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Tenant id must be set.", nameof(id));
        }

        return new Tenant
        {
            Id = id,
            Name = name.Trim(),
            Status = status
        };
    }

    /// <summary>Suspends the tenant.</summary>
    public void Suspend()
    {
        if (Status == TenantStatus.Suspended)
        {
            return;
        }

        Status = TenantStatus.Suspended;
        Raise(new TenantSuspendedDomainEvent(Id, DateTimeOffset.UtcNow));
    }
}

/// <summary>Tenant lifecycle statuses.</summary>
public enum TenantStatus
{
    /// <summary>Tenant provisioned and active.</summary>
    Active = 1,
    /// <summary>Tenant suspended by policy/admin action.</summary>
    Suspended = 2
}

/// <summary>EVT-001 TenantProvisioned domain event.</summary>
/// <param name="TenantId">Provisioned tenant identifier.</param>
/// <param name="OccurredAt">UTC occurrence timestamp.</param>
public sealed record TenantProvisionedDomainEvent(Guid TenantId, DateTimeOffset OccurredAt) : IDomainEvent;

/// <summary>EVT-002 TenantSuspended domain event.</summary>
/// <param name="TenantId">Suspended tenant identifier.</param>
/// <param name="OccurredAt">UTC occurrence timestamp.</param>
public sealed record TenantSuspendedDomainEvent(Guid TenantId, DateTimeOffset OccurredAt) : IDomainEvent;
