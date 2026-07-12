# BC-01 Dependency Map

## Layer flow

```text
AIPM.Host
    │  MediatR send, endpoint mapping, auth, ProblemDetails
    ▼
AIPM.Application  (Portfolio commands/queries/DTOs/interfaces/events)
    │  depends on Domain + SharedKernel only (no Infrastructure)
    ▼
AIPM.Domain       (Portfolio aggregates / entities / domain events)
    │
    ▼
AIPM.SharedKernel (primitives, errors, execution context)

AIPM.Infrastructure
    │  implements Application Portfolio interfaces
    │  EF Core IdentityDbContext + portfolio_* tables
    │  MassTransit publish for IPortfolioEventPublisher
    ▼
PostgreSQL / message bus
```

## Confirmed rules

| Rule | Status |
|------|--------|
| Domain does not reference Application/Infrastructure/Host | Enforced by architecture tests |
| Application does not reference Infrastructure/Host | Enforced |
| Host references Application (+ Infrastructure DI composition) | Expected |
| Infrastructure references Application + Domain | Expected for adapters |
| No circular project references | Verified in solution graph |

## Composition root

`AIPM.Host` → `AddInfrastructure(...)` registers:

- `IPortfolioRepository` → `PortfolioRepository`
- `IProgramRepository` → `ProgramRepository`
- `IProjectRepository` → `ProjectRepository`
- `IPortfolioProjectionReadRepository` → `PortfolioProjectionReadRepository`
- `IPortfolioEventPublisher` → `PortfolioEventPublisher`

MediatR handlers are discovered from the Application assembly (existing host registration).

## Architecture test suite

`AIPM.Architecture.Tests` — 7 tests validating Clean Architecture dependency constraints (passing at freeze).
