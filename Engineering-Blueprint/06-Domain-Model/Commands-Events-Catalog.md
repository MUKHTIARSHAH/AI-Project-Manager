# Commands & Domain Events — DM-AIPM-001

**Version:** 1.0.0 | **Rule:** Every CMD → CAP + CON + AGG. Every EVT originates from AGG (ADR-GOV-007).

---

## Command catalog

| CMD | Name | AGG | CAP | CON | Preconditions | Postconditions | Auth |
|-----|------|-----|-----|-----|---------------|----------------|------|
| CMD-001 | ProvisionTenant | AGG-001 | CAP-055 | CON-001 | Valid contract | Tenant Active | Platform admin |
| CMD-020 | CreateProject | AGG-004 | CAP-001 | CON-008 | Workspace exists | Project Draft | Project creator |
| CMD-021 | ArchiveProject | AGG-004 | CAP-001 | CON-008 | No active dispatches | Project Archived | Project owner |
| CMD-030 | IntakeRequirement | AGG-005 | CAP-005 | CON-013 | Project active | Requirement draft | Member |
| CMD-031 | ApproveRequirement | AGG-005 | CAP-006 | CON-013 | Parsed | Requirement approved | PO |
| CMD-040 | CreatePlan | AGG-006 | CAP-009 | CON-016 | Requirements exist | Plan created | Planner |
| CMD-043 | ActivatePlanVersion | AGG-007 | CAP-012 | CON-017 | Approved; acyclic | Baseline active | Approver |
| CMD-050 | CreateTasksFromPlan | AGG-008 | CAP-009 | CON-018 | Active baseline | Tasks pending | System |
| CMD-060 | BuildSchedule | AGG-009 | CAP-013 | CON-023 | Policy permit | Schedule active | System |
| CMD-070 | DispatchTask | AGG-010 | CAP-014 | CON-024 | Scheduled; certified | Dispatch sent | System |
| CMD-080 | EngageHalt | AGG-011 | CAP-017 | CON-028 | Authorized | Halt active | Operator |
| CMD-090 | RegisterAgentType | AGG-012 | CAP-018 | CON-029 | Valid manifest | Type registered | Platform |
| CMD-100 | PublishPolicySet | AGG-014 | CAP-023 | CON-033 | Validated rules | Policy active | Admin |
| CMD-110 | RequestApproval | AGG-015 | CAP-026 | CON-036 | Gate triggered | Pending approval | System |
| CMD-111 | GrantApproval | AGG-015 | CAP-026 | CON-036 | SoD pass | Approved | Approver |
| CMD-121 | RecordGateResult | AGG-016 | CAP-029 | CON-039 | CI evidence | Result immutable | System |
| CMD-131 | AuthorizeRelease | AGG-017 | CAP-031 | CON-042 | Gates+approvals | Release authorized | Release approver |
| CMD-140 | AppendAudit | AGG-018 | CAP-036 | CON-053 | Material action | Record appended | System |
| CMD-150 | RegisterConnection | AGG-020 | CAP-033 | CON-043 | Certified connector | Connection active | Admin |
| CMD-170 | SetBudget | AGG-023 | CAP-047 | CON-057 | Valid limit | Budget active | FinOps |

*Full CMD list extends symmetrically for remaining aggregates; all trace in Domain-Traceability-Matrix.*

---

## Domain event catalog

| EVT | Name | Source AGG | Trigger CMD | Business impact | Subscribers (BC) |
|-----|------|------------|-------------|-----------------|------------------|
| EVT-001 | TenantProvisioned | AGG-001 | CMD-001 | Tenant ready | BC-10 |
| EVT-020 | ProjectCreated | AGG-004 | CMD-020 | Delivery begins | BC-02, BC-09 |
| EVT-031 | RequirementApproved | AGG-005 | CMD-031 | Planning input ready | BC-03, BC-09 |
| EVT-041 | PlanVersionActivated | AGG-007 | CMD-043 | Execution may schedule | BC-04, BC-09 |
| EVT-050 | TaskCreated | AGG-008 | CMD-050 | Work units exist | BC-04, BC-09 |
| EVT-060 | ScheduleBuilt | AGG-009 | CMD-060 | Tasks queued | BC-04 |
| EVT-070 | TaskDispatched | AGG-010 | CMD-070 | Workforce engaged | BC-05, BC-09, BC-13, BC-15 |
| EVT-080 | HaltEngaged | AGG-011 | CMD-080 | Delegation stopped | BC-05, BC-09, BC-14 |
| EVT-090 | AgentTypeRegistered | AGG-012 | CMD-090 | Workforce available | BC-04, BC-09 |
| EVT-100 | PolicySetPublished | AGG-014 | CMD-100 | Rules updated | BC-03,04,08 |
| EVT-110 | ApprovalGranted | AGG-015 | CMD-111 | Action unblocked | BC-03,04,08 |
| EVT-120 | GatePassed | AGG-016 | CMD-121 | Quality met | BC-04, BC-08 |
| EVT-130 | ReleaseAuthorized | AGG-017 | CMD-131 | Production permitted | BC-04, BC-09, BC-11 |
| EVT-140 | AuditRecordAppended | AGG-018 | CMD-140 | Evidence captured | BC-15 |
| EVT-170 | BudgetExceeded | AGG-023 | (rollup) | Schedule pause | BC-04, BC-14 |
| EVT-180 | NotificationSent | AGG-025 | CMD-180 | Human informed | External |

---

## CMD–EVT–State linkage (for Prompt 07–08)

| CMD | Postcondition state (aggregate) | EVT |
|-----|--------------------------------|-----|
| CMD-043 | PlanVersion: Active baseline | EVT-041 |
| CMD-070 | Dispatch: Sent | EVT-070 |
| CMD-080 | Halt: Engaged | EVT-080 |
| CMD-131 | Release: Authorized | EVT-130 |

Per ADR-GOV-004.

---

## Traceability sample

| CMD-070 | CAP-014 · CON-024 · FR-040 · MOD-08 · PC §2 · ADR-007 |
| EVT-070 | AGG-010 only · CAP-014 · CON-024 · MOD-06 |
