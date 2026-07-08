using AIPM.Application.Identity.Events;
using AIPM.Domain.Identity;
using MediatR;

namespace AIPM.Application.Identity.Commands;

/// <summary>CMD-001 ProvisionTenant command.</summary>
public sealed record ProvisionTenantCommand(string Name) : IRequest<TenantDto>;

/// <summary>Handles tenant provisioning.</summary>
public sealed class ProvisionTenantCommandHandler : IRequestHandler<ProvisionTenantCommand, TenantDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IIdentityEventPublisher _eventPublisher;

    /// <summary>Initializes handler.</summary>
    public ProvisionTenantCommandHandler(ITenantRepository tenantRepository, IIdentityEventPublisher eventPublisher)
    {
        _tenantRepository = tenantRepository;
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task<TenantDto> Handle(ProvisionTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = Tenant.Provision(request.Name);
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new TenantProvisionedIntegrationEvent
        {
            TenantId = tenant.Id,
            Name = tenant.Name
        }, cancellationToken);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Status.ToString());
    }
}

/// <summary>CMD-002 SuspendTenant command.</summary>
public sealed record SuspendTenantCommand(Guid TenantId) : IRequest;

/// <summary>Handles tenant suspension.</summary>
public sealed class SuspendTenantCommandHandler : IRequestHandler<SuspendTenantCommand>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IIdentityEventPublisher _eventPublisher;

    /// <summary>Initializes handler.</summary>
    public SuspendTenantCommandHandler(ITenantRepository tenantRepository, IIdentityEventPublisher eventPublisher)
    {
        _tenantRepository = tenantRepository;
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task Handle(SuspendTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.FindAsync(request.TenantId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"Tenant '{request.TenantId}' not found.");

        tenant.Suspend();
        await _tenantRepository.UpdateAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);
        await _eventPublisher.PublishAsync(new TenantSuspendedIntegrationEvent
        {
            TenantId = tenant.Id,
            Name = tenant.Name
        }, cancellationToken);
    }
}
