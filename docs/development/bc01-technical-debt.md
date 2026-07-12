# BC-01 Technical Debt Report

**Scope:** Actual debt only — items that create measurable gap vs blueprint intent or operational risk.  
**Freeze HEAD:** `2ef3a00`

| ID | Debt | Evidence | Impact | Suggested resolution |
|----|------|----------|--------|----------------------|
| TD-01 | FR-122 interim live SQL instead of async materialized projections | Slice 6 decision log; ADR-SAD-008 / MOD-23 / DU-5 | No projection lag metric; heavier OLTP read path under scale | Introduce Analytics projectors when DU-5 is scheduled |
| TD-02 | Commands-Events-Catalog missing Clone / ScopeChange event rows | Slice 4/5 notes; Aggregate-Catalog incomplete for clone | Traceability relies on implementation docs | Blueprint catalog amend (governance) |
| TD-03 | No `ENT-###` for ScopeChange | Domain-Tactical-Catalog gap | Entity identity not catalog-indexed | Add ENT row without new AGG |
| TD-04 | Project lifecycle statuses Active/OnHold/Completed lack commands | Enum present; only Draft after create + Archive | Clone “Active-only” was unreachable; status rollups under-populated | Add CAP-001 transition commands later |
| TD-05 | Workspace is opaque Guid (no BC-01 Workspace aggregate) | CMD-020 precondition “Workspace exists” caller-enforced by reference | Weak referential integrity to Workspace | Align with BC-10 Workspace APIs / FK when ready |
| TD-06 | CON-006 allows Project→Portfolio direct; runtime requires Program | Hierarchy decision Portfolio→Program→Project | Cannot model direct portfolio projects | Accept for GA path or ADR if product requires direct link |
| TD-07 | FR-002 Delivery Templates not in BC-01 | CAP-003 deferred | No template-driven project bootstrap | Separate slice / BC capability |

**Explicitly not debt (accepted decisions):** HierarchyTree out of scope; DashboardSummary out of scope; no auto-suffix on clone names; ScopeChange owned by Project (not new AGG).
