using Microsoft.EntityFrameworkCore;

namespace AIPM.Infrastructure.Identity.Persistence;

/// <summary>
/// Configures <see cref="IdentityDbContext"/> for PostgreSQL (development/runtime) or SQLite (tests).
/// </summary>
public static class IdentityDatabaseConfiguration
{
    /// <summary>Returns true when the connection string targets PostgreSQL.</summary>
    public static bool IsPostgreSql(string connectionString) =>
        connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
        || connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase);

    /// <summary>Registers the appropriate EF Core provider for the identity store.</summary>
    public static void Configure(DbContextOptionsBuilder options, string connectionString)
    {
        if (IsPostgreSql(connectionString))
        {
            options.UseNpgsql(connectionString);
            return;
        }

        options.UseSqlite(connectionString);
    }
}
