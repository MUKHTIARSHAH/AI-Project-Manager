# BC-01 Slice 4 — FR-004 Scope Change catalog gap

**Status:** Implemented (Slice 4)  
**Trace:** FR-004, CAP-004, CON-011, CMD-022 (AGG-004), ADR-003, ADR-GOV-007

## Catalog gap

The approved Aggregate Catalog lists **CMD-022 RecordScopeChange** on AGG-004, and EIM defines **CON-011 ScopeChange**. However:

| Catalog | Gap |
|---------|-----|
| `Commands-Events-Catalog.md` | No CMD-022 / EVT-022 rows |
| `Domain-Tactical-Catalog.md` | No `ENT-###` for ScopeChange |

## Implementation choices (Slice 4)

- **CMD-022** is used as named in `Aggregate-Catalog.md`.
- Domain/integration events are named **`ScopeChangeRecorded`** (plus approve/reject/implement transition events for the FR-004 approval trail). XML/docs cite FR-004 / CON-011 / CAP-004.
- ScopeChange is an **entity owned by AGG-004 Project** (collection), not a new aggregate — ADR-GOV-007 forbids new aggregates without ADR.
- Optional `affectedRequirementCitation` stores CON-011 “must cite affected requirements” as text only; no BC-02 Requirement FK.

## Out of scope

- Slice 5 project cloning (FR-005)
- Slice 6 async read-model projections (FR-122)
- Replan linkage (CAP-011 / FR-021)
