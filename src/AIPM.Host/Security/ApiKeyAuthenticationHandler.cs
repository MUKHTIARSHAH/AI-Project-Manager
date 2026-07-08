using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AIPM.Host.Security;

/// <summary>
/// Simple API key authentication for service-to-service BC-10 endpoints.
/// </summary>
public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>Authentication scheme name.</summary>
    public const string SchemeName = "ApiKey";
    private const string _headerName = "X-Api-Key";
    private readonly IConfiguration _configuration;

    /// <summary>Initializes handler.</summary>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(_headerName, out var submittedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing API key."));
        }

        var configured = _configuration["Security:ApiKey"];
        if (string.IsNullOrWhiteSpace(configured) || !string.Equals(submittedKey.ToString(), configured, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "bc10-service"),
            new Claim(ClaimTypes.Role, "PlatformAdmin")
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
