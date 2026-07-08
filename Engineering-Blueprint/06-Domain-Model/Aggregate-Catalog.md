# Aggregate Catalog — DM-AIPM-001

**Version:** 1.0.0 | **Status:** APPROVED | **Frozen:** ADR-GOV-007

Each aggregate includes lifecycle stages: **Creation → Activation → Modification → Suspension → Completion → Archival → Deletion** (where applicable).

---

## AGG-001 — Tenant

| Field | Value |
|-------|-------|
| **Aggregate Root** | Tenant |
| **BC** | BC-10 |
| **Business Purpose** | Root tenancy boundary for all customer information |
| **Responsibilities** | Isolate data; bind retention; scope configuration |
| **Consistency Boundary** | Tenant + default RetentionPolicy reference + Organization refs |
| **Transaction Boundary** | Single tenant mutation per command |
| **Lifecycle** | Provisioned → Active → Suspended → Deprovisioned |
| **Invariants (always)** | tenant_id immutable; one active default retention policy |
| **Must never** | Cross-tenant parentage; operation without tenant scope |
| **Commands** | CMD-001 ProvisionTenant, CMD-002 SuspendTenant |
| **Events** | EVT-001 TenantProvisioned, EVT-002 TenantSuspended |
| **Policies** | POL-001 TenantIsolation |
| **Dependencies** | None (root) |
| **Trace** | CON-001 · CAP-040,055 · FR-090,092 · MOD-13 · PC §8 · ADR-SAD-006 |

---

## AGG-002 — Portfolio | AGG-003 — Program | AGG-004 — Project

| AGG | Root | Purpose | Key invariants | Commands | Events |
|-----|------|---------|----------------|----------|--------|
| AGG-002 | Portfolio | Executive rollup | Unique name per Tenant; child projects valid | CMD-010 CreatePortfolio | EVT-010 PortfolioCreated |
| AGG-003 | Program | Strategic grouping | Belongs to one Portfolio | CMD-011 CreateProgram | EVT-011 ProgramCreated |
| AGG-004 | Project | Primary delivery unit | Has owner; belongs to Workspace; archived=read-only | CMD-020 CreateProject, CMD-021 ArchiveProject, CMD-022 RecordScopeChange | EVT-020 ProjectCreated, EVT-021 ProjectArchived |

**AGG-004 entities:** ENT-001 Initiative, ENT-002 WorkspaceRef, ENT-003 DeliveryTemplateRef  
**Trace:** CON-006–011,008 · CAP-001–004 · FR-001–005 · MOD-02 · PC §2 · IDL-001

---

## AGG-005 — Requirement

| Field | Value |
|-------|-------|
| **Root** | Requirement |
| **BC** | BC-02 |
| **Purpose** | Structured delivery intent with acceptance criteria |
| **Consistency** | Requirement + AcceptanceCriteria + AmbiguityReports |
| **Invariants** | Must belong to Project; approved before plan baseline |
| **Must never** | Orphan requirement; critical ambiguity unescalated |
| **Commands** | CMD-030 IntakeRequirement, CMD-031 ApproveRequirement |
| **Events** | EVT-030 RequirementIntaken, EVT-031 RequirementApproved |
| **Trace** | CON-013–015 · CAP-005–007 · FR-010–012 · MOD-03 |

---

## AGG-006 — Plan | AGG-007 — PlanVersion

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-006 | Plan | Plan container per Project | One Plan per Project | CMD-040 CreatePlan | EVT-040 PlanCreated |
| AGG-007 | PlanVersion | Versioned snapshot | Acyclic TaskNodes; ≤1 active baseline; cycle rejected | CMD-041 AddTaskNodes, CMD-042 SimulateReplan, CMD-043 ActivatePlanVersion | EVT-041 PlanVersionActivated, EVT-042 ReplanSimulated |

**AGG-007 entities:** ENT-010 TaskNode, ENT-011 Dependency, ENT-012 Milestone, ENT-013 ExplainabilityRecord  
**Trace:** CON-016–022 · CAP-009–012 · FR-020–024,119 · MOD-04 · IDL-002

---

## AGG-008 — Task

| Field | Value |
|-------|-------|
| **Root** | Task |
| **BC** | BC-04 |
| **Purpose** | Delegable work unit |
| **Consistency** | Task + ExecutionResult refs + Dispatch refs |
| **Invariants** | Belongs to Project and PlanVersion; policy permit before dispatch |
| **Must never** | Dispatch without certification; PM generating app code |
| **Commands** | CMD-050 CreateTasksFromPlan, CMD-051 MarkTaskFailed, CMD-052 CompleteTask |
| **Events** | EVT-050 TaskCreated, EVT-051 TaskCompleted, EVT-052 TaskFailed |
| **Trace** | CON-018 · CAP-014–016 · FR-040,043 · MOD-06 · ADR-007 |

---

## AGG-009 — WorkSchedule | AGG-010 — Dispatch | AGG-011 — Halt

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-009 | WorkSchedule | Ordered ready tasks | Respects budget and halt | CMD-060 BuildSchedule, CMD-061 PauseSchedule | EVT-060 ScheduleBuilt, EVT-061 SchedulePaused |
| AGG-010 | Dispatch | Delegation act | Certified AgentType only; produces WorkAssignment | CMD-070 DispatchTask | EVT-070 TaskDispatched, EVT-071 DispatchFailed |
| AGG-011 | Halt | Emergency stop | Stops new dispatches; triggers credential revoke | CMD-080 EngageHalt, CMD-081 ReleaseHalt | EVT-080 HaltEngaged, EVT-081 HaltReleased |

**AGG-010 entities:** ENT-020 WorkAssignment, ENT-021 ExecutionResult  
**Trace:** CON-023–028 · CAP-013–017 · FR-041,042,117 · MOD-05,08,27 · ADR-SAD-009,010 · IDL-003

---

## AGG-012 — AgentType | AGG-013 — AgentInstance

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-012 | AgentType | Workforce catalog | Registered before dispatch; certification for prod | CMD-090 RegisterAgentType | EVT-090 AgentTypeRegistered |
| AGG-013 | AgentInstance | Runtime workforce | Healthy for schedule inclusion | CMD-091 ReportHealth | EVT-091 AgentHealthChanged |

**Entities:** ENT-030 Certification, ENT-031 Credential  
**Trace:** CON-029–032 · CAP-018–022,041 · FR-030–034 · MOD-07,14 · ADR-007 · IDL-004

---

## AGG-014 — PolicySet | AGG-015 — Approval

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-014 | PolicySet | Tenant policies | Versioned; fail-closed evaluation | CMD-100 PublishPolicySet, CMD-101 EvaluatePolicy | EVT-100 PolicySetPublished, EVT-101 PolicyDenied |
| AGG-015 | Approval | Human gate | SoD enforced; blocks until resolved | CMD-110 RequestApproval, CMD-111 GrantApproval, CMD-112 DenyApproval | EVT-110 ApprovalGranted, EVT-111 ApprovalDenied |

**Trace:** CON-033–037,036 · CAP-023–028 · FR-050–054,052 · MOD-09,11 · ADR-SAD-009,003

---

## AGG-016 — GateDefinition | AGG-017 — Release

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-016 | GateDefinition | Quality gate config | GateResult immutable once recorded | CMD-120 DefineGate, CMD-121 RecordGateResult, CMD-122 GrantWaiver | EVT-120 GatePassed, EVT-121 GateFailed |
| AGG-017 | Release | Production authorization | Requires gates + approvals + traceability | CMD-130 RequestRelease, CMD-131 AuthorizeRelease | EVT-130 ReleaseAuthorized, EVT-131 ReleaseRejected |

**Entities:** ENT-040 GateResult, ENT-041 Waiver, ENT-042 EnvironmentRef  
**Trace:** CON-038–042 · CAP-029–032 · FR-060–063 · MOD-10 · OBJ-004

---

## AGG-018 — AuditRecord | AGG-019 — EvidencePackage

| AGG | Root | Purpose | Invariants | Commands | Events |
|-----|------|---------|------------|----------|--------|
| AGG-018 | AuditRecord | Immutable log | Append-only; tenant-scoped queries | CMD-140 AppendAudit (system) | EVT-140 AuditRecordAppended |
| AGG-019 | EvidencePackage | Compliance export | Scoped to tenant | CMD-141 RequestEvidenceExport | EVT-141 EvidenceExportReady |

**Must never:** Delete or mutate AuditRecord  
**Trace:** CON-053–055 · CAP-036–037 · FR-080–082,121 · MOD-12 · IDL-005

---

## AGG-020 — Connection | AGG-021 — ExternalWorkItem

| AGG | Root | Purpose | Commands | Events | Trace |
|-----|------|---------|----------|--------|-------|
| AGG-020 | Connection | Integration link | CMD-150 RegisterConnection | EVT-150 ConnectionRegistered | CON-043 · CAP-033 · MOD-15 |
| AGG-021 | ExternalWorkItem | Synced ticket | CMD-151 SyncExternalWorkItem | EVT-151 ExternalWorkItemSynced | CON-044 · CAP-034 · MOD-17 |

---

## AGG-022 — KnowledgeItem

| Field | Value |
|-------|-------|
| **Root** | KnowledgeItem |
| **BC** | BC-12 |
| **Purpose** | Persistent delivery knowledge |
| **Commands** | CMD-160 RecordKnowledge |
| **Events** | EVT-160 KnowledgeRecorded |
| **Trace** | CON-048,027 · CAP-049 · FR-100 · MOD-19 |

**ContextPackage** produced via **DS-001 ContextAssemblyService** (not aggregate root)—see Tactical Catalog.

---

## AGG-023 — Budget | AGG-024 — CostRecord | AGG-025 — Notification

| AGG | Root | Purpose | Key command | Key event | Trace |
|-----|------|---------|-------------|-----------|-------|
| AGG-023 | Budget | Spending limit | CMD-170 SetBudget | EVT-170 BudgetExceeded | CON-057 · CAP-047 · MOD-22 |
| AGG-024 | CostRecord | Cost attribution | CMD-171 RecordCost | EVT-171 CostRecorded | CON-056 · CAP-046 · MOD-22 |
| AGG-025 | Notification | Stakeholder alert | CMD-180 SendNotification | EVT-180 NotificationSent | CON-061 · CAP-056 · MOD-24 |

---

## Global Domain Invariants (cross-aggregate)

| ID | Rule |
|----|------|
| DOM-INV-01 | All mutations scoped to Tenant |
| DOM-INV-02 | Commands require policy permit when action is governed |
| DOM-INV-03 | Material decisions produce AuditRecord |
| DOM-INV-04 | Production Release requires Approval + Gate pass |
| DOM-INV-05 | Domain events originate only from aggregates listed above |
