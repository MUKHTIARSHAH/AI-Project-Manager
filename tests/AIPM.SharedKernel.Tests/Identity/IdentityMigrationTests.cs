using AIPM.Infrastructure.Identity.Persistence;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Identity;

public sealed class IdentityMigrationTests
{
    [Fact]
    public async Task Migrate_BuildsIdentitySchema()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new IdentityDbContext(options);
        await dbContext.Database.MigrateAsync();

        var tableNames = new List<string>();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table';";
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tableNames.Add(reader.GetString(0));
        }

        tableNames.Should().Contain("identity_tenants");
        tableNames.Should().Contain("identity_users");
        tableNames.Should().Contain("identity_roles");
        tableNames.Should().Contain("identity_role_assignments");
        tableNames.Should().Contain("identity_permission_assignments");
    }
}
