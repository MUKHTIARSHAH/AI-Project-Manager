using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Queries;

/// <summary>
/// FR-122 portfolio summary projection query.
/// Trace: FR-122, CAP-002, CON-006, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record GetPortfolioSummaryQuery(Guid PortfolioId) : IRequest<PortfolioSummaryDto>;

/// <summary>
/// FR-122 program summary projection query.
/// Trace: FR-122, CAP-002, CON-007, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record GetProgramSummaryQuery(Guid ProgramId) : IRequest<ProgramSummaryDto>;

/// <summary>
/// FR-122 project summary projection query.
/// Trace: FR-122, CAP-001, CON-008, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record GetProjectSummaryQuery(Guid ProjectId) : IRequest<ProjectSummaryDto>;

/// <summary>
/// Handles FR-122 projection summary queries.
/// Trace: FR-122, ADR-SAD-008, ADR-005.
/// </summary>
public sealed class ProjectionSummaryQueryHandler :
    IRequestHandler<GetPortfolioSummaryQuery, PortfolioSummaryDto>,
    IRequestHandler<GetProgramSummaryQuery, ProgramSummaryDto>,
    IRequestHandler<GetProjectSummaryQuery, ProjectSummaryDto>
{
    private readonly IPortfolioProjectionReadRepository _projectionRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes projection query handlers.</summary>
    public ProjectionSummaryQueryHandler(
        IPortfolioProjectionReadRepository projectionRepository,
        ITenantScope tenantScope)
    {
        _projectionRepository = projectionRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<PortfolioSummaryDto> Handle(
        GetPortfolioSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return await _projectionRepository.GetPortfolioSummaryAsync(tenantId, request.PortfolioId, cancellationToken)
            ?? throw new NotFoundError($"Portfolio '{request.PortfolioId}' not found.");
    }

    /// <inheritdoc />
    public async Task<ProgramSummaryDto> Handle(
        GetProgramSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return await _projectionRepository.GetProgramSummaryAsync(tenantId, request.ProgramId, cancellationToken)
            ?? throw new NotFoundError($"Program '{request.ProgramId}' not found.");
    }

    /// <inheritdoc />
    public async Task<ProjectSummaryDto> Handle(
        GetProjectSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return await _projectionRepository.GetProjectSummaryAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");
    }
}
