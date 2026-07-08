using AIPM.Infrastructure.Identity.Persistence.Models;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
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
    /// <summary>Portfolios table (BC-01 AGG-002).</summary>
    public DbSet<PortfolioRecord> Portfolios => Set<PortfolioRecord>();
    /// <summary>Programs table (BC-01 AGG-003).</summary>
    public DbSet<ProgramRecord> Programs => Set<ProgramRecord>();
    /// <summary>Projects table (BC-01 AGG-004).</summary>
    public DbSet<ProjectRecord> Projects => Set<ProjectRecord>();

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
            entity.HasOne<TenantRecord>()
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RoleRecord>(entity =>
        {
            entity.ToTable("identity_roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            entity.HasOne<TenantRecord>()
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RoleAssignmentRecord>(entity =>
        {
            entity.ToTable("identity_role_assignments");
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.HasOne<UserRecord>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<RoleRecord>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PermissionAssignmentRecord>(entity =>
        {
            entity.ToTable("identity_permission_assignments");
            entity.HasKey(x => new { x.RoleId, x.PermissionCode });
            entity.Property(x => x.PermissionCode).HasMaxLength(128).IsRequired();
            entity.HasOne<RoleRecord>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PortfolioRecord>(entity =>
        {
            entity.ToTable("portfolio_portfolios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            entity.HasOne<TenantRecord>()
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProgramRecord>(entity =>
        {
            entity.ToTable("portfolio_programs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.PortfolioId, x.Name }).IsUnique();
            entity.HasOne<TenantRecord>()
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<PortfolioRecord>()
                .WithMany()
                .HasForeignKey(x => x.PortfolioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectRecord>(entity =>
        {
            entity.ToTable("portfolio_projects");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            entity.HasOne<TenantRecord>()
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ProgramRecord>()
                .WithMany()
                .HasForeignKey(x => x.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
