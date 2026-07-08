using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// CMD-020 CreateProject.
/// Trace: FR-001, AGG-004, EVT-020, ADR-SAD-008.
/// </summary>
public sealed record CreateProjectCommand(
    Guid ProgramId,
    Guid WorkspaceId,
    Guid OwnerUserId,
    string Name) : IRequest<ProjectDto>;

/// <summary>Handles project creation.</summary>
public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProgramRepository _programRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public CreateProjectCommandHandler(
        IProgramRepository programRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _programRepository = programRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var program = await _programRepository.FindAsync(tenantId, request.ProgramId, cancellationToken);
        if (program is null)
        {
            throw new NotFoundError($"Program '{request.ProgramId}' not found.");
        }

        var owner = await _userRepository.FindAsync(request.OwnerUserId, cancellationToken)
            ?? throw new NotFoundError($"Owner user '{request.OwnerUserId}' not found.");
        _tenantScope.EnsureMatches(owner.TenantId);

        var project = ProjectAggregate.Create(
            tenantId,
            request.ProgramId,
            request.WorkspaceId,
            request.OwnerUserId,
            request.Name);

        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ProjectCreatedIntegrationEvent
        {
            ProjectId = project.Id,
            TenantId = project.TenantId,
            ProgramId = project.ProgramId,
            WorkspaceId = project.WorkspaceId,
            OwnerUserId = project.OwnerUserId,
            Name = project.Name
        }, cancellationToken);

        return ToDto(project);
    }

    private static ProjectDto ToDto(ProjectAggregate project)
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
