using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AIPM.Infrastructure.Identity.Persistence;

/// <summary>
/// Design-time factory for EF Core migrations (PostgreSQL).
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    /// <inheritdoc />
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("AIPM_MIGRATION_CONNECTION")
            ?? "Host=127.0.0.1;Port=5432;Database=aipm_dev;Username=postgres;Password=REPLACE_ME";

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        IdentityDatabaseConfiguration.Configure(optionsBuilder, connection);
        return new IdentityDbContext(optionsBuilder.Options);
    }
}
