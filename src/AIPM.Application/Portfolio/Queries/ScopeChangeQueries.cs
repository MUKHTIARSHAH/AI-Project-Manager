using AIPM.Application.Identity;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Queries;

/// <summary>Lists scope changes for a tenant-scoped project.</summary>
public sealed record ListScopeChangesQuery(Guid ProjectId) : IRequest<IReadOnlyList<ScopeChangeDto>>;

/// <summary>Gets one scope change on a tenant-scoped project.</summary>
public sealed record GetScopeChangeQuery(Guid ProjectId, Guid ScopeChangeId) : IRequest<ScopeChangeDto>;

/// <summary>
/// BC-01 scope change query handlers.
/// Trace: FR-004, CAP-004, CON-011, AGG-004, ADR-SAD-008.
/// </summary>
public sealed class ScopeChangeQueryHandler :
    IRequestHandler<ListScopeChangesQuery, IReadOnlyList<ScopeChangeDto>>,
    IRequestHandler<GetScopeChangeQuery, ScopeChangeDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes query handlers.</summary>
    public ScopeChangeQueryHandler(IProjectRepository projectRepository, ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ScopeChangeDto>> Handle(
        ListScopeChangesQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        return project.ScopeChanges
            .OrderBy(x => x.RecordedAt)
            .Select(ToDto)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ScopeChangeDto> Handle(GetScopeChangeQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var scopeChange = project.ScopeChanges.FirstOrDefault(x => x.Id == request.ScopeChangeId)
            ?? throw new NotFoundError(
                $"Scope change '{request.ScopeChangeId}' not found on project '{request.ProjectId}'.");

        return ToDto(scopeChange);
    }

    private static ScopeChangeDto ToDto(ScopeChange scopeChange)
        => new(
            scopeChange.Id,
            scopeChange.ProjectId,
            scopeChange.TenantId,
            scopeChange.Title,
            scopeChange.Description,
            scopeChange.AffectedRequirementCitation,
            scopeChange.Status.ToString(),
            scopeChange.RecordedAt,
            scopeChange.DecidedAt);
}
