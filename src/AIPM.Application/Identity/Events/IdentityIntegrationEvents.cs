namespace AIPM.Application.Identity.Events;

/// <summary>EVT-001 integration event payload.</summary>
public sealed record TenantProvisionedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();
    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";
    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();
    /// <summary>Occurrence time.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    /// <summary>Tenant id.</summary>
    public Guid TenantId { get; init; }
    /// <summary>Tenant name.</summary>
    public string Name { get; init; } = string.Empty;
}

/// <summary>EVT-002 integration event payload.</summary>
public sealed record TenantSuspendedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();
    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";
    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();
    /// <summary>Occurrence time.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    /// <summary>Tenant id.</summary>
    public Guid TenantId { get; init; }
    /// <summary>Tenant name.</summary>
    public string Name { get; init; } = string.Empty;
}
