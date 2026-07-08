using AIPM.Infrastructure.Identity.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Identity.Persistence;

/// <summary>
/// EF Core context for BC-10 identity and access.
/// </summary>
public sealed class IdentityDbContext : DbContext
{
    /// <summary>Initializes db context.</summary>
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    /// <summary>Tenants table.</summary>
    public DbSet<TenantRecord> Tenants => Set<TenantRecord>();
    /// <summary>Users table.</summary>
    public DbSet<UserRecord> Users => Set<UserRecord>();
    /// <summary>Roles table.</summary>
    public DbSet<RoleRecord> Roles => Set<RoleRecord>();
    /// <summary>Role assignments table.</summary>
    public DbSet<RoleAssignmentRecord> RoleAssignments => Set<RoleAssignmentRecord>();
    /// <summary>Permission assignments table.</summary>
    public DbSet<PermissionAssignmentRecord> PermissionAssignments => Set<PermissionAssignmentRecord>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantRecord>(entity =>
        {
            entity.ToTable("identity_tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
        });

        modelBuilder.Entity<UserRecord>(entity =>
        {
            entity.ToTable("identity_users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        });

        modelBuilder.Entity<RoleRecord>(entity =>
        {
            entity.ToTable("identity_roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });

        modelBuilder.Entity<RoleAssignmentRecord>(entity =>
        {
            entity.ToTable("identity_role_assignments");
            entity.HasKey(x => new { x.UserId, x.RoleId });
        });

        modelBuilder.Entity<PermissionAssignmentRecord>(entity =>
        {
            entity.ToTable("identity_permission_assignments");
            entity.HasKey(x => new { x.RoleId, x.PermissionCode });
            entity.Property(x => x.PermissionCode).HasMaxLength(128).IsRequired();
        });
    }
}
