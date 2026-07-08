using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// CMD-021 ArchiveProject.
/// Trace: FR-001, AGG-004, EVT-021, ADR-SAD-008.
/// </summary>
public sealed record ArchiveProjectCommand(Guid ProjectId) : IRequest<ProjectDto>;

/// <summary>Handles project archival.</summary>
public sealed class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public ArchiveProjectCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ProjectDto> Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        project.Archive();
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ProjectArchivedIntegrationEvent
        {
            ProjectId = project.Id,
            TenantId = project.TenantId,
            Name = project.Name
        }, cancellationToken);

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
