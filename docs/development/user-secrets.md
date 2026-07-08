# User secrets (local development)

The host project uses **.NET user secrets** so connection strings and API keys are never committed.

| Setting | Value |
|---------|-------|
| Project | `src/AIPM.Host/AIPM.Host.csproj` |
| User secrets ID | `aipm-host-dev` |

## Initialize (once per machine)

```powershell
cd src/AIPM.Host
dotnet user-secrets init
```

The `UserSecretsId` is already set in the project file.

## Set secrets

```powershell
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=aipm;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
dotnet user-secrets set "ConnectionStrings:RabbitMq" "amqp://guest:guest@localhost:5672"
dotnet user-secrets set "ConnectionStrings:IdentityDb" "Host=localhost;Port=5432;Database=aipm_dev;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Security:ApiKey" "dev-local-bc10-key"
dotnet user-secrets set "OpenTelemetry:OtlpEndpoint" "http://localhost:4317"
```

## List secrets

```powershell
dotnet user-secrets list --project src/AIPM.Host
```

## Rules

- **Never** commit secrets to `appsettings.json`, `appsettings.Development.json`, or Blueprint documents.
- CI uses GitHub Actions secrets; production uses Vault/KMS (ADR-SAD-010).
- Empty connection strings in committed config mean "use in-memory / skip health check" for local skeleton mode.
