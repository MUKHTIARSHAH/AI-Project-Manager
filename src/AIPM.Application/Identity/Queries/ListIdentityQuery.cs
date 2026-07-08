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

    /// <summary>Initializes handler.</summary>
    public ListIdentityQueryHandler(ITenantRepository tenantRepository, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TenantDto>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
        => (await _tenantRepository.ListAsync(cancellationToken))
            .Select(x => new TenantDto(x.Id, x.Name, x.Status.ToString()))
            .ToList();

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        => (await _userRepository.ListAsync(cancellationToken))
            .Select(x => new UserDto(x.Id, x.TenantId, x.Email, x.RoleAssignments.Select(r => r.RoleId).ToList()))
            .ToList();

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoleDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
        => (await _roleRepository.ListAsync(cancellationToken))
            .Select(x => new RoleDto(x.Id, x.TenantId, x.Name, x.PermissionAssignments.Select(p => p.Permission.Code).ToList()))
            .ToList();
}
