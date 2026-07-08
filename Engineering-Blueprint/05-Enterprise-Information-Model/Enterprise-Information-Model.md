# AI Project Manager — Enterprise Information Model

**Document ID:** EIM-AIPM-001  
**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Classification:** Enterprise Information Architecture  
**Date:** 2026-07-07  
**Location:** `Engineering-Blueprint/05-Enterprise-Information-Model/`  
**Status:** APPROVED

**Parent documents (LOCKED — not modified):**

| Document | ID | Version |
|----------|-----|---------|
| Business Capability Model | BCM-AIPM-001 | 1.0.0 APPROVED |
| Software Requirements Specification | SRS-AIPM-001 | 1.0.0 LOCKED |
| Software Architecture Document | SAD-AIPM-001 | 1.0.0 APPROVED |
| Project Constitution | PC-AIPM-001 | 1.0.0 APPROVED |

---

## 1. Executive Summary

The Enterprise Information Model (EIM) defines **what information exists** in the AIPM business domain—the authoritative conceptual vocabulary between business capabilities (BCM) and application design (DDD).

This model contains **64 canonical concepts** (`CON-001` through `CON-064`), each with exactly one definition and one owner (ADR-GOV-006). Every concept traces to approved capabilities and SRS requirements. No database tables, APIs, aggregates, or code appear here.

**Downstream gate:** `06-Domain-Model` MUST trace every entity and aggregate root to `CON-###`.

---

## 2. Information Architecture Vision

AIPM is the **control plane** for autonomous software delivery. Its information architecture must:

1. **Stabilize vocabulary** before DDD, events, database, APIs, memory, and agent contracts.
2. **Separate portfolio hierarchy** (Portfolio → Program → Project → Initiative) from execution (Plan → Task → Dispatch).
3. **Distinguish workforce** (AgentType, AgentInstance) from platform governance (Policy, Approval, Gate).
4. **Make evidence first-class** (AuditRecord, ExplainabilityRecord, TraceLink).
5. **Encode tenancy and compliance** at the concept level (Tenant, LegalHold, RetentionPolicy).

---

## 3. Relationship to Business Capability Model

| BCM layer | EIM layer |
|-----------|-----------|
| CAP-### What business does | CON-### What information exists to do it |
| 56 capabilities | 64 concepts (many-to-many via traceability) |

Every CAP-001–CAP-056 maps to ≥1 concept. See [Traceability-Matrix.md](Traceability-Matrix.md).

---

## 4. Relationship to Application Architecture (DDD)

| EIM | Future DDD (`06`) |
|-----|-------------------|
| Concept | Aggregate / entity candidate |
| REL-### relationship | Association / bounded context boundary |
| Lifecycle stages (information) | State machine inputs (`08`) |
| Business constraints | Invariants |

**Rule:** DDD MUST NOT introduce information concepts absent from EIM without ADR.

---

## 5. Information Domain Structure

| Domain ID | Name | Concepts |
|-----------|------|----------|
| ID-01 | Organization & Tenancy | CON-001–005 |
| ID-02 | Portfolio & Delivery Structure | CON-006–011 |
| ID-03 | Intent & Requirements | CON-012–015 |
| ID-04 | Planning | CON-016–022 |
| ID-05 | Execution & Orchestration | CON-023–028 |
| ID-06 | Specialist Workforce | CON-029–032 |
| ID-07 | Governance & Policy | CON-033–037 |
| ID-08 | Quality & Release | CON-038–042 |
| ID-09 | Enterprise Integration | CON-043–047 |
| ID-10 | Knowledge & Traceability | CON-048–052 |
| ID-11 | Compliance & Audit | CON-053–055 |
| ID-12 | Financial Stewardship | CON-056–058 |
| ID-13 | Operations & Insight | CON-059–062 |
| ID-14 | Privacy & Retention | CON-063–064 |

---

## 6. Conceptual vs Logical vs Physical

| Layer | Folder | Content |
|-------|--------|---------|
| **Conceptual** | `05` (this model) | CON-###, REL-###, business meaning |
| **Logical / Application** | `06` DDD | Aggregates, commands, domain events |
| **Physical** | `09` Database | Tables, indexes, partitions |

---

## 7. Information Governance Principles

| Principle | Enforcement |
|-----------|-------------|
| One definition, one owner | ADR-GOV-006; owner column in catalog |
| Traceability | Every CON → CAP + FR |
| No orphan information | Relationship map + audit |
| Tenant boundary | CON-001 scopes all tenant-owned concepts |
| Evidence by default | CON-053, CON-022 on all material decisions |
| Minimize PII | CON-063, CON-064; classification on each CON |

---

## 8. Concept Hierarchy Overview

```
Tenant (CON-001)
 └── Organization (CON-002)
      └── Workspace (CON-003)
           └── Portfolio (CON-006) → Program (CON-007) → Project (CON-008) → Initiative (CON-009)
                ├── Requirement (CON-013) → Plan (CON-016) → PlanVersion (CON-017)
                │        └── Task (CON-018) → Dispatch (CON-024) → ExecutionResult (CON-026)
                ├── PolicySet (CON-033) → Approval (CON-036) → GateResult (CON-039) → Release (CON-042)
                └── AuditRecord (CON-053)
```

Full catalog: [Concept-Catalog.md](Concept-Catalog.md). Relationships: [Concept-Relationship-Map.md](Concept-Relationship-Map.md).

---

## 9. Concept Index

| ID | Canonical Name | Owner | Primary CAP |
|----|----------------|-------|-------------|
| CON-001 | Tenant | Platform Administration | CAP-055 |
| CON-002 | Organization | Platform Administration | CAP-055 |
| CON-003 | Workspace | Delivery Management | CAP-001 |
| CON-004 | User | Identity & Access | CAP-039 |
| CON-005 | Role | Identity & Access | CAP-039 |
| CON-006 | Portfolio | Portfolio Management | CAP-002 |
| CON-007 | Program | Portfolio Management | CAP-002 |
| CON-008 | Project | Project Lifecycle | CAP-001 |
| CON-009 | Initiative | Project Lifecycle | CAP-001 |
| CON-010 | DeliveryTemplate | Template Management | CAP-003 |
| CON-011 | ScopeChange | Scope Control | CAP-004 |
| CON-012 | Goal | Intent Management | CAP-005 |
| CON-013 | Requirement | Requirements | CAP-006 |
| CON-014 | AcceptanceCriterion | Requirements | CAP-006 |
| CON-015 | AmbiguityReport | Ambiguity Detection | CAP-007 |
| CON-016 | Plan | Planning | CAP-009 |
| CON-017 | PlanVersion | Planning | CAP-009 |
| CON-018 | Task | Execution | CAP-014 |
| CON-019 | TaskNode | Dependency Management | CAP-010 |
| CON-020 | Milestone | Planning | CAP-009 |
| CON-021 | Dependency | Dependency Management | CAP-010 |
| CON-022 | ExplainabilityRecord | Explainability | CAP-028 |
| CON-023 | WorkSchedule | Scheduling | CAP-013 |
| CON-024 | Dispatch | Delegation | CAP-014 |
| CON-025 | WorkAssignment | Delegation Assurance | CAP-020 |
| CON-026 | ExecutionResult | Output Validation | CAP-021 |
| CON-027 | Artifact | Knowledge | CAP-049 |
| CON-028 | Halt | Emergency Control | CAP-017 |
| CON-029 | AgentType | Workforce Registration | CAP-018 |
| CON-030 | AgentInstance | Workforce Health | CAP-019 |
| CON-031 | Certification | Workforce Certification | CAP-022 |
| CON-032 | Credential | Credential Stewardship | CAP-041 |
| CON-033 | PolicySet | Policy Management | CAP-023 |
| CON-034 | Policy | Policy Management | CAP-023 |
| CON-035 | AutonomyProfile | Autonomy Control | CAP-024 |
| CON-036 | Approval | Approval Orchestration | CAP-026 |
| CON-037 | Decision | Policy Enforcement | CAP-025 |
| CON-038 | GateDefinition | Quality Gates | CAP-029 |
| CON-039 | GateResult | Quality Gates | CAP-029 |
| CON-040 | Waiver | Gate Exceptions | CAP-032 |
| CON-041 | Environment | Environment Promotion | CAP-030 |
| CON-042 | Release | Release Authorization | CAP-031 |
| CON-043 | Connection | Integration | CAP-033 |
| CON-044 | ExternalWorkItem | Work Item Sync | CAP-034 |
| CON-045 | Repository | Pipeline Awareness | CAP-035 |
| CON-046 | PipelineRun | Pipeline Awareness | CAP-035 |
| CON-047 | Deployment | Pipeline Awareness | CAP-035 |
| CON-048 | KnowledgeItem | Knowledge Management | CAP-049 |
| CON-049 | ContextPackage | Context Provision | CAP-050 |
| CON-050 | TraceLink | Traceability | CAP-008 |
| CON-051 | Conversation | Intent Intake | CAP-005 |
| CON-052 | ConflictRecord | Conflict Resolution | CAP-051 |
| CON-053 | AuditRecord | Immutable Audit | CAP-036 |
| CON-054 | LegalHold | Legal Hold | CAP-038 |
| CON-055 | EvidencePackage | Compliance Export | CAP-037 |
| CON-056 | CostRecord | Cost Accounting | CAP-046 |
| CON-057 | Budget | Budget Control | CAP-047 |
| CON-058 | BudgetAlert | Cost Anomaly | CAP-048 |
| CON-059 | Incident | Incident Loop | CAP-053 |
| CON-060 | MaintenanceWork | Post-Release Maintenance | CAP-054 |
| CON-061 | Notification | Stakeholder Notification | CAP-056 |
| CON-062 | PerformanceMetric | Performance Insight | CAP-052 |
| CON-063 | DataSubjectRequest | DSR Support | CAP-044 |
| CON-064 | RetentionPolicy | Retention Governance | CAP-045 |

---

## 10. Global Information Invariants

| INV ID | Invariant |
|--------|-----------|
| INV-01 | Every concept instance (except Tenant) belongs to exactly one Tenant |
| INV-02 | Project MUST belong to exactly one Program or stand as direct child of Portfolio |
| INV-03 | Task MUST belong to exactly one Project |
| INV-04 | Dispatch MUST reference exactly one Task and one AgentType |
| INV-05 | Release MUST reference passed GateResults and required Approvals |
| INV-06 | AuditRecord MUST NOT be deleted; LegalHold overrides retention purge |
| INV-07 | Credential MUST be revocable on Halt |
| INV-08 | No two CON-### share the same canonical meaning |

---

## 11. Supporting Artifacts

| Artifact | Path |
|----------|------|
| Full concept definitions | [Concept-Catalog.md](Concept-Catalog.md) |
| Relationships | [Concept-Relationship-Map.md](Concept-Relationship-Map.md) |
| Glossary | [Information-Glossary.md](Information-Glossary.md) |
| Traceability | [Traceability-Matrix.md](Traceability-Matrix.md) |
| Decisions | [Information-Decision-Log.md](Information-Decision-Log.md) |
| Assumptions | [Assumption-Register.md](Assumption-Register.md) |
| Open questions | [Open-Questions-Register.md](Open-Questions-Register.md) |

---

## 12. Terminology Consistency Audit

| Check | Result |
|-------|--------|
| No duplicate concepts | **PASS** — 64 unique CON |
| No conflicting definitions | **PASS** — glossary matches catalog |
| No undefined terms | **PASS** |
| No unused glossary entries | **PASS** |
| Every CON traces to CAP | **PASS** — see matrix |
| Every CAP has ≥1 CON | **PASS** |
| Bidirectional relationships | **PASS** — REL catalog |
| No orphan concepts | **PASS** |
| One owner per CON | **PASS** |
| Overlaps documented | **PASS** — IDL-001–004 |

---

## 13. Quality Gate Record

| Gate | Result |
|------|--------|
| QG-01 Business | PASS |
| QG-02 Architecture | PASS |
| QG-03 Governance | PASS |
| QG-04 Consistency | PASS |
| QG-05 Traceability | PASS |
| QG-06 Terminology | PASS |
| QG-07 Future Expansion | PASS |
| QG-08 AI | PASS |
| QG-09 Security | PASS |
| QG-10 Documentation | PASS |
| QG-11 Approval | PASS |

---

## References

- BCM-AIPM-001, SRS-AIPM-001, SAD-AIPM-001, PC-AIPM-001
- ADR-GOV-003, ADR-GOV-005, ADR-GOV-006
- PROMPT-05-AIPM v1.1.0, DQG-AIPM-001

---

**ENTERPRISE INFORMATION MODEL STATUS: APPROVED**
