# Bounded Contexts — DM-AIPM-001

**Version:** 1.0.0 | **Status:** APPROVED

---

## BC-01 — Project Portfolio

| Field | Value |
|-------|-------|
| **Purpose** | Manage portfolio hierarchy and project lifecycle |
| **Responsibilities** | Portfolio, Program, Project, Workspace, Initiative, ScopeChange, DeliveryTemplate |
| **Inputs** | Tenant config, template selection, scope change requests |
| **Outputs** | Active projects, hierarchy for rollup |
| **Upstream** | BC-10 Identity |
| **Downstream** | BC-02 Requirements, BC-03 Planning |
| **Shared Kernel** | TenantId, ProjectId with BC-03, BC-04 |
| **ACL** | None |
| **OHS** | None |
| **Published Language** | ProjectCreated, ProjectArchived |
| **Partnership** | BC-03 on plan ownership |
| **Customer/Supplier** | Supplier to BC-02 |
| **Separate Ways** | None |
| **Owner** | Platform Edge & Core |
| **CAP** | CAP-001–004 |
| **MOD** | MOD-02 |

---

## BC-02 — Requirements

| Field | Value |
|-------|-------|
| **Purpose** | Capture and structure delivery intent |
| **Responsibilities** | Requirement, Goal, AcceptanceCriterion, AmbiguityReport, TraceLink |
| **Inputs** | Intent intake, external tickets (BC-11) |
| **Outputs** | Approved requirements, trace links |
| **Upstream** | BC-01 Project |
| **Downstream** | BC-03 Planning (Conformist) |
| **Shared Kernel** | RequirementId |
| **ACL** | BC-11 external ticket → Requirement mapping |
| **OHS** | None |
| **Published Language** | RequirementApproved, AmbiguityDetected |
| **Partnership** | BC-03 |
| **Customer/Supplier** | Customer to BC-03 |
| **Separate Ways** | None |
| **Owner** | Planning Domain |
| **CAP** | CAP-005–008 |
| **MOD** | MOD-03 |

---

## BC-03 — Planning

| Field | Value |
|-------|-------|
| **Purpose** | Produce and baseline executable plans |
| **Responsibilities** | Plan, PlanVersion, TaskNode, Dependency, Milestone, ExplainabilityRecord |
| **Inputs** | Requirements, policies, workforce catalog |
| **Outputs** | Activated PlanVersion, task breakdown |
| **Upstream** | BC-02, BC-06 Policy, BC-07 Approvals |
| **Downstream** | BC-04 Execution (Partnership) |
| **Shared Kernel** | PlanVersionId, TaskNodeId with BC-04 |
| **ACL** | None |
| **OHS** | None |
| **Published Language** | PlanVersionActivated, ReplanProposed |
| **Partnership** | BC-04, BC-07 |
| **Customer/Supplier** | Supplier of work breakdown to BC-04 |
| **Separate Ways** | None |
| **Owner** | Planning Domain |
| **CAP** | CAP-009–012 |
| **MOD** | MOD-04 |

---

## BC-04 — Scheduling & Execution

| Field | Value |
|-------|-------|
| **Purpose** | Schedule, delegate, monitor, and halt delivery work |
| **Responsibilities** | Task, WorkSchedule, Dispatch, ExecutionResult, Halt, Incident linkage |
| **Inputs** | PlanVersion, policy permits, workforce availability, budget |
| **Outputs** | Dispatches, results, halt state |
| **Upstream** | BC-03, BC-06, BC-07, BC-08, BC-12, BC-13 |
| **Downstream** | BC-05 Agents, BC-09 Audit, BC-13 Cost, BC-15 Analytics |
| **Shared Kernel** | TaskId, DispatchId |
| **ACL** | BC-05 Dispatch Protocol |
| **OHS** | None |
| **Published Language** | TaskDispatched, TaskCompleted, HaltEngaged |
| **Partnership** | BC-03, BC-07 |
| **Customer/Supplier** | Customer to BC-05; Supplier of events to BC-09 |
| **Separate Ways** | None |
| **Owner** | Execution Domain |
| **CAP** | CAP-013–017, CAP-053 |
| **MOD** | MOD-05, MOD-06, MOD-08, MOD-27 |

---

## BC-05 — Agent Management

| Field | Value |
|-------|-------|
| **Purpose** | Register and assure specialist workforce |
| **Responsibilities** | AgentType, AgentInstance, Certification, Credential |
| **Inputs** | Manifests, health signals, certification requests |
| **Outputs** | Availability, certification status |
| **Upstream** | BC-10 Identity |
| **Downstream** | BC-04 (workforce for Dispatch) |
| **Shared Kernel** | AgentTypeId |
| **ACL** | Dispatch Protocol from BC-04 |
| **OHS** | Workforce catalog queries |
| **Published Language** | AgentTypeRegistered, CertificationGranted |
| **Partnership** | None |
| **Customer/Supplier** | Supplier to BC-04 |
| **Separate Ways** | No agent-to-agent coordination (ADR-007) |
| **Owner** | Execution Domain |
| **CAP** | CAP-018–022, CAP-041 |
| **MOD** | MOD-07, MOD-14 |

---

## BC-06 — Policy & Autonomy

| Field | Value |
|-------|-------|
| **Purpose** | Define and publish governance rules |
| **Responsibilities** | PolicySet, Policy, AutonomyProfile, Decision outcomes |
| **Inputs** | Tenant policy definitions |
| **Outputs** | Policy evaluation (Published Language) |
| **Upstream** | BC-10 |
| **Downstream** | BC-03, BC-04, BC-08 |
| **Shared Kernel** | PolicySetId, DecisionId |
| **ACL** | None |
| **OHS** | PolicyEvaluation (Published Language) |
| **Published Language** | Permit, Deny, Obligation |
| **Partnership** | None |
| **Customer/Supplier** | Supplier to execution contexts |
| **Separate Ways** | LLM providers (ADR-004) |
| **Owner** | Governance & Compliance |
| **CAP** | CAP-023–025 |
| **MOD** | MOD-09 |

---

## BC-07 — Approvals

| Field | Value |
|-------|-------|
| **Purpose** | Human authorization workflows |
| **Responsibilities** | Approval, ExplainabilityRecord attachment, SoD enforcement |
| **Inputs** | Approval triggers from BC-03, BC-04, BC-08 |
| **Outputs** | ApprovalGranted, ApprovalDenied |
| **Upstream** | BC-10 User/Role |
| **Downstream** | BC-03, BC-04, BC-08 |
| **Shared Kernel** | ApprovalId |
| **ACL** | None |
| **OHS** | None |
| **Published Language** | ApprovalGranted, ApprovalDenied |
| **Partnership** | BC-03, BC-04, BC-08 |
| **Customer/Supplier** | None |
| **Separate Ways** | None |
| **Owner** | Governance & Compliance |
| **CAP** | CAP-026–028 |
| **MOD** | MOD-11 |

---

## BC-08 — Quality Gates

| Field | Value |
|-------|-------|
| **Purpose** | Objective quality and release control |
| **Responsibilities** | GateDefinition, GateResult, Waiver, Environment, Release |
| **Inputs** | CI signals (BC-11), execution artifacts |
| **Outputs** | Gate pass/fail, release authorization |
| **Upstream** | BC-06 Policy, BC-07 Approvals, BC-11 |
| **Downstream** | BC-04 (unblock promotion) |
| **Shared Kernel** | GateDefinitionId, ReleaseId |
| **ACL** | External CI results |
| **OHS** | None |
| **Published Language** | GatePassed, GateFailed, ReleaseAuthorized |
| **Partnership** | BC-07 |
| **Customer/Supplier** | Customer to BC-04 for promotion |
| **Separate Ways** | None |
| **Owner** | Governance & Compliance |
| **CAP** | CAP-029–032 |
| **MOD** | MOD-10 |

---

## BC-09 — Audit & Compliance

| Field | Value |
|-------|-------|
| **Purpose** | Immutable evidence and compliance export |
| **Responsibilities** | AuditRecord, EvidencePackage, LegalHold coordination |
| **Inputs** | Domain events from all contexts |
| **Outputs** | Audit trail, export bundles |
| **Upstream** | All publishing contexts |
| **Downstream** | External auditors (conceptual) |
| **Shared Kernel** | AuditRecordId |
| **ACL** | Export format translation |
| **OHS** | AuditQuery (read) |
| **Published Language** | AuditRecordAppended |
| **Partnership** | None |
| **Customer/Supplier** | Consumer of events |
| **Separate Ways** | None |
| **Owner** | Governance & Compliance |
| **CAP** | CAP-036–038 |
| **MOD** | MOD-12 |

---

## BC-10 — Identity & Access

| Field | Value |
|-------|-------|
| **Purpose** | Tenancy, users, roles |
| **Responsibilities** | Tenant, Organization, Workspace, User, Role |
| **Inputs** | IdP assertions |
| **Outputs** | Authorization decisions (OHS) |
| **Upstream** | Customer IdP |
| **Downstream** | All contexts |
| **Shared Kernel** | TenantId, UserId |
| **ACL** | IdP protocol |
| **OHS** | AuthorizationService |
| **Published Language** | RoleAssigned |
| **Partnership** | None |
| **Customer/Supplier** | Supplier to all |
| **Separate Ways** | None |
| **Owner** | Security & Identity |
| **CAP** | CAP-039, CAP-040, CAP-055 |
| **MOD** | MOD-13, MOD-24 |

---

## BC-11 — Integrations

| Field | Value |
|-------|-------|
| **Purpose** | Enterprise SDLC connectivity |
| **Responsibilities** | Connection, ExternalWorkItem, Repository ref, PipelineRun, Deployment awareness |
| **Inputs** | External webhooks, sync jobs |
| **Outputs** | Normalized external signals |
| **Upstream** | External systems |
| **Downstream** | BC-02, BC-08 |
| **Shared Kernel** | ExternalId mapping |
| **ACL** | All external models |
| **OHS** | None |
| **Published Language** | ExternalWorkItemSynced |
| **Partnership** | None |
| **Customer/Supplier** | ACL to external |
| **Separate Ways** | External vendor models |
| **Owner** | Integrations |
| **CAP** | CAP-033–035 |
| **MOD** | MOD-15, MOD-17 |

---

## BC-12 — Knowledge & Context

| Field | Value |
|-------|-------|
| **Purpose** | Delivery knowledge and dispatch context |
| **Responsibilities** | KnowledgeItem, ContextPackage, ConflictRecord |
| **Inputs** | Artifacts, decisions, requirements |
| **Outputs** | ContextPackage for Dispatch |
| **Upstream** | BC-02, BC-04 |
| **Downstream** | BC-04 |
| **Shared Kernel** | ProjectId, RequirementId (IDs only) |
| **ACL** | None |
| **OHS** | ContextAssembly |
| **Published Language** | ContextPackagePrepared |
| **Partnership** | BC-04 |
| **Customer/Supplier** | Supplier to BC-04 |
| **Separate Ways** | None |
| **Owner** | Knowledge & AI Safety |
| **CAP** | CAP-049–051, CAP-050 |
| **MOD** | MOD-19, MOD-28 |

---

## BC-13 — Cost Management

| Field | Value |
|-------|-------|
| **Purpose** | Cost attribution and budget control |
| **Responsibilities** | Budget, CostRecord, BudgetAlert |
| **Inputs** | Resource consumption from BC-04 |
| **Outputs** | Budget stop signals |
| **Upstream** | BC-04 events |
| **Downstream** | BC-04 (pause schedule) |
| **Shared Kernel** | BudgetId |
| **ACL** | None |
| **OHS** | None |
| **Published Language** | BudgetExceeded |
| **Partnership** | BC-04 |
| **Customer/Supplier** | Customer to BC-04 |
| **Separate Ways** | None |
| **Owner** | Analytics & FinOps |
| **CAP** | CAP-046–048 |
| **MOD** | MOD-22 |

---

## BC-14 — Notifications

| Field | Value |
|-------|-------|
| **Purpose** | Stakeholder alerts |
| **Responsibilities** | Notification delivery |
| **Inputs** | Triggers from all contexts |
| **Outputs** | Delivered Notification |
| **Upstream** | BC-10 User |
| **Downstream** | Users |
| **Shared Kernel** | UserId |
| **ACL** | Channel providers |
| **OHS** | None |
| **Published Language** | NotificationSent |
| **Partnership** | None |
| **Customer/Supplier** | None |
| **Separate Ways** | Email/chat providers |
| **Owner** | Platform Admin |
| **CAP** | CAP-056 |
| **MOD** | MOD-24 |

---

## BC-15 — Analytics & Insights

| Field | Value |
|-------|-------|
| **Purpose** | Read-only delivery and platform insight |
| **Responsibilities** | PerformanceMetric projections |
| **Inputs** | Events from BC-04, BC-13 |
| **Outputs** | Dashboards (conceptual consumer) |
| **Upstream** | BC-04, BC-13 |
| **Downstream** | Stakeholders |
| **Shared Kernel** | Read replicas of IDs |
| **ACL** | None |
| **OHS** | MetricQuery (read-only) |
| **Published Language** | N/A (consumer only) |
| **Partnership** | None |
| **Customer/Supplier** | Customer of events |
| **Separate Ways** | None |
| **Owner** | Analytics & FinOps |
| **CAP** | CAP-052 |
| **MOD** | MOD-23, MOD-30 |

**Note:** BC-15 has no mutable aggregates per CQRS read model (ADR-SAD-008).
