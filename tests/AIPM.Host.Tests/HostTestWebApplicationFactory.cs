using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AIPM.Host.Tests;

/// <summary>
/// Configures isolated test host settings including a fresh identity database per factory.
/// </summary>
public sealed class HostTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _identityDbPath = Path.Combine(Path.GetTempPath(), $"aipm-identity-{Guid.NewGuid():N}.db");
    private readonly string _apiKey = "test-bc10-key";

    /// <summary>Creates factory with default test configuration.</summary>
    public HostTestWebApplicationFactory()
    {
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");
        builder.UseSetting("ConnectionStrings:PostgreSql", "");
        builder.UseSetting("ConnectionStrings:Redis", "inmemory");
        builder.UseSetting("ConnectionStrings:RabbitMq", "inmemory");
        builder.UseSetting("ConnectionStrings:IdentityDb", $"Data Source={_identityDbPath}");
        builder.UseSetting("Security:ApiKey", _apiKey);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_identityDbPath))
        {
            try
            {
                File.Delete(_identityDbPath);
            }
            catch (IOException)
            {
                // Best-effort cleanup; SQLite may still hold the file lock briefly.
            }
        }
    }
}
