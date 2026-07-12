# BC-01 Slice 6 — FR-122 Projection Read Models (decision log)

**Status:** Implemented (Slice 6)  
**Trace:** FR-122, CAP-002, CON-006, ADR-SAD-008, ADR-SAD-004, ADR-005

## Catalog / architecture gap

FR-122 requires portfolio aggregates computed **asynchronously** with tenant-scoped indexes. SAD/ADR-SAD-008 place read models in Analytics DU (MOD-23 / DU-5). BC-01 P2-M2 Slice 6 implements an **interim** Core-side approach because a separate Analytics plane is out of scope for this milestone.

## Approved Slice 6 decisions

| Decision | Resolution |
|----------|------------|
| Read-model approach | **Option A** — tenant-scoped live SQL rollups over existing `portfolio_*` tables |
| Not in scope | Separate Analytics BC, DU-5, background projectors, event consumers, projection tables |
| Aggregate shapes | `PortfolioSummary`, `ProgramSummary`, `ProjectSummary` only (fields listed below) |
| HierarchyTree | Out of scope |
| DashboardSummary | Out of scope |
| Staleness | Not implemented — live rollups have no projection lag |
| Events | No new domain/integration events |
| Schema | No new tables; add index `IX_portfolio_projects_TenantId_ProgramId_Status` for rollups |

## Aggregate fields

**PortfolioSummary:** PortfolioId, PortfolioName, ProgramCount, ProjectCount, Draft/Active/OnHold/Completed/Archived project counts  

**ProgramSummary:** ProgramId, ProgramName, PortfolioId, ProjectCount, status counts  

**ProjectSummary:** ProjectId, ProjectName, ProgramId, PortfolioId, OwnerUserId, WorkspaceId, Status, ScopeChangeCount  

## API

- `GET /api/v1/portfolio/{id}/summary`
- `GET /api/v1/programs/{id}/summary`
- `GET /api/v1/projects/{id}/summary`

## Future evolution

Replace live rollups with async materialized projections in Analytics DU (MOD-23) when DU-5 is introduced; then add staleness indicators per SAD query architecture.
