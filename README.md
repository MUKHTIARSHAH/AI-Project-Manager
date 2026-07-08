# AI Project Manager (AIPM) — Control Plane

Enterprise AI project management control plane implementing the approved Engineering Blueprint (ADR-TECH-001: .NET 9, PostgreSQL, RabbitMQ, Redis).

## Current status

| Phase | Milestone | Status |
|-------|-----------|--------|
| **Phase 1** | M1–M6 Platform infrastructure | **Complete** |
| **Phase 2** | P2-M1 BC-10 Identity & Access Core | **Complete** |
| **Phase 2** | P2-M2+ | Not started |

**Latest capability:** BC-10 identity bounded context — tenant, user, role, and permission management with EF Core persistence, fail-closed API authorization, and domain event publishing.

See [docs/development/p2-m1-identity-access-core.md](docs/development/p2-m1-identity-access-core.md) for P2-M1 details.

## Quick start

```powershell
.\scripts\dev-up.ps1
cd src
dotnet build AIPM.sln -c Release
dotnet test AIPM.sln -c Release

cd AIPM.Host
dotnet user-secrets set "Security:ApiKey" "dev-local-bc10-key"
dotnet user-secrets set "ConnectionStrings:IdentityDb" "Data Source=aipm.identity.db"

cd ..
dotnet run --project AIPM.Host
```

- Health: `GET /health`, `GET /ready`
- Platform: `GET /api/v1/platform/ping`
- Identity (requires `X-Api-Key` header): `GET /api/v1/identity/tenants`

Full setup: [docs/development/README.md](docs/development/README.md)

## Repository layout

| Path | Purpose |
|------|---------|
| `src/` | .NET solution (Clean Architecture) |
| `tests/` | Unit, integration, architecture tests |
| `docs/` | Implementation documentation |
| `deploy/` | Docker Compose, deployment profiles |
| `Engineering-Blueprint/` | Locked specifications (SRS, SAD, EIM, DM, ADRs) |
| `apps/admin-console/` | Minimal Next.js admin shell |

## Quality gates

Every milestone must pass:

```powershell
dotnet build src/AIPM.sln -c Release
dotnet test src/AIPM.sln -c Release
dotnet format src/AIPM.sln --verify-no-changes
```

See [docs/development/milestone-quality-gate.md](docs/development/milestone-quality-gate.md).

## Authority

- **Blueprint (locked):** `Engineering-Blueprint/` — do not modify without ADR.
- **Implementation:** this repository — derived from blueprint, traced via milestone docs.

## Changelog

[CHANGELOG.md](CHANGELOG.md)
