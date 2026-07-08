namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 portfolio DTO.
/// Trace: FR-001, FR-003, AGG-002.
/// </summary>
public sealed record PortfolioDto(Guid Id, Guid TenantId, string Name, DateTimeOffset CreatedAt);
