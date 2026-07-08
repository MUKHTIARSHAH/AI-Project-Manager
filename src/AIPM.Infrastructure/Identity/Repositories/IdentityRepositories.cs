using AIPM.Application.Identity;
using AIPM.Domain.Identity;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Identity.Repositories;

/// <summary>Tenant EF repository.</summary>
public sealed class TenantRepository : ITenantRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public TenantRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _dbContext.Tenants.AddAsync(new TenantRecord
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Status = tenant.Status.ToString()
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Tenant?> FindAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
        return record is null
            ? null
            : Tenant.Rehydrate(record.Id, record.Name, Enum.Parse<TenantStatus>(record.Status, true));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Tenant>> ListAsync(CancellationToken cancellationToken = default)
        => (await _dbContext.Tenants.AsNoTracking().ToListAsync(cancellationToken))
            .Select(x => Tenant.Rehydrate(x.Id, x.Name, Enum.Parse<TenantStatus>(x.Status, true)))
            .ToList();

    /// <inheritdoc />
    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Tenants.FirstOrDefaultAsync(x => x.Id == tenant.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Tenant '{tenant.Id}' was not found.");
        record.Name = tenant.Name;
        record.Status = tenant.Status.ToString();
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}

/// <summary>User EF repository.</summary>
public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public UserRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(new UserRecord
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Email = user.Email
        }, cancellationToken);

        foreach (var assignment in user.RoleAssignments)
        {
            await _dbContext.RoleAssignments.AddAsync(new RoleAssignmentRecord
            {
                UserId = assignment.UserId,
                RoleId = assignment.RoleId,
                AssignedAt = assignment.AssignedAt
            }, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (record is null)
        {
            return null;
        }

        var assignments = await _dbContext.RoleAssignments.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new RoleAssignment(x.UserId, x.RoleId, x.AssignedAt))
            .ToListAsync(cancellationToken);

        return User.Rehydrate(record.Id, record.TenantId, record.Email, assignments);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        var assignments = await _dbContext.RoleAssignments.AsNoTracking().ToListAsync(cancellationToken);
        return users.Select(x =>
            User.Rehydrate(
                x.Id,
                x.TenantId,
                x.Email,
                assignments.Where(y => y.UserId == x.Id)
                    .Select(y => new RoleAssignment(y.UserId, y.RoleId, y.AssignedAt))
                    .ToList())).ToList();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken)
            ?? throw new InvalidOperationException($"User '{user.Id}' was not found.");
        record.Email = user.Email;
        var roleRows = await _dbContext.RoleAssignments.Where(x => x.UserId == user.Id).ToListAsync(cancellationToken);
        _dbContext.RoleAssignments.RemoveRange(roleRows);
        foreach (var assignment in user.RoleAssignments)
        {
            await _dbContext.RoleAssignments.AddAsync(new RoleAssignmentRecord
            {
                UserId = assignment.UserId,
                RoleId = assignment.RoleId,
                AssignedAt = assignment.AssignedAt
            }, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}

/// <summary>Role EF repository.</summary>
public sealed class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _dbContext;

    /// <summary>Initializes repository.</summary>
    public RoleRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _dbContext.Roles.AddAsync(new RoleRecord
        {
            Id = role.Id,
            TenantId = role.TenantId,
            Name = role.Name,
            FailClosed = role.IdentityPolicy.FailClosed
        }, cancellationToken);

        foreach (var assignment in role.PermissionAssignments)
        {
            await _dbContext.PermissionAssignments.AddAsync(new PermissionAssignmentRecord
            {
                RoleId = assignment.RoleId,
                PermissionCode = assignment.Permission.Code,
                AssignedAt = assignment.AssignedAt
            }, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<Role?> FindAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
        if (record is null)
        {
            return null;
        }

        var permissions = await _dbContext.PermissionAssignments.AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => new PermissionAssignment(x.RoleId, new Permission(x.PermissionCode), x.AssignedAt))
            .ToListAsync(cancellationToken);

        return Role.Rehydrate(record.Id, record.TenantId, record.Name, new IdentityPolicy(record.FailClosed), permissions);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _dbContext.Roles.AsNoTracking().ToListAsync(cancellationToken);
        var permissions = await _dbContext.PermissionAssignments.AsNoTracking().ToListAsync(cancellationToken);

        return roles.Select(x =>
            Role.Rehydrate(
                x.Id,
                x.TenantId,
                x.Name,
                new IdentityPolicy(x.FailClosed),
                permissions.Where(y => y.RoleId == x.Id)
                    .Select(y => new PermissionAssignment(y.RoleId, new Permission(y.PermissionCode), y.AssignedAt))
                    .ToList())).ToList();
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == role.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Role '{role.Id}' was not found.");
        record.Name = role.Name;
        record.FailClosed = role.IdentityPolicy.FailClosed;
        var permissionRows = await _dbContext.PermissionAssignments.Where(x => x.RoleId == role.Id).ToListAsync(cancellationToken);
        _dbContext.PermissionAssignments.RemoveRange(permissionRows);
        foreach (var assignment in role.PermissionAssignments)
        {
            await _dbContext.PermissionAssignments.AddAsync(new PermissionAssignmentRecord
            {
                RoleId = assignment.RoleId,
                PermissionCode = assignment.Permission.Code,
                AssignedAt = assignment.AssignedAt
            }, cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
}
