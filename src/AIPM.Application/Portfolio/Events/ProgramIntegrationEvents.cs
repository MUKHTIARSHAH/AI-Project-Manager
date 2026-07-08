namespace AIPM.Application.Portfolio.Events;

/// <summary>
/// EVT-011 integration contract.
/// Trace: AGG-003, CMD-011, FR-003.
/// </summary>
public sealed record ProgramCreatedIntegrationEvent
{
    /// <summary>Message identifier.</summary>
    public Guid MessageId { get; init; } = Guid.NewGuid();

    /// <summary>Contract version.</summary>
    public string ContractVersion { get; init; } = "1.0";

    /// <summary>Correlation identifier.</summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>Causation identifier.</summary>
    public Guid CausationId { get; init; } = Guid.NewGuid();

    /// <summary>Event occurrence time.</summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>Program identifier.</summary>
    public Guid ProgramId { get; init; }

    /// <summary>Tenant identifier.</summary>
    public Guid TenantId { get; init; }

    /// <summary>Portfolio identifier.</summary>
    public Guid PortfolioId { get; init; }

    /// <summary>Program name.</summary>
    public string Name { get; init; } = string.Empty;
}
