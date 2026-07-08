# Development guide

## Prerequisites

- .NET 9 SDK
- Docker Desktop (for PostgreSQL, Redis, RabbitMQ)

## Bootstrap

```powershell
.\scripts\dev-up.ps1
cd src
dotnet build AIPM.sln
dotnet test AIPM.sln
dotnet run --project AIPM.Host
```

## User secrets (required for local connections)

**Never commit secrets in `appsettings.json`.** Use `dotnet user-secrets` for development:

```powershell
cd src/AIPM.Host

dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=aipm;Username=aipm;Password=aipm_dev"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
dotnet user-secrets set "ConnectionStrings:RabbitMq" "amqp://guest:guest@localhost:5672"
dotnet user-secrets set "ConnectionStrings:IdentityDb" "Host=localhost;Port=5432;Database=aipm_dev;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Security:ApiKey" "dev-local-bc10-key"
```

User secrets ID: `aipm-host-dev` (see `AIPM.Host.csproj`). See also [user-secrets.md](user-secrets.md).

## Milestone progress

See [milestone-progress.md](milestone-progress.md). Latest: **P2-M1 BC-10 Identity & Access Core** — [p2-m1-identity-access-core.md](p2-m1-identity-access-core.md).

## Formatting

```powershell
dotnet format src/AIPM.sln
dotnet format src/AIPM.sln --verify-no-changes
```

CI enforces `--verify-no-changes` before build.

## API versioning

Public HTTP APIs are under `/api/v1/`. Health probes remain unversioned: `/health`, `/ready`.

## Milestone quality gate

Every milestone PR must satisfy [milestone-quality-gate.md](milestone-quality-gate.md).
