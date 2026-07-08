namespace AIPM.Application.Identity;

/// <summary>Tenant DTO.</summary>
public sealed record TenantDto(Guid Id, string Name, string Status);

/// <summary>User DTO.</summary>
public sealed record UserDto(Guid Id, Guid TenantId, string Email, IReadOnlyList<Guid> RoleIds);

/// <summary>Role DTO.</summary>
public sealed record RoleDto(Guid Id, Guid TenantId, string Name, IReadOnlyList<string> Permissions);
