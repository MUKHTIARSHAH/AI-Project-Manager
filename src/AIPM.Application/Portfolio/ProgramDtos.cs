namespace AIPM.Application.Portfolio;

/// <summary>
/// BC-01 program DTO.
/// Trace: FR-003, AGG-003.
/// </summary>
public sealed record ProgramDto(Guid Id, Guid TenantId, Guid PortfolioId, string Name, DateTimeOffset CreatedAt);
