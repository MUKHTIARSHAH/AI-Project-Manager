using MediatR;

namespace AIPM.Application.Identity.Queries;

/// <summary>Lists tenants.</summary>
public sealed record ListTenantsQuery : IRequest<IReadOnlyList<TenantDto>>;

/// <summary>Lists users.</summary>
public sealed record ListUsersQuery : IRequest<IReadOnlyList<UserDto>>;

/// <summary>Lists roles.</summary>
public sealed record ListRolesQuery : IRequest<IReadOnlyList<RoleDto>>;

/// <summary>List handlers for BC-10 resources.</summary>
public sealed class ListIdentityQueryHandler :
    IRequestHandler<ListTenantsQuery, IReadOnlyList<TenantDto>>,
    IRequestHandler<ListUsersQuery, IReadOnlyList<UserDto>>,
    IRequestHandler<ListRolesQuery, IReadOnlyList<RoleDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public ListIdentityQueryHandler(
        ITenantRepository tenantRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITenantScope tenantScope)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantDto>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = _tenantScope.CurrentTenantId is { } tenantId
            ? await _tenantRepository.ListByTenantAsync(tenantId.Value, cancellationToken)
            : await _tenantRepository.ListAsync(cancellationToken);

        return tenants
            .Select(x => new TenantDto(x.Id, x.Name, x.Status.ToString()))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return (await _userRepository.ListByTenantAsync(tenantId, cancellationToken))
            .Select(x => new UserDto(x.Id, x.TenantId, x.Email, x.RoleAssignments.Select(r => r.RoleId).ToList()))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoleDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return (await _roleRepository.ListByTenantAsync(tenantId, cancellationToken))
            .Select(x => new RoleDto(x.Id, x.TenantId, x.Name, x.PermissionAssignments.Select(p => p.Permission.Code).ToList()))
            .ToList();
    }
}
