using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Requirements.Events;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Requirements.Commands;

/// <summary>
/// CMD-031 ApproveRequirement.
/// Trace: FR-011, CAP-006, AGG-005, CON-013, EVT-031, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record ApproveRequirementCommand(Guid RequirementId) : IRequest<RequirementDto>;

/// <summary>Handles requirement approval against a BC-01 project.</summary>
public sealed class ApproveRequirementCommandHandler : IRequestHandler<ApproveRequirementCommand, RequirementDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IRequirementRepository _requirementRepository;
    private readonly IRequirementsEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    public ApproveRequirementCommandHandler(
        IProjectRepository projectRepository,
        IRequirementRepository requirementRepository,
        IRequirementsEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _requirementRepository = requirementRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    public async Task<RequirementDto> Handle(ApproveRequirementCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var requirement = await _requirementRepository.FindAsync(tenantId, request.RequirementId, cancellationToken)
            ?? throw new NotFoundError($"Requirement '{request.RequirementId}' not found.");

        var project = await _projectRepository.FindAsync(tenantId, requirement.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{requirement.ProjectId}' not found.");

        if (project.IsArchived || project.Status == ProjectStatus.Archived)
        {
            throw new ValidationError("Cannot approve requirements for an archived project.");
        }

        var approved = requirement.Approve();

        await _requirementRepository.UpdateAsync(approved, cancellationToken);
        await _requirementRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new RequirementApprovedIntegrationEvent
        {
            RequirementId = approved.Id,
            TenantId = approved.TenantId,
            ProjectId = approved.ProjectId,
            Title = approved.Title,
            Status = approved.Status.ToString(),
            Parsed = approved.Parsed
        }, cancellationToken);

        return RequirementMapping.ToDto(approved);
    }
}
