# Architecture (implementation)

## Current layout

```
src/
  AIPM.SharedKernel/
  AIPM.Domain/              # BC-10 identity aggregates (P2-M1+)
  AIPM.Application/         # Use cases, ports
  AIPM.Infrastructure/      # EF Core, messaging, adapters
  AIPM.Plugins/
  AIPM.Workflow/
  AIPM.Host/                # Composition root, HTTP API
```

Clean Architecture dependency rule: Domain ← Application ← Infrastructure; Host is composition root.

## Phase status

| Phase | Status |
|-------|--------|
| Phase 1 (M1–M6) | Complete |
| Phase 2 P2-M1 (BC-10) | Complete |
| Phase 2 P2-M2+ | Not started |

## API surface

| Path | Purpose | Auth |
|------|---------|------|
| `GET /health` | Liveness | Public |
| `GET /ready` | Readiness (incl. messaging) | Public |
| `GET /api/v1/platform/ping` | Application probe | Public |
| `GET /api/v1/platform/deployment` | Deployment profile | Public |
| `GET /api/v1/agent-types` | Agent catalog | Public |
| `GET /api/v1/ai/providers` | AI abstraction status | Public |
| `GET /api/v1/identity/*` | BC-10 identity CRUD | API key required |
| `POST /api/v1/identity/*` | BC-10 mutations | API key required |

RFC 7807 Problem Details: [m3-api-hardening.md](../development/m3-api-hardening.md).

BC-10 details: [p2-m1-identity-access-core.md](../development/p2-m1-identity-access-core.md).

## Persistence (P2-M1)

- `IdentityDbContext` in Infrastructure
- Migration: `InitialIdentity`
- Dev: SQLite (`ConnectionStrings:IdentityDb`)
- Production PostgreSQL path: planned (ADR-TECH-001)

## BuildingBlocks roadmap (future)

As the platform grows toward multiple deployable units (ADR-SAD-001), reusable components may consolidate under `src/BuildingBlocks/`. Not refactored in Phase 1 to avoid churn.

See [building-blocks-roadmap.md](building-blocks-roadmap.md).
