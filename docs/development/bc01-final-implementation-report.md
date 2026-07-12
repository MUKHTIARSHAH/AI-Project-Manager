# BC-01 Final Implementation Report

**Bounded context:** BC-01 — Project Portfolio  
**Milestone:** P2-M2  
**Status:** COMPLETE — release freeze  
**HEAD:** `2ef3a00` (`feat(bc-01): implement Projection Read Models (Slice 6)`)  
**Date:** 2026-07-13  
**Tests:** 137 passing  

---

## 1. Executive summary

BC-01 delivers the first business delivery bounded context for AIPM: portfolio hierarchy, project lifecycle primitives (create/update/archive/clone), scope-change tracking, and interim FR-122 projection summaries. Implementation follows Clean Architecture, MediatR CQRS, tenant-scoped PostgreSQL persistence, and MassTransit-backed integration event publishing.

**Slices delivered**

| Slice | Capability | Primary FR | Commit |
|-------|------------|------------|--------|
| 1 | Portfolio | FR-003 | `fa45bc5` (with 2–3) |
| 2 | Program | FR-003 | `fa45bc5` |
| 3 | Project CRUD + Archive | FR-001 | `fa45bc5` |
| 4 | Scope Change | FR-004 | `e546d13` |
| 5 | Project Cloning | FR-005 | `5ea750e` |
| 6 | Projection Read Models | FR-122 | `2ef3a00` |

**Release-freeze verification (read-only):** migration chain coherent; no duplicate endpoints; no Portfolio TODO/FIXME; all repository interfaces DI-registered; architecture tests green; working tree clean on `origin/main`.

---

## 2. Blueprint traceability

| Layer | IDs |
|-------|-----|
| **FR** | FR-001, FR-003, FR-004, FR-005, FR-122 |
| **CAP** | CAP-001, CAP-002, CAP-004 |
| **CON** | CON-006 Portfolio, CON-007 Program, CON-008 Project, CON-011 ScopeChange; Workspace as reference (CON-003) |
| **AGG** | AGG-002 Portfolio, AGG-003 Program, AGG-004 Project |
| **CMD** | CMD-010, CMD-011, CMD-020, CMD-021, CMD-022; CloneProject (catalog gap ≈ CMD-023) |
| **EVT** | EVT-010, EVT-011, EVT-020, EVT-021; ScopeChange* / ProjectCloned (catalog gaps) |
| **ADR** | ADR-005 (tenant), ADR-SAD-004 (events), ADR-SAD-008 (CQRS), ADR-003 (scope), ADR-GOV-007 (no new AGG) |

**Not implemented in BC-01 (explicit deferrals):** FR-002 Delivery Templates (CAP-003), full Analytics DU-5 / MOD-23 async projectors, HierarchyTree, DashboardSummary, Draft→Active lifecycle command.

Supporting notes:

- `docs/development/bc01-slice4-fr004-scope-change-catalog-gap.md`
- `docs/development/bc01-slice5-fr005-project-cloning-catalog-gap.md`
- `docs/development/bc01-slice6-fr122-projection-read-models.md`

---

## 3. Domain model implemented

| Type | Name | Notes |
|------|------|-------|
| Aggregate | `PortfolioAggregate` | Create; unique name per tenant |
| Aggregate | `ProgramAggregate` | Create under Portfolio |
| Aggregate | `ProjectAggregate` | Create, Update, Archive, Clone, ScopeChange lifecycle |
| Entity | `ScopeChange` | Owned by Project; Proposed→Approved/Rejected→Implemented |
| Enum | `ProjectStatus` | Draft, Active, OnHold, Completed, Archived |
| Enum | `ScopeChangeStatus` | Proposed, Approved, Rejected, Implemented |

**Value objects:** No dedicated Portfolio VOs; identifiers are `Guid`. SharedKernel `TenantId` used at application boundary via `ITenantScope`.

---

## 4. CQRS commands

| Command | Catalog | Trace |
|---------|---------|-------|
| `CreatePortfolioCommand` | CMD-010 | FR-003 |
| `CreateProgramCommand` | CMD-011 | FR-003 |
| `CreateProjectCommand` | CMD-020 | FR-001 |
| `UpdateProjectCommand` | CRUD path | FR-001 |
| `ArchiveProjectCommand` | CMD-021 | FR-001 |
| `CloneProjectCommand` | gap ≈ CMD-023 | FR-005 |
| `RecordScopeChangeCommand` | CMD-022 | FR-004 |
| `ApproveScopeChangeCommand` | FR-004 trail | FR-004 |
| `RejectScopeChangeCommand` | FR-004 trail | FR-004 |
| `ImplementScopeChangeCommand` | CON-011 | FR-004 |

---

## 5. Queries

| Query | Purpose | FR |
|-------|---------|-----|
| `ListPortfoliosQuery` / `GetPortfolioQuery` | Strong-consistency CRUD reads | FR-003 |
| `ListProgramsQuery` / `GetProgramQuery` | CRUD reads | FR-003 |
| `ListProjectsQuery` / `GetProjectQuery` | CRUD reads | FR-001 |
| `ListScopeChangesQuery` / `GetScopeChangeQuery` | Scope trail reads | FR-004 |
| `GetPortfolioSummaryQuery` | Live rollup | FR-122 |
| `GetProgramSummaryQuery` | Live rollup | FR-122 |
| `GetProjectSummaryQuery` | Live rollup + scope count | FR-122 |

---

## 6. Domain events

| Event | Raised by |
|-------|-----------|
| `PortfolioCreatedDomainEvent` | Portfolio.Create |
| `ProgramCreatedDomainEvent` | Program.Create |
| `ProjectCreatedDomainEvent` | Project.Create |
| `ProjectArchivedDomainEvent` | Project.Archive |
| `ProjectClonedDomainEvent` | Project.Clone (on clone only; not ProjectCreated) |
| `ScopeChangeRecordedDomainEvent` | RecordScopeChange |
| `ScopeChangeApprovedDomainEvent` | ApproveScopeChange |
| `ScopeChangeRejectedDomainEvent` | RejectScopeChange |
| `ScopeChangeImplementedDomainEvent` | MarkScopeChangeImplemented |

---

## 7. Integration events

Published via `IPortfolioEventPublisher` / MassTransit:

- `PortfolioCreatedIntegrationEvent`
- `ProgramCreatedIntegrationEvent`
- `ProjectCreatedIntegrationEvent`
- `ProjectArchivedIntegrationEvent`
- `ProjectClonedIntegrationEvent`
- `ScopeChangeRecordedIntegrationEvent`
- `ScopeChangeApprovedIntegrationEvent`
- `ScopeChangeRejectedIntegrationEvent`
- `ScopeChangeImplementedIntegrationEvent`

---

## 8. API endpoints

See [bc01-api-inventory.md](bc01-api-inventory.md). All BC-01 groups use `Bc10Admin` + `X-Tenant-Id` (via `ITenantScope.GetRequiredTenantId()`).

---

## 9. PostgreSQL schema

| Table | Purpose |
|-------|---------|
| `portfolio_portfolios` | AGG-002 |
| `portfolio_programs` | AGG-003 |
| `portfolio_projects` | AGG-004 |
| `portfolio_scope_changes` | CON-011 owned collection |

Key constraints: unique `(TenantId, Name)` on portfolios/projects; unique `(TenantId, PortfolioId, Name)` on programs; FKs to `identity_tenants` and hierarchy parents; scope changes cascade from project.

---

## 10. Migration history

| Order | Migration | Change |
|-------|-----------|--------|
| 1 | `InitialIdentity` | Identity tables (BC-10 prerequisite) |
| 2 | `AddPortfolioCore` | `portfolio_portfolios` |
| 3 | `AddProgramCore` | `portfolio_programs` |
| 4 | `AddProjectCore` | `portfolio_projects` |
| 5 | `AddScopeChangeCore` | `portfolio_scope_changes` |
| 6 | `AddProjectionReadIndexes` | `IX_portfolio_projects_TenantId_ProgramId_Status` |

---

## 11. Security / tenant isolation

- Authorization: `RequireAuthorization("Bc10Admin")` on `/portfolio`, `/programs`, `/projects`.
- Tenant: `X-Tenant-Id` required; handlers call `GetRequiredTenantId()`; repositories filter by `tenantId`.
- Cross-tenant access returns NotFound (not Forbidden) for resource lookups.
- Owner user on create/update validated with `EnsureMatches(owner.TenantId)`.

---

## 12. Testing summary

| Area | Coverage |
|------|----------|
| Domain | Portfolio/Program/Project invariants, scope lifecycle, clone rules |
| Handlers | Commands, clone, scope, projections, tenant failures |
| Repositories | Project persist/rehydrate/scope; projection rollups |
| Host | CRUD, archive, clone, scope, summaries, tenant isolation |
| Architecture | Clean Architecture dependency rules (7 tests) |
| **Total** | **137** |

---

## 13. Engineering decisions

| Decision | Rationale |
|----------|-----------|
| Program required under Portfolio | Canonical `Portfolio → Program → Project` (P2-M2) |
| ScopeChange entity on AGG-004 | ADR-GOV-007 — no new aggregate |
| Clone requires explicit name | Tenant unique names; no auto-suffix |
| Clone eligibility = non-archived | No Draft→Active command yet |
| FR-122 Option A live SQL | Interim; no DU-5 in P2-M2 |
| Shared `IdentityDbContext` for portfolio tables | Matches BC-10/BC-01 co-location pattern |

---

## 14. Catalog gaps resolved (documented)

| Gap | Resolution |
|-----|------------|
| CMD-022 / EVT-022 missing from Commands-Events-Catalog | Implemented per Aggregate-Catalog + Slice 4 note |
| Clone CMD/EVT missing | Implemented as CloneProject / ProjectCloned + Slice 5 note |
| No ENT-### for ScopeChange | Owned entity; Slice 4 note |
| FR-122 async DU-5 | Interim live rollups + Slice 6 note |

---

## 15. Known limitations

- Projection summaries are **strongly consistent live queries**, not async materialized views (no staleness field).
- Workspace is a **Guid reference** only (full Workspace lifecycle is BC-10 / later).
- Project status stays **Draft** after create unless archived; Active/OnHold/Completed exist in enum but lack transition commands.
- CON-006 “Project direct to Portfolio” path is **not** implemented (always via Program).
- Delivery templates (FR-002) not implemented.

---

## 16. Deferred work (BC-02+)

| Item | Target |
|------|--------|
| Requirements intake / approve | BC-02 (AGG-005, FR-010+) |
| Delivery templates | CAP-003 / FR-002 |
| Analytics DU-5 projectors + staleness | MOD-23 / ADR-SAD-008 full intent |
| HierarchyTree / DashboardSummary | Later analytics milestone |
| Project Activate / status machine commands | CAP-001 lifecycle completeness |
| Blueprint catalog updates (CMD-023, EVT-022/023, ENT ScopeChange) | Governance / ADR-GOV doc amend |

---

## Related freeze artifacts

- [bc01-api-inventory.md](bc01-api-inventory.md)
- [bc01-domain-inventory.md](bc01-domain-inventory.md)
- [bc01-dependency-map.md](bc01-dependency-map.md)
- [bc01-technical-debt.md](bc01-technical-debt.md)
- [bc01-bc02-readiness.md](bc01-bc02-readiness.md)
