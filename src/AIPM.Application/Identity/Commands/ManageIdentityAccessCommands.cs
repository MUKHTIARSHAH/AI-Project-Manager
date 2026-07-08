using AIPM.Domain.Identity;
using MediatR;

namespace AIPM.Application.Identity.Commands;

/// <summary>Creates a user in a tenant.</summary>
public sealed record CreateUserCommand(Guid TenantId, string Email) : IRequest<UserDto>;

/// <summary>Creates a role in a tenant.</summary>
public sealed record CreateRoleCommand(Guid TenantId, string Name) : IRequest<RoleDto>;

/// <summary>Assigns role to a user.</summary>
public sealed record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest;

/// <summary>Assigns permission to a role.</summary>
public sealed record AssignPermissionCommand(Guid RoleId, string PermissionCode) : IRequest;

/// <summary>Handles user and role mutations.</summary>
public sealed class ManageIdentityAccessCommandsHandler :
    IRequestHandler<CreateUserCommand, UserDto>,
    IRequestHandler<CreateRoleCommand, RoleDto>,
    IRequestHandler<AssignRoleCommand>,
    IRequestHandler<AssignPermissionCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public ManageIdentityAccessCommandsHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITenantRepository tenantRepository,
        ITenantScope tenantScope)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tenantRepository = tenantRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _tenantScope.EnsureMatches(request.TenantId);
        var tenant = await _tenantRepository.FindAsync(request.TenantId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"Tenant '{request.TenantId}' not found.");

        if (tenant.Status != TenantStatus.Active)
        {
            throw new AIPM.SharedKernel.Errors.ConflictError("User cannot be added to a suspended tenant.");
        }

        var user = User.Create(request.TenantId, request.Email);
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return new UserDto(user.Id, user.TenantId, user.Email, user.RoleAssignments.Select(x => x.RoleId).ToList());
    }

    /// <inheritdoc />
    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        _tenantScope.EnsureMatches(request.TenantId);
        var tenant = await _tenantRepository.FindAsync(request.TenantId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"Tenant '{request.TenantId}' not found.");

        if (tenant.Status != TenantStatus.Active)
        {
            throw new AIPM.SharedKernel.Errors.ConflictError("Role cannot be added to a suspended tenant.");
        }

        var role = Role.Create(request.TenantId, request.Name);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);
        return new RoleDto(role.Id, role.TenantId, role.Name, role.PermissionAssignments.Select(x => x.Permission.Code).ToList());
    }

    /// <inheritdoc />
    public async Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var scopedTenantId = _tenantScope.GetRequiredTenantId().Value;

        var user = await _userRepository.FindAsync(request.UserId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"User '{request.UserId}' not found.");
        var role = await _roleRepository.FindAsync(request.RoleId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"Role '{request.RoleId}' not found.");

        if (user.TenantId != role.TenantId)
        {
            throw new AIPM.SharedKernel.Errors.ForbiddenError("Cross-tenant role assignment is forbidden.");
        }

        if (user.TenantId != scopedTenantId)
        {
            throw new AIPM.SharedKernel.Errors.ForbiddenError("Cross-tenant access is forbidden.");
        }

        user.AssignRole(request.RoleId);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
    {
        var scopedTenantId = _tenantScope.GetRequiredTenantId().Value;

        var role = await _roleRepository.FindAsync(request.RoleId, cancellationToken)
            ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"Role '{request.RoleId}' not found.");

        if (role.TenantId != scopedTenantId)
        {
            throw new AIPM.SharedKernel.Errors.ForbiddenError("Cross-tenant access is forbidden.");
        }

        role.AssignPermission(new Permission(request.PermissionCode));
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);
    }
}
