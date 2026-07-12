namespace AIPM.Application.Portfolio;

/// <summary>
/// Portfolio aggregate summary (FR-122 interim live rollup).
/// Trace: FR-122, CAP-002, CON-006, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record PortfolioSummaryDto(
    Guid PortfolioId,
    string PortfolioName,
    int ProgramCount,
    int ProjectCount,
    int DraftProjectCount,
    int ActiveProjectCount,
    int OnHoldProjectCount,
    int CompletedProjectCount,
    int ArchivedProjectCount);

/// <summary>
/// Program aggregate summary (FR-122 interim live rollup).
/// Trace: FR-122, CAP-002, CON-007, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record ProgramSummaryDto(
    Guid ProgramId,
    string ProgramName,
    Guid PortfolioId,
    int ProjectCount,
    int DraftProjectCount,
    int ActiveProjectCount,
    int OnHoldProjectCount,
    int CompletedProjectCount,
    int ArchivedProjectCount);

/// <summary>
/// Project aggregate summary (FR-122 interim live rollup).
/// Trace: FR-122, CAP-001, CON-008, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record ProjectSummaryDto(
    Guid ProjectId,
    string ProjectName,
    Guid ProgramId,
    Guid PortfolioId,
    Guid OwnerUserId,
    Guid WorkspaceId,
    string Status,
    int ScopeChangeCount);
