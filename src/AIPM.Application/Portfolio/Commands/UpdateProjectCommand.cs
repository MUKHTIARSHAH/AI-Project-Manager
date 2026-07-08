using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// FR-001 UpdateProject (CRUD update path).
/// Trace: AGG-004, CAP-001, CON-008, ADR-SAD-008.
/// </summary>
public sealed record UpdateProjectCommand(
    Guid ProjectId,
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name) : IRequest<ProjectDto>;

/// <summary>Handles project updates.</summary>
public sealed class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var owner = await _userRepository.FindAsync(request.OwnerUserId, cancellationToken)
            ?? throw new NotFoundError($"Owner user '{request.OwnerUserId}' not found.");
        _tenantScope.EnsureMatches(owner.TenantId);

        project.Update(request.Name, request.OwnerUserId, request.WorkspaceId);
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return new ProjectDto(
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
}
