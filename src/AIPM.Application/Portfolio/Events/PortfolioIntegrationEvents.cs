namespace AIPM.Application.Portfolio.Events;

/// <summary>
/// EVT-010 integration contract.
/// Trace: AGG-002, CMD-010, FR-122.
/// </summary>
public sealed record PortfolioCreatedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>Event occurrence timestamp.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Portfolio identifier.</summary>
    public Guid PortfolioId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Portfolio name.</summary>
    public string Name { get; init; } = string.Empty;
}
