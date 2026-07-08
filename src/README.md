## Build

```bash
cd src
dotnet build AIPM.sln -c Release
dotnet test AIPM.sln -c Release
```

## Run locally

```bash
docker compose -f ../deploy/docker/docker-compose.yml up -d

# Configure secrets once (see docs/development/README.md)
cd AIPM.Host
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=aipm;Username=aipm;Password=aipm_dev"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
dotnet user-secrets set "ConnectionStrings:RabbitMq" "amqp://guest:guest@localhost:5672"
dotnet user-secrets set "ConnectionStrings:IdentityDb" "Host=localhost;Port=5432;Database=aipm_dev;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Security:ApiKey" "dev-local-bc10-key"

cd ..
dotnet run --project AIPM.Host
```

Health: `GET /health` · API: `GET /api/v1/platform/ping` · Identity: `GET /api/v1/identity/tenants` (requires `X-Api-Key`)

## Stack

.NET 9 · ASP.NET Core 9 · PostgreSQL · Redis · RabbitMQ · Serilog · OpenTelemetry

Per ADR-TECH-001.

## Phase status

- Phase 1 (M1–M6): complete
- Phase 2 P2-M1 (BC-10 Identity): complete
- Phase 2 P2-M2+: not started
