using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// FR-005 CloneProject (catalog gap: CMD-023 not listed in Commands-Events-Catalog).
/// Trace: FR-005, CAP-001, CON-008, AGG-004, ADR-SAD-008, ADR-005.
/// </summary>
public sealed record CloneProjectCommand(Guid SourceProjectId, string Name) : IRequest<CloneProjectResponse>;

/// <summary>Handles project cloning.</summary>
public sealed class CloneProjectCommandHandler : IRequestHandler<CloneProjectCommand, CloneProjectResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public CloneProjectCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<CloneProjectResponse> Handle(CloneProjectCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var source = await _projectRepository.FindAsync(tenantId, request.SourceProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.SourceProjectId}' not found.");

        var trimmedName = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ValidationError("Project name is required.");
        }

        var existing = await _projectRepository.ListByTenantAsync(tenantId, cancellationToken);
        if (existing.Any(x => string.Equals(x.Name, trimmedName, StringComparison.Ordinal)))
        {
            throw new ValidationError($"Project name '{trimmedName}' already exists for this tenant.");
        }

        var clone = source.Clone(trimmedName);
        await _projectRepository.AddAsync(clone, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ProjectClonedIntegrationEvent
        {
            ProjectId = clone.Id,
            SourceProjectId = source.Id,
            TenantId = clone.TenantId,
            ProgramId = clone.ProgramId,
            WorkspaceId = clone.WorkspaceId,
            OwnerUserId = clone.OwnerUserId,
            Name = clone.Name
        }, cancellationToken);

        return new CloneProjectResponse(
            source.Id,
            clone.Id,
            clone.TenantId,
            clone.ProgramId,
            clone.WorkspaceId,
            clone.OwnerUserId,
            clone.Name,
            clone.Status.ToString(),
            clone.CreatedAt,
            clone.ArchivedAt);
    }
}
