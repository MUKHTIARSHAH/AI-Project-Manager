# Domain Tactical Catalog — DM-AIPM-001

**Version:** 1.0.0 | Entities · Value Objects · Domain Services · Repositories · Factories · Policies

---

## Value Objects

| VO ID | Name | Meaning | Immutable | Equality | Validation | Constraints |
|-------|------|---------|-----------|----------|------------|-------------|
| VO-001 | TenantId | Unique tenant identifier | Yes | Value | Non-empty, immutable | Assigned at provision |
| VO-002 | ProjectId | Project identifier | Yes | Value | UUID format | Unique per tenant |
| VO-003 | PlanVersionId | Plan version identifier | Yes | Value | Non-empty | Unique per plan |
| VO-004 | TaskId | Task identifier | Yes | Value | Non-empty | Unique per tenant |
| VO-005 | DispatchId | Dispatch identifier | Yes | Value | Non-empty | Unique per tenant |
| VO-006 | AgentTypeId | Workforce type identifier | Yes | Value | Registered type | Must exist in catalog |
| VO-007 | PolicyDecision | Permit/Deny/Obligation | Yes | Value | Enum | Fail-closed default Deny |
| VO-008 | Money | Cost amount with currency | Yes | Value | Non-negative | Currency ISO code |
| VO-009 | TraceabilityRef | Link to requirement/artifact | Yes | Structural | Valid target id | Bidirectional integrity |
| VO-010 | ExplainabilityText | Human rationale | Yes | Value | Max length policy | Required for baseline/release |
| VO-011 | GateVerdict | Pass/Fail/Waived | Yes | Value | Enum | Immutable once recorded |
| VO-012 | ResidencyRegion | Data region pin | Yes | Value | Allowed region set | FR-120 |
| VO-013 | AutonomyLevel | Autonomy tier | Yes | Value | Enum per profile | Production conservative default |

---

## Key Entities (within aggregates)

| ENT ID | Name | Parent AGG | Identity | Responsibilities | Lifecycle |
|--------|------|------------|----------|------------------|-----------|
| ENT-001 | Initiative | AGG-004 | initiative_id | Group goals under project | Active → Completed |
| ENT-010 | TaskNode | AGG-007 | task_node_id | WBS/DAG structure | Active → Removed |
| ENT-011 | Dependency | AGG-007 | dependency_id | Prerequisite edge | Created → Removed |
| ENT-012 | Milestone | AGG-007 | milestone_id | Checkpoint marker | Open → Achieved |
| ENT-013 | ExplainabilityRecord | AGG-007,015 | explain_id | Decision rationale | Immutable after write |
| ENT-020 | WorkAssignment | AGG-010 | assignment_id | Assured binding to instance | Active → Completed |
| ENT-021 | ExecutionResult | AGG-008 | result_id | Validated outcome | Proposed → Accepted/Rejected |
| ENT-030 | Certification | AGG-012 | cert_id | Production approval | Pending → Granted/Revoked |
| ENT-031 | Credential | AGG-013 | cred_id | Secret reference | Issued → Revoked |
| ENT-040 | GateResult | AGG-016 | gate_result_id | Evaluation outcome | Immutable |
| ENT-041 | Waiver | AGG-016 | waiver_id | Time-bound exception | Active → Expired |

---

## Domain Services

| DS ID | Name | Responsibility | Trigger | Inputs | Outputs | Side Effects |
|-------|------|----------------|---------|--------|---------|--------------|
| DS-001 | ContextAssemblyService | Build ContextPackage for dispatch | CMD-070 | Task, Requirements, Knowledge | ContextPackage | None on aggregates |
| DS-002 | PolicyEvaluationService | Evaluate policy at PEP | Pre-command | PolicySet, action context | PolicyDecision | Decision record |
| DS-003 | TraceabilityService | Maintain TraceLinks | Requirement/artifact change | Requirement, Artifact refs | TraceLink set | Updates ENT links |
| DS-004 | CriticalPathService | Derive critical path | Plan structure change | TaskNodes, Dependencies | Path summary | Read-only on AGG-007 |
| DS-005 | SoDValidationService | Enforce segregation of duties | CMD-111 | User, action | Permit/Deny | None |
| DS-006 | CostAttributionService | Attribute cost to project | EVT-171 | CostRecord, Task | Attribution | CostRecord creation |

---

## Repositories (domain interface — persistence independent)

| REP ID | Aggregate | Query responsibilities |
|--------|-----------|------------------------|
| REP-001 | Tenant | By id; active tenants |
| REP-002 | Project | By workspace; by portfolio |
| REP-003 | PlanVersion | Active baseline for project |
| REP-004 | Task | By project; ready for schedule |
| REP-005 | Dispatch | By task; in-flight |
| REP-006 | AgentType | Certified types for production |
| REP-007 | PolicySet | Active set for tenant |
| REP-008 | Approval | Pending for user |
| REP-009 | AuditRecord | By tenant, time, entity (append stream) |

---

## Factories

| FAC ID | Creates | Rules | Invariants enforced |
|--------|---------|-------|---------------------|
| FAC-001 | Project | From template + workspace | Owner assigned; tenant scope |
| FAC-002 | PlanVersion | From plan + requirements | Acyclic graph |
| FAC-003 | Task | From TaskNode + PlanVersion | Valid agent type ref |
| FAC-004 | Dispatch | From Task + permit | Certification check |
| FAC-005 | ContextPackage | Via DS-001 | PII minimization |
| FAC-006 | AuditRecord | From domain event | Immutable fields set |

---

## Domain Policies

| POL ID | Trigger | Decision | Outcome | Constraints |
|--------|---------|----------|---------|-------------|
| POL-001 | Any command | Tenant scope | Deny if cross-tenant | PC §8 |
| POL-002 | CMD-070 DispatchTask | Policy + certification | Permit/Deny | ADR-SAD-009 |
| POL-003 | CMD-043 ActivatePlanVersion | Autonomy + approval | Permit/Deny | FR-022 |
| POL-004 | CMD-131 AuthorizeRelease | Gates + approval + trace | Permit/Deny | OBJ-004 |
| POL-005 | Budget check on schedule | Spend vs limit | Pause schedule | SC-006 |
| POL-006 | CMD-080 EngageHalt | Always permitted | Halt engaged | SC-008 |
| POL-007 | Output validation | NFR-019 | Reject unvalidated | CAP-021 |

---

## Validation Rules (domain level)

| VAL ID | Applies to | Rule |
|--------|------------|------|
| VAL-001 | AGG-007 | TaskNode graph acyclic |
| VAL-002 | AGG-010 | AgentType certified |
| VAL-003 | AGG-004 | Project has owner before Active |
| VAL-004 | AGG-017 | TraceLinks exist for release scope |
| VAL-005 | AGG-015 | Approver ≠ requester when SoD applies |
