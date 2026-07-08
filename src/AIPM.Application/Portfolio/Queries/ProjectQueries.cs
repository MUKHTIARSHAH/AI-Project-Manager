using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Queries;

/// <summary>Lists tenant projects.</summary>
public sealed record ListProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;

/// <summary>Gets one tenant project.</summary>
public sealed record GetProjectQuery(Guid ProjectId) : IRequest<ProjectDto>;

/// <summary>
/// BC-01 project query handlers.
/// Trace: FR-001, AGG-004, ADR-SAD-008.
/// </summary>
public sealed class ProjectQueryHandler :
    IRequestHandler<ListProjectsQuery, IReadOnlyList<ProjectDto>>,
    IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes query handlers.</summary>
    public ProjectQueryHandler(IProjectRepository projectRepository, ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProjectDto>> Handle(ListProjectsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return (await _projectRepository.ListByTenantAsync(tenantId, cancellationToken))
            .Select(ToDto)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ProjectDto> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        return ToDto(project);
    }

    private static ProjectDto ToDto(Domain.Portfolio.ProjectAggregate project)
        => new(
            project.Id,
            project.TenantId,
            project.ProgramId,
            project.WorkspaceId,
            project.OwnerUserId,
            project.Name,
            project.Status.ToString(),
            project.CreatedAt,
            project.ArchivedAt);
}
