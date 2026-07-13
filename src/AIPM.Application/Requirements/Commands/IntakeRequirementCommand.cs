using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Requirements.Events;
using AIPM.Domain.Portfolio;
using AIPM.Domain.Requirements;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Requirements.Commands;

/// <summary>
/// CMD-030 IntakeRequirement.
/// Trace: FR-010, FR-011, CAP-005, CAP-006, AGG-005, EVT-030, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record IntakeRequirementCommand(
    Guid ProjectId,
    string Title,
    string Statement,
    IReadOnlyList<string>? AcceptanceCriteria = null,
    string? DocumentTitle = null,
    string? DocumentContentType = null,
    string? DocumentUriOrName = null) : IRequest<RequirementDto>;

/// <summary>Handles requirement intake against a BC-01 project.</summary>
public sealed class IntakeRequirementCommandHandler : IRequestHandler<IntakeRequirementCommand, RequirementDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IRequirementRepository _requirementRepository;
    private readonly IRequirementsEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public IntakeRequirementCommandHandler(
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

    /// <inheritdoc />
    public async Task<RequirementDto> Handle(IntakeRequirementCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        if (project.IsArchived || project.Status == ProjectStatus.Archived)
        {
            throw new ValidationError("Cannot intake requirements against an archived project.");
        }

        var documentMetadata = DocumentMetadata.CreateOptional(
            request.DocumentTitle,
            request.DocumentContentType,
            request.DocumentUriOrName);

        var requirement = RequirementAggregate.Intake(
            tenantId,
            request.ProjectId,
            request.Title,
            request.Statement,
            request.AcceptanceCriteria,
            documentMetadata);

        await _requirementRepository.AddAsync(requirement, cancellationToken);
        await _requirementRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new RequirementIntakenIntegrationEvent
        {
            RequirementId = requirement.Id,
            TenantId = requirement.TenantId,
            ProjectId = requirement.ProjectId,
            Title = requirement.Title,
            Status = requirement.Status.ToString(),
            Parsed = requirement.Parsed
        }, cancellationToken);

        return RequirementMapping.ToDto(requirement);
    }
}
