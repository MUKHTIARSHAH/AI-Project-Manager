# BC-01 Slice 5 — FR-005 Project Cloning catalog gap and decision log

**Status:** Implemented (Slice 5)  
**Trace:** FR-005, CAP-001, CON-008, AGG-004, ADR-SAD-008, ADR-SAD-004, ADR-005

## Catalog gap

FR-005 and CAP-001 require project cloning for repeatable delivery patterns. However:

| Catalog | Gap |
|---------|-----|
| `Aggregate-Catalog.md` (AGG-004) | Lists CMD-020 / CMD-021 / CMD-022 only — no CloneProject command |
| `Commands-Events-Catalog.md` | No CMD-023 / EVT-023 (CloneProject / ProjectCloned) rows |

This implementation follows the approved P2-M2 plan: implement cloning with an explicit FR-005 trace note.

## Approved Slice 5 decisions (not explicit in blueprint)

| Decision | Resolution |
|----------|------------|
| Eligibility | Any **non-archived** status (Draft, Active, OnHold, Completed). Archived MUST NOT be cloned. |
| Name | `CloneProjectCommand` requires an explicit new name; uniqueness via tenant-scoped rules; no auto-suffix. |
| Workspace | `WorkspaceId` preserved from source. |
| Events | Publish `ProjectClonedDomainEvent` + `ProjectClonedIntegrationEvent` only — **not** `ProjectCreated`. |
| Behavior | New Guid; same Tenant/Program/Owner/Workspace; Draft; new CreatedAt; ArchivedAt null; ScopeChanges / audit / source domain events **not** copied. |

## Implementation mapping

- Domain: `ProjectAggregate.Clone(newName)` → `ProjectClonedDomainEvent`
- Application: `CloneProjectCommand` / `CloneProjectCommandHandler` / `CloneProjectResponse` / `ProjectClonedIntegrationEvent`
- API: `POST /api/v1/projects/{id}/clone`
- Persistence: reuse `portfolio_projects` via existing `IProjectRepository.AddAsync` — **no new table / migration**

## Out of scope

- Slice 6 async read-model projections (FR-122)
- Delivery templates (FR-002 / CAP-003)
- Draft → Active lifecycle command
