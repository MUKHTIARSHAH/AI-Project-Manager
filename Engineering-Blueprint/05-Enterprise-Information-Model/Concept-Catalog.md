# Concept Catalog — EIM-AIPM-001

**Version:** 1.0.0 | **Status:** APPROVED  
**Rule:** One canonical definition and one canonical owner per CON (ADR-GOV-006).

Full attribute set per PROMPT-05-AIPM v1.1.0. Concepts CON-001–CON-064.

---

## ID-01 — Organization & Tenancy

### CON-001 — Tenant

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-001 |
| Canonical Name | Tenant |
| Canonical Definition | An isolated customer partition of the AIPM platform with its own data, policies, users, and configuration boundaries. |
| Business Purpose | Enable secure multi-tenant enterprise operations. |
| Business Meaning | The top-level scope key for all customer-owned information. |
| Business Owner | Platform Administration |
| Source Capability | CAP-055, CAP-040 |
| Source Requirement | FR-090, FR-092, OBJ-001 |
| Related ADRs | ADR-SAD-006, ADR-001 |
| Parent Concepts | None |
| Child Concepts | CON-002, CON-003, CON-033, CON-057 |
| Peer Concepts | None |
| Allowed Relationships | Tenant 1—N Organization; Tenant 1—N User; Tenant 1—1 RetentionPolicy |
| Forbidden Relationships | Tenant spanning multiple Organizations as owner; cross-Tenant data parentage |
| Cardinality Rules | One Tenant per customer contract; many workspaces per Tenant |
| Lifecycle Stages | Provisioned → Active → Suspended → Deprovisioned |
| Lifecycle Owner | Platform Administration |
| Business Constraints | tenant_id immutable once assigned (ADR-SAD-006) |
| Business Policies | POL-06 cross-tenant prohibition |
| Security Classification | Restricted |
| Privacy Classification | PII (admin contacts) |
| Retention Requirements | Per CON-064; deprovision triggers purge workflow |
| Audit Requirements | All lifecycle changes audited (CON-053) |
| Quality Requirements | 100% operations scoped to Tenant |
| Synonyms | None (do not use "account" as canonical) |
| Deprecated Terms | "Customer account" → use Tenant |
| Examples | Acme Corp production tenant in EU region |
| Non-examples | A single Project; an Organization inside another Tenant |
| Future Evolution Notes | Dedicated silo tenant profile (ADR-SAD-005) |

### CON-002 — Organization

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-002 |
| Canonical Name | Organization |
| Canonical Definition | A customer business entity registered under a Tenant that owns workspaces and delivery portfolios. |
| Business Purpose | Model enterprise customer structure above workspaces. |
| Business Meaning | Business identity within a Tenant. |
| Business Owner | Platform Administration |
| Source Capability | CAP-055 |
| Source Requirement | FR-092, CON-002 |
| Related ADRs | ADR-001 |
| Parent Concepts | CON-001 |
| Child Concepts | CON-003, CON-006 |
| Peer Concepts | CON-004 |
| Allowed Relationships | Organization N—1 Tenant; Organization 1—N Workspace |
| Forbidden Relationships | Organization without Tenant |
| Cardinality Rules | At least one Organization per enterprise Tenant |
| Lifecycle Stages | Registered → Active → Archived |
| Lifecycle Owner | Tenant Administrator |
| Business Constraints | Must belong to exactly one Tenant |
| Business Policies | Enterprise SSO binding at Tenant level |
| Security Classification | Internal |
| Privacy Classification | PII |
| Retention Requirements | Archived orgs retain audit per CON-064 |
| Audit Requirements | Create/archive audited |
| Quality Requirements | Name unique within Tenant |
| Synonyms | None |
| Deprecated Terms | "Company" acceptable in UI only, not in specs |
| Examples | "Acme Engineering Division" |
| Non-examples | External vendor organization not under Tenant |
| Future Evolution Notes | Multi-org conglomerate federation (SRS §44 Year 3) |

### CON-003 — Workspace

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-003 |
| Canonical Name | Workspace |
| Canonical Definition | A bounded collaboration area within an Organization where projects and delivery artifacts are grouped. |
| Business Purpose | Separate delivery contexts (e.g., business units, products). |
| Business Meaning | Container for projects and local configuration. |
| Business Owner | Delivery Management |
| Source Capability | CAP-001 |
| Source Requirement | FR-001 |
| Related ADRs | ADR-SAD-001 |
| Parent Concepts | CON-002 |
| Child Concepts | CON-008, CON-010 |
| Peer Concepts | CON-006 |
| Allowed Relationships | Workspace N—1 Organization; Workspace 1—N Project |
| Forbidden Relationships | Project in multiple Workspaces |
| Cardinality Rules | Project belongs to exactly one Workspace |
| Lifecycle Stages | Created → Active → Archived |
| Lifecycle Owner | Project Owner / Org Admin |
| Business Constraints | Archived workspace blocks new projects |
| Business Policies | Tenant isolation |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Per workspace policy |
| Audit Requirements | Lifecycle audited |
| Quality Requirements | Unique name within Organization |
| Synonyms | None |
| Deprecated Terms | "Space", "folder" — non-canonical |
| Examples | "Payments Platform workspace" |
| Non-examples | Tenant; Portfolio |
| Future Evolution Notes | Workspace-level policy inheritance |

### CON-004 — User

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-004 |
| Canonical Name | User |
| Canonical Definition | A human identity authenticated to AIPM with assigned roles and approval authority. |
| Business Purpose | Human accountability and access control. |
| Business Meaning | Actor for approvals, halts, and configuration. |
| Business Owner | Identity & Access |
| Source Capability | CAP-039 |
| Source Requirement | FR-091, CON-002 |
| Related ADRs | ADR-SAD-007 |
| Parent Concepts | CON-001 |
| Child Concepts | None |
| Peer Concepts | CON-005 |
| Allowed Relationships | User N—M Role; User 1—N Approval |
| Forbidden Relationships | User without Tenant affiliation |
| Cardinality Rules | User unique per IdP subject within Tenant |
| Lifecycle Stages | Invited → Active → Suspended → Removed |
| Lifecycle Owner | Identity & Access |
| Business Constraints | Enterprise SSO required (CON-002) |
| Business Policies | Least privilege; SoD (CAP-027) |
| Security Classification | Confidential |
| Privacy Classification | PII |
| Retention Requirements | Removed user retains audit attribution |
| Audit Requirements | All role changes audited |
| Quality Requirements | No orphan approvals without User |
| Synonyms | None |
| Deprecated Terms | "Actor" — use User for humans |
| Examples | jane.doe@acme.com as Approver |
| Non-examples | AgentInstance |
| Future Evolution Notes | ABAC attributes (PC §8) |

### CON-005 — Role

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-005 |
| Canonical Name | Role |
| Canonical Definition | A named bundle of permissions defining what a User may do within a Tenant. |
| Business Purpose | RBAC for governance and delivery functions. |
| Business Meaning | Permission template assigned to Users. |
| Business Owner | Identity & Access |
| Source Capability | CAP-039 |
| Source Requirement | FR-091 |
| Related ADRs | ADR-SAD-007 |
| Parent Concepts | CON-001 |
| Child Concepts | None |
| Peer Concepts | CON-004 |
| Allowed Relationships | Role N—M User |
| Forbidden Relationships | Role granting cross-Tenant access |
| Cardinality Rules | User has one or more Roles per Tenant |
| Lifecycle Stages | Defined → Active → Deprecated |
| Lifecycle Owner | Identity & Access |
| Business Constraints | Production release requires ReleaseApprover role |
| Business Policies | SoD role pairs configured per Tenant |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Deprecated roles retained for audit |
| Audit Requirements | Role definition changes audited |
| Quality Requirements | No overlapping role definitions with same name |
| Synonyms | None |
| Deprecated Terms | "Permission group" — non-canonical |
| Examples | PlanApprover, SecurityReviewer |
| Non-examples | AgentType capability profile |
| Future Evolution Notes | Custom tenant roles |

---

## ID-02 — Portfolio & Delivery Structure

### CON-006 — Portfolio

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-006 |
| Canonical Name | Portfolio |
| Canonical Definition | An executive-level collection of programs and projects providing aggregated delivery visibility. |
| Business Purpose | Executive oversight across delivery investments. |
| Business Meaning | Top of delivery hierarchy for reporting. |
| Business Owner | Portfolio Management |
| Source Capability | CAP-002 |
| Source Requirement | FR-003, FR-122 |
| Related ADRs | ADR-SAD-008 |
| Parent Concepts | CON-002 |
| Child Concepts | CON-007, CON-008 |
| Peer Concepts | CON-003 |
| Allowed Relationships | Portfolio 1—N Program; Portfolio 1—N Project (direct) |
| Forbidden Relationships | Project in conflicting Portfolios |
| Cardinality Rules | Program optional; Project must link to Portfolio or Program |
| Lifecycle Stages | Defined → Active → Closed |
| Lifecycle Owner | Executive Sponsor |
| Business Constraints | Async status aggregation (FR-122) |
| Business Policies | Tenant-scoped visibility |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Closed portfolio retains history |
| Audit Requirements | Structural changes audited |
| Quality Requirements | Aggregates must reconcile to child projects |
| Synonyms | None |
| Deprecated Terms | "Bucket" |
| Examples | "2026 Digital Transformation Portfolio" |
| Non-examples | Workspace |
| Future Evolution Notes | Cross-portfolio capacity planning |

### CON-007 — Program

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-007 |
| Canonical Name | Program |
| Canonical Definition | A coordinated set of related projects delivering a strategic outcome within a Portfolio. |
| Business Purpose | Group projects with shared objectives. |
| Business Meaning | Mid-level delivery hierarchy between Portfolio and Project. |
| Business Owner | Portfolio Management |
| Source Capability | CAP-002 |
| Source Requirement | FR-003 |
| Related ADRs | ADR-GOV-005 |
| Parent Concepts | CON-006 |
| Child Concepts | CON-008 |
| Peer Concepts | CON-008 |
| Allowed Relationships | Program N—1 Portfolio; Program 1—N Project |
| Forbidden Relationships | Program without Portfolio parent |
| Cardinality Rules | Project may belong to Program OR direct to Portfolio |
| Lifecycle Stages | Initiated → Executing → Completed → Closed |
| Lifecycle Owner | Program Manager |
| Business Constraints | See IDL-001 |
| Business Policies | Status rolls up to Portfolio |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Standard delivery retention |
| Audit Requirements | Yes |
| Quality Requirements | Unique name within Portfolio |
| Synonyms | None |
| Deprecated Terms | "Epic collection" |
| Examples | "Core Banking Modernization Program" |
| Non-examples | Plan |
| Future Evolution Notes | Program-level budgets (CON-057) |

### CON-008 — Project

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-008 |
| Canonical Name | Project |
| Canonical Definition | The primary unit of governed software delivery with lifecycle, owner, requirements, plan, and execution under AIPM authority. |
| Business Purpose | Single source of delivery engagement truth. |
| Business Meaning | Where intent becomes executed and released work. |
| Business Owner | Delivery Management |
| Source Capability | CAP-001 |
| Source Requirement | FR-001, FR-005 |
| Related ADRs | ADR-005 |
| Parent Concepts | CON-003, CON-007, CON-006 |
| Child Concepts | CON-009, CON-013, CON-016, CON-018, CON-057 |
| Peer Concepts | CON-009 |
| Allowed Relationships | Project N—1 Workspace; Project 1—N Requirement, Task, Plan |
| Forbidden Relationships | Project without owner; cross-Tenant Project |
| Cardinality Rules | One active Plan baseline per project at a time |
| Lifecycle Stages | Draft → Active → OnHold → Completed → Archived |
| Lifecycle Owner | Project Owner |
| Business Constraints | Archived read-only (CAP-001) |
| Business Policies | CON-001 platform does not generate app code |
| Security Classification | Internal |
| Privacy Classification | May contain PII in requirements |
| Retention Requirements | Per CON-064 |
| Audit Requirements | All state transitions |
| Quality Requirements | 100% have owner and Tenant |
| Synonyms | None |
| Deprecated Terms | "Job", "run" |
| Examples | "Mobile App v2 Project" |
| Non-examples | Task; Tenant |
| Future Evolution Notes | Project templates (CON-010) |

### CON-009 — Initiative

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-009 |
| Canonical Name | Initiative |
| Canonical Definition | A time-bounded strategic effort within a Project grouping related goals and requirements. |
| Business Purpose | Decompose large projects into strategic threads. |
| Business Meaning | Optional sub-structure under Project for intent grouping. |
| Business Owner | Delivery Management |
| Source Capability | CAP-001 |
| Source Requirement | FR-001 |
| Related ADRs | ADR-GOV-006 |
| Parent Concepts | CON-008 |
| Child Concepts | CON-012, CON-013 |
| Peer Concepts | CON-008 |
| Allowed Relationships | Initiative N—1 Project; Initiative 1—N Goal |
| Forbidden Relationships | Initiative without Project |
| Cardinality Rules | Optional; Project may have zero or many |
| Lifecycle Stages | Proposed → Active → Completed → Cancelled |
| Lifecycle Owner | Project Owner |
| Business Constraints | Cannot span Projects |
| Business Policies | Traceability to requirements |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Inherits Project |
| Audit Requirements | Create/close audited |
| Quality Requirements | Name unique within Project |
| Synonyms | None |
| Deprecated Terms | "Theme" (use Initiative) |
| Examples | "PCI Compliance Initiative" |
| Non-examples | Program |
| Future Evolution Notes | Initiative-level metrics |

### CON-010 — DeliveryTemplate

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-010 |
| Canonical Name | DeliveryTemplate |
| Canonical Definition | A reusable pattern defining default workforce sequences, gates, and policies for new projects. |
| Business Purpose | Standardize repeatable delivery startup. |
| Business Meaning | Blueprint for project instantiation. |
| Business Owner | Delivery Management |
| Source Capability | CAP-003 |
| Source Requirement | FR-002 |
| Related ADRs | CON-006 |
| Parent Concepts | CON-003 |
| Child Concepts | None |
| Peer Concepts | CON-038 |
| Allowed Relationships | Template N—1 Workspace; Template references AgentType, GateDefinition |
| Forbidden Relationships | Template referencing uncertified AgentType |
| Cardinality Rules | Versioned; many templates per Workspace |
| Lifecycle Stages | Draft → Published → Deprecated |
| Lifecycle Owner | Delivery Management |
| Business Constraints | CON-006 no core forks |
| Business Policies | Certification required for workforce types |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Deprecated templates retained |
| Audit Requirements | Publish events audited |
| Quality Requirements | Validated before publish |
| Synonyms | None |
| Deprecated Terms | "Project template" acceptable in speech |
| Examples | "Microservice Greenfield Template" |
| Non-examples | Plan |
| Future Evolution Notes | Marketplace templates |

### CON-011 — ScopeChange

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-011 |
| Canonical Name | ScopeChange |
| Canonical Definition | A formally recorded change to project scope that may trigger replanning and approval. |
| Business Purpose | Controlled scope creep management. |
| Business Meaning | Delta to agreed delivery scope. |
| Business Owner | Delivery Management |
| Source Capability | CAP-004 |
| Source Requirement | FR-004 |
| Related ADRs | ADR-003 |
| Parent Concepts | CON-008 |
| Child Concepts | None |
| Peer Concepts | CON-013, CON-016 |
| Allowed Relationships | ScopeChange N—1 Project; may link Requirement |
| Forbidden Relationships | ScopeChange without Project |
| Cardinality Rules | Many per Project |
| Lifecycle Stages | Proposed → Approved → Rejected → Implemented |
| Lifecycle Owner | Project Owner |
| Business Constraints | Material changes require Approval |
| Business Policies | Links to replan (CAP-011) |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Permanent audit trail |
| Audit Requirements | Full |
| Quality Requirements | Must cite affected requirements |
| Synonyms | None |
| Deprecated Terms | "Change request" — external ITSM term maps via CON-044 |
| Examples | "Add OAuth2 login scope" |
| Non-examples | Task status change |
| Future Evolution Notes | Automated impact scoring |

---

## ID-03 — Intent & Requirements

### CON-012 — Goal | CON-013 — Requirement | CON-014 — AcceptanceCriterion | CON-015 — AmbiguityReport

| ID | Name | Definition | Owner | CAP | FR | Parent | Children |
|----|------|------------|-------|-----|-----|--------|----------|
| CON-012 | Goal | Strategic outcome a project or initiative pursues | Requirements | CAP-005 | FR-010 | CON-009 | CON-013 |
| CON-013 | Requirement | Structured statement of what must be built with priority | Requirements | CAP-006 | FR-011 | CON-008 | CON-014, CON-050 |
| CON-014 | AcceptanceCriterion | Testable condition proving a requirement is met | Requirements | CAP-006 | FR-011 | CON-013 | None |
| CON-015 | AmbiguityReport | Record of unclear or conflicting requirement text | Requirements | CAP-007 | FR-012 | CON-013 | None |

**Shared lifecycle:** Draft → UnderReview → Approved → Superseded → Retired. **Security:** Internal. **Privacy:** CON-013 may be PII. **Audit:** All changes. **Forbidden:** Requirement without Project. **ADRs:** ADR-003, ADR-005.

---

## ID-04 — Planning

### CON-016 — Plan

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-016 |
| Canonical Name | Plan |
| Canonical Definition | A delivery roadmap for a Project comprising phased work breakdown before baseline activation. |
| Business Purpose | Translate requirements into executable structure. |
| Business Meaning | Container for plan versions and tasks. |
| Business Owner | Planning |
| Source Capability | CAP-009 |
| Source Requirement | FR-020 |
| Related ADRs | ADR-003, ADR-005 |
| Parent Concepts | CON-008 |
| Child Concepts | CON-017, CON-018, CON-019, CON-020, CON-021 |
| Peer Concepts | CON-013 |
| Allowed Relationships | Plan 1—1 active Project; Plan 1—N PlanVersion |
| Forbidden Relationships | Plan without Project; cyclic dependencies |
| Cardinality Rules | One logical Plan per Project; many versions |
| Lifecycle Stages | Draft → UnderReview → Baseline → Superseded |
| Lifecycle Owner | Planning / Project Owner |
| Business Constraints | Cycle detection required (FR-021) |
| Business Policies | Baseline requires Approval (CAP-012) |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | All versions retained |
| Audit Requirements | Version activation audited |
| Quality Requirements | Explainability on baseline (CON-022) |
| Synonyms | None |
| Deprecated Terms | "Roadmap" — use Plan |
| Examples | "Q3 release plan" |
| Non-examples | WorkSchedule |
| Future Evolution Notes | Multi-scenario plans |

### CON-017 — PlanVersion

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-017 |
| Canonical Name | PlanVersion |
| Canonical Definition | An immutable snapshot of a Plan at a point in time, one of which may be active baseline. |
| Business Purpose | Audit and replay of planning decisions. |
| Business Meaning | Versioned plan state. |
| Business Owner | Planning |
| Source Capability | CAP-009, CAP-011 |
| Source Requirement | FR-020, FR-023 |
| Related ADRs | ADR-005 |
| Parent Concepts | CON-016 |
| Child Concepts | CON-018, CON-019 |
| Peer Concepts | CON-022 |
| Allowed Relationships | PlanVersion N—1 Plan; triggers WorkSchedule |
| Forbidden Relationships | Two active baselines per Plan |
| Cardinality Rules | Exactly zero or one active baseline |
| Lifecycle Stages | Draft → Simulated → Approved → Active → Superseded |
| Lifecycle Owner | Planning |
| Business Constraints | Simulation does not affect execution until approved |
| Business Policies | FR-024 simulation |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Permanent |
| Audit Requirements | Full |
| Quality Requirements | Linked ExplainabilityRecord on activation |
| Synonyms | None |
| Deprecated Terms | None |
| Examples | "Plan v3 baseline" |
| Non-examples | Task execution state |
| Future Evolution Notes | Diff between versions |

### CON-018 — Task

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-018 |
| Canonical Name | Task |
| Canonical Definition | A unit of delegable work within a Plan assigned to an AgentType for execution under policy. |
| Business Purpose | Atomic orchestration unit. |
| Business Meaning | What gets scheduled and dispatched. |
| Business Owner | Execution |
| Source Capability | CAP-014 |
| Source Requirement | FR-040 |
| Related ADRs | ADR-007, IDL-002 |
| Parent Concepts | CON-016, CON-008 |
| Child Concepts | CON-024 |
| Peer Concepts | CON-019 |
| Allowed Relationships | Task N—1 Project; Task N—1 PlanVersion; Task N—1 AgentType (intended) |
| Forbidden Relationships | Task without Project; Task without policy permit |
| Cardinality Rules | Many tasks per plan |
| Lifecycle Stages | Pending → Scheduled → Dispatched → Completed → Failed → Cancelled |
| Lifecycle Owner | Execution |
| Business Constraints | CON-001 no PM code generation |
| Business Policies | Policy before dispatch |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Per project retention |
| Audit Requirements | All transitions |
| Quality Requirements | Output validation (CON-026) |
| Synonyms | None |
| Deprecated Terms | "Job", "work item" (external = CON-044) |
| Examples | "Implement payment API task" |
| Non-examples | TaskNode (structure only) |
| Future Evolution Notes | Task priority SLAs |

### CON-019 — TaskNode

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-019 |
| Canonical Name | TaskNode |
| Canonical Definition | A structural node in the plan DAG representing grouping or dependency anchor, not necessarily delegable alone. |
| Business Purpose | Model WBS and dependency graph. |
| Business Meaning | Plan structure element. |
| Business Owner | Planning |
| Source Capability | CAP-010 |
| Source Requirement | FR-021 |
| Related ADRs | IDL-002 |
| Parent Concepts | CON-017 |
| Child Concepts | CON-018, CON-021 |
| Peer Concepts | CON-018 |
| Allowed Relationships | TaskNode tree under PlanVersion; Dependency between nodes |
| Forbidden Relationships | Cycles in dependency graph |
| Cardinality Rules | Tasks may reference TaskNode |
| Lifecycle Stages | Active → Removed |
| Lifecycle Owner | Planning |
| Business Constraints | Acyclic graph |
| Business Policies | Critical path derivation |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | With PlanVersion |
| Audit Requirements | Structure changes |
| Quality Requirements | No orphan Task without node path |
| Synonyms | None |
| Deprecated Terms | "Epic", "story" — map in integration only |
| Examples | "Backend phase node" |
| Non-examples | Dispatch |
| Future Evolution Notes | Auto critical path |

### CON-020 — Milestone | CON-021 — Dependency | CON-022 — ExplainabilityRecord

| ID | Name | Definition | Owner | CAP | Key relationships |
|----|------|------------|-------|-----|-------------------|
| CON-020 | Milestone | Named checkpoint in Plan marking significant progress | Planning | CAP-009 | N—1 Plan; marks TaskNode completion |
| CON-021 | Dependency | Directed prerequisite between TaskNodes or Tasks | Planning | CAP-010 | N—N nodes; forbidden cycles |
| CON-022 | ExplainabilityRecord | Human-readable rationale for plan, approval, or release decisions | Governance | CAP-028 | N—1 Decision/PlanVersion/Approval; audit required FR-119 |

**CON-022 detail:** Security Internal; Privacy may include decision context; Retention permanent; ADR ADR-003; Forbidden: replacing AuditRecord (IDL-005).

---

## ID-05 — Execution & Orchestration

### CON-023 — WorkSchedule

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-023 |
| Canonical Name | WorkSchedule |
| Canonical Definition | The ordered queue of Tasks ready for delegation subject to dependencies, quotas, and policy. |
| Business Purpose | Determine when work may proceed. |
| Business Meaning | Scheduling outcome, not calendar UI. |
| Business Owner | Execution |
| Source Capability | CAP-013 |
| Source Requirement | FR-041 |
| Related ADRs | ADR-SAD-009 |
| Parent Concepts | CON-017 |
| Child Concepts | CON-024 |
| Peer Concepts | CON-057 |
| Allowed Relationships | WorkSchedule N—1 Project; entries reference Task |
| Forbidden Relationships | Schedule entry without policy permit |
| Cardinality Rules | One active schedule view per project execution window |
| Lifecycle Stages | Active → Paused → Drained |
| Lifecycle Owner | Execution |
| Business Constraints | Pause on budget (CON-057) or Halt (CON-028) |
| Business Policies | Fail-closed policy evaluation |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Schedule history audited |
| Audit Requirements | Pause/resume audited |
| Quality Requirements | Quota compliance NFR-003 |
| Synonyms | None |
| Deprecated Terms | "Queue" — internal term only |
| Examples | "Sprint 4 schedule" |
| Non-examples | PlanVersion |
| Future Evolution Notes | Priority SLA scheduling |

### CON-024 — Dispatch

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-024 |
| Canonical Name | Dispatch |
| Canonical Definition | The act of delegating a Task to a certified AgentType with context, recorded as an auditable orchestration event. |
| Business Purpose | Core control-plane delegation. |
| Business Meaning | PM assigns work; does not perform work. |
| Business Owner | Execution |
| Source Capability | CAP-014 |
| Source Requirement | FR-040, FR-118, CON-001 |
| Related ADRs | ADR-007, IDL-003 |
| Parent Concepts | CON-018 |
| Child Concepts | CON-025, CON-026 |
| Peer Concepts | CON-025 |
| Allowed Relationships | Dispatch N—1 Task; N—1 AgentType; produces WorkAssignment |
| Forbidden Relationships | Dispatch to uncertified type; PM generating app code |
| Cardinality Rules | Many dispatches per Task (retries) |
| Lifecycle Stages | Requested → Permitted → Sent → Acknowledged → Completed → Failed |
| Lifecycle Owner | Execution |
| Business Constraints | NFR-023 ack SLA |
| Business Policies | BR-01, BR-04 |
| Security Classification | Confidential |
| Privacy Classification | Context may include minimized PII |
| Retention Requirements | Permanent audit |
| Audit Requirements | 100% |
| Quality Requirements | Trace on all paths NFR-015 |
| Synonyms | None |
| Deprecated Terms | "Agent call" |
| Examples | Dispatch backend task to BackendAgent |
| Non-examples | Agent-internal message |
| Future Evolution Notes | Workforce matching |

### CON-025 — WorkAssignment | CON-026 — ExecutionResult | CON-027 — Artifact | CON-028 — Halt

| ID | Name | Definition | Owner | CAP | IDL/Notes |
|----|------|------------|-------|-----|-----------|
| CON-025 | WorkAssignment | Confirmed assignment of Task to AgentInstance after assurance checks | Workforce | CAP-020 | IDL-003; child of Dispatch |
| CON-026 | ExecutionResult | Outcome of workforce work including pass/fail validation | Workforce | CAP-021 | NFR-019 validation required |
| CON-027 | Artifact | Durable deliverable produced by execution (spec, config, report) | Knowledge | CAP-049 | Not customer app repo code stored as PM core |
| CON-028 | Halt | Emergency stop of new dispatches and trigger for credential revoke | Operations | CAP-017 | SC-008 ≤30s; links CON-032 |

---

## ID-06 — Specialist Workforce

### CON-029 — AgentType | CON-030 — AgentInstance | CON-031 — Certification | CON-032 — Credential

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-029 | AgentType | Registered class of specialist contributor (e.g., Backend, Security) | Workforce Platform | CAP-018 |
| CON-030 | AgentInstance | Running deployment of an AgentType with health status | Workforce Platform | CAP-019 |
| CON-031 | Certification | Attestation that AgentType or connector is approved for production | Workforce Platform | CAP-022 |
| CON-032 | Credential | Secret or token reference issued and revocable for workforce/integration | Security | CAP-041 |

**IDL-004:** AgentType is catalog metadata; AgentInstance is runtime. **Forbidden:** Agent-to-agent coordination outside PM (ADR-007). **Lifecycle:** Type Registered→Active→Deprecated; Instance Starting→Healthy→Unhealthy→Retired. **Credential:** Revoke on CON-028 within 60s (FR-117).

---

## ID-07 — Governance & Policy

### CON-033 — PolicySet | CON-034 — Policy | CON-035 — AutonomyProfile | CON-036 — Approval | CON-037 — Decision

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-033 | PolicySet | Versioned collection of Policies for a Tenant | Governance | CAP-023 |
| CON-034 | Policy | Rule evaluating permit/deny for actions | Governance | CAP-023 |
| CON-035 | AutonomyProfile | Configuration of allowed autonomy by environment/action | Governance | CAP-024 |
| CON-036 | Approval | Human authorization decision for gated action | Governance | CAP-026 |
| CON-037 | Decision | Outcome of policy evaluation (permit/deny/obligation) | Governance | CAP-025 |

**Shared:** Parent CON-001 Tenant; CON-036 links User, CON-022; CON-037 triggers before Dispatch/Release. **Policies:** Fail-closed ADR-SAD-009. **Audit:** All. **Forbidden:** Self-approval where SoD applies (CAP-027).

---

## ID-08 — Quality & Release

### CON-038 — GateDefinition | CON-039 — GateResult | CON-040 — Waiver | CON-041 — Environment | CON-042 — Release

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-038 | GateDefinition | Objective quality check configuration (tests, scans) | Quality | CAP-029 |
| CON-039 | GateResult | Pass/fail outcome of gate evaluation | Quality | CAP-029 |
| CON-040 | Waiver | Time-bound exception to failed gate with approval | Quality | CAP-032 |
| CON-041 | Environment | Named deployment stage (dev, staging, production) | Quality | CAP-030 |
| CON-042 | Release | Authorized promotion to production | Quality | CAP-031 |

**Lifecycle Release:** Proposed → Gated → Approved → Deployed → RolledBack. **Forbidden:** Production release without CON-036 and passed CON-039. **Examples:** "v2.1.0 production release".

---

## ID-09 — Enterprise Integration

### CON-043 — Connection | CON-044 — ExternalWorkItem | CON-045 — Repository | CON-046 — PipelineRun | CON-047 — Deployment

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-043 | Connection | Registered link to external SDLC tool | Integrations | CAP-033 |
| CON-044 | ExternalWorkItem | Ticket/issue synchronized with external tracker | Integrations | CAP-034 |
| CON-045 | Repository | External source code repository reference | Integrations | CAP-035 |
| CON-046 | PipelineRun | CI/CD pipeline execution snapshot | Integrations | CAP-035 |
| CON-047 | Deployment | Record of application deployment in environment | Integrations | CAP-035 |

**Note:** CON-047 Deployment is customer application deployment awareness, not AIPM platform deployment (→ folder 15). **Privacy:** Connection credentials via CON-032 only.

---

## ID-10 — Knowledge & Traceability

### CON-048 — KnowledgeItem | CON-049 — ContextPackage | CON-050 — TraceLink | CON-051 — Conversation | CON-052 — ConflictRecord

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-048 | KnowledgeItem | Persistent delivery knowledge (decision, doc summary) | Knowledge | CAP-049 |
| CON-049 | ContextPackage | Assembled information bundle for a Dispatch | Knowledge | CAP-050 |
| CON-050 | TraceLink | Bidirectional link between requirements and artifacts | Requirements | CAP-008 |
| CON-051 | Conversation | Intake dialogue or narrative capturing intent | Requirements | CAP-005 |
| CON-052 | ConflictRecord | Detected conflict between concurrent delivery changes | Knowledge | CAP-051 |

**Privacy:** CON-049 subject to minimization (CON-043 concept CAP-043). **Forbidden:** ContextPackage including unnecessary PII.

---

## ID-11 — Compliance & Audit

### CON-053 — AuditRecord | CON-054 — LegalHold | CON-055 — EvidencePackage

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-053 | AuditRecord | Immutable append-only record of material action | Compliance | CAP-036 |
| CON-054 | LegalHold | Suspension of deletion for legal proceedings | Compliance | CAP-038 |
| CON-055 | EvidencePackage | Export bundle for auditors (SOC2, GDPR) | Compliance | CAP-037 |

**IDL-005:** AuditRecord is system of record; ExplainabilityRecord is human rationale subset. **Forbidden:** Deleting AuditRecord. **Retention:** LegalHold overrides CON-064 purge.

---

## ID-12 — Financial Stewardship

### CON-056 — CostRecord | CON-057 — Budget | CON-058 — BudgetAlert

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-056 | CostRecord | Attributed resource consumption cost | FinOps | CAP-046 |
| CON-057 | Budget | Spending limit for Tenant or Project | FinOps | CAP-047 |
| CON-058 | BudgetAlert | Notification of anomalous or threshold spend | FinOps | CAP-048 |

**Constraints:** Budget stop ≤60s SC-006; CON-056 rolls up from Dispatch/execution events.

---

## ID-13 — Operations & Insight

### CON-059 — Incident | CON-060 — MaintenanceWork | CON-061 — Notification | CON-062 — PerformanceMetric

| ID | Name | Definition | Owner | CAP |
|----|------|------------|-------|-----|
| CON-059 | Incident | Production disruption requiring response | Operations | CAP-053 |
| CON-060 | MaintenanceWork | Scheduled post-release upkeep work | Operations | CAP-054 |
| CON-061 | Notification | Delivered alert to stakeholder | Platform Admin | CAP-056 |
| CON-062 | PerformanceMetric | Measured delivery or platform KPI | Operations | CAP-052 |

**Lifecycle Incident:** Open → Triaged → Remediation → Closed. Links to CON-018 Task for fixes.

---

## ID-14 — Privacy & Retention

### CON-063 — DataSubjectRequest

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-063 |
| Canonical Name | DataSubjectRequest |
| Canonical Definition | A regulated request from a data subject for access, rectification, or erasure of personal data. |
| Business Purpose | GDPR compliance. |
| Business Meaning | Privacy rights workflow object. |
| Business Owner | Privacy Officer |
| Source Capability | CAP-044 |
| Source Requirement | FR-094, NFR-010 |
| Related ADRs | ADR-006 |
| Parent Concepts | CON-001 |
| Child Concepts | None |
| Peer Concepts | CON-064, CON-054 |
| Allowed Relationships | DSR N—1 Tenant; may reference User |
| Forbidden Relationships | Erasure under active LegalHold |
| Cardinality Rules | Many per Tenant |
| Lifecycle Stages | Received → InProgress → Fulfilled → Rejected |
| Lifecycle Owner | Privacy Officer |
| Business Constraints | SLA ≤30 days |
| Business Policies | LegalHold precedence |
| Security Classification | Restricted |
| Privacy Classification | Sensitive PII |
| Retention Requirements | DSR records retained |
| Audit Requirements | Full |
| Quality Requirements | Identity verification |
| Synonyms | None |
| Deprecated Terms | "GDPR ticket" |
| Examples | Erasure request for user X |
| Non-examples | AuditRecord deletion request |
| Future Evolution Notes | Self-service portal |

### CON-064 — RetentionPolicy

| Attribute | Value |
|-----------|-------|
| Concept ID | CON-064 |
| Canonical Name | RetentionPolicy |
| Canonical Definition | Rules governing how long information concepts are kept before archive or purge. |
| Business Purpose | Compliance and cost control. |
| Business Meaning | Lifecycle limit configuration. |
| Business Owner | Privacy Officer |
| Source Capability | CAP-045 |
| Source Requirement | FR-093 |
| Related ADRs | ADR-006 |
| Parent Concepts | CON-001 |
| Child Concepts | None |
| Peer Concepts | CON-054 |
| Allowed Relationships | RetentionPolicy 1—1 Tenant (default) |
| Forbidden Relationships | Purge under LegalHold |
| Cardinality Rules | One active default per Tenant |
| Lifecycle Stages | Draft → Active → Superseded |
| Lifecycle Owner | Privacy Officer |
| Business Constraints | FR-103 legal hold |
| Business Policies | Tiered retention by concept class |
| Security Classification | Internal |
| Privacy Classification | None |
| Retention Requirements | Policy itself retained |
| Audit Requirements | Changes audited |
| Quality Requirements | Mapped to concept classes |
| Synonyms | None |
| Deprecated Terms | None |
| Examples | "7-year audit retention" |
| Non-examples | Backup schedule (technology) |
| Future Evolution Notes | Automated tiering |

---

**Catalog complete:** CON-001 through CON-064. Full attribute expansion for grouped concepts available in [Information-Glossary.md](Information-Glossary.md) and [Enterprise-Information-Model.md](Enterprise-Information-Model.md).


