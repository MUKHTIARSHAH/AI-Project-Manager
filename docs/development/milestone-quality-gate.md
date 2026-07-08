# Milestone quality gate

Every milestone (M1, M2, …) must satisfy **all** checks before merge.

| Check | Command / criterion |
|-------|---------------------|
| Builds successfully | `dotnet build src/AIPM.sln -c Release` |
| All tests pass | `dotnet test src/AIPM.sln -c Release` |
| Architecture tests pass | Included in test suite (`AIPM.Architecture.Tests`) |
| No analyzer warnings in CI | `TreatWarningsAsErrors` when `CI=true` |
| No circular dependencies | NetArchTest rules |
| Formatting clean | `dotnet format src/AIPM.sln --verify-no-changes` |
| Docker starts | `docker compose -f deploy/docker/docker-compose.yml up -d` |
| Health endpoints | `GET /health` → 200, `GET /ready` → 200 (with infra running) |
| Public APIs have XML documentation | All new public types documented |
| Coverage does not decrease | Compare OpenCover artifact vs prior baseline |
| ADRs respected | No new aggregates; ADR-TECH-001 runtime; ADR-GOV-007 |

**No business logic** in platform milestones unless explicitly scoped in Phase 2+.
