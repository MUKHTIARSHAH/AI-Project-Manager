# AI Project Manager — Business Capability Model

**Document ID:** BCM-AIPM-001  
**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Classification:** Enterprise Business Architecture  
**Date:** 2026-07-07  
**Location:** `Engineering-Blueprint/04-Business-Capability-Model/Business-Capability-Model.md`  
**Status:** APPROVED

**Parent documents (LOCKED — not modified):**

| Document | ID | Version |
|----------|-----|---------|
| Software Requirements Specification | SRS-AIPM-001 | 1.0.0 |
| Software Architecture Document | SAD-AIPM-001 | 1.0.0 |
| Project Constitution | PC-AIPM-001 | 1.0.0 |

---

## 1. Executive Summary

The Business Capability Model (BCM) defines **what business capabilities** the AI Project Manager platform must possess to coordinate autonomous software delivery for enterprise customers. It describes the business of AIPM—not how capabilities are implemented.

AIPM is the control plane for software delivery: it governs intent, planning, work coordination, quality, compliance, and human accountability while specialist contributors (including AI-assisted roles) perform delivery work under its authority (SRS §3, CON-001, PC Section 2).

This model contains **56 leaf capabilities** (CAP-001 through CAP-056) organized in three hierarchy levels. Every capability is traced to SRS requirements, SAD modules, constitutional principles, and ADRs. No capability exists without traceability.

---

## 2. Business Mission Alignment

| SRS / Constitution source | Business alignment |
|---------------------------|-------------------|
| SRS §3 Mission | Coordinate specialized contributors to plan, execute, verify, and deliver software safely and measurably |
| SRS §2 Vision | Trusted brain of autonomous software engineering at enterprise scale |
| PC Section 2 | AIPM is control plane; does not generate customer application code |
| SRS CON-001 | Governance and coordination only; execution belongs to workforce |

The BCM expresses mission as **business capabilities** that remain valid regardless of technology choices.

---

## 3. Business Value Proposition

| Stakeholder | Value delivered by capabilities |
|-------------|------------------------------|
| Enterprise executive | Predictable, auditable autonomous delivery with risk controls |
| Engineering leadership | Standardized delivery across teams without tool fragmentation |
| Security & compliance | Evidence, isolation, policy enforcement, human gates |
| Product owners | Transparent plans, progress, and approval accountability |
| Operations | Halt, budget control, incident loops, performance insight |

**Core proposition:** AIPM provides **governed delivery orchestration**—not code generation—as a business service.

---

## 4. Business Objectives

Derived from SRS §8 (OBJ-001 through OBJ-008):

| Obj ID | Business objective | Supporting capability groups |
|--------|-------------------|------------------------------|
| OBJ-001 | Support enterprise-scale multi-tenant operations | CAP-039, CAP-040, CAP-055 |
| OBJ-002 | Coordinate 14+ specialist workforce types | CAP-018–CAP-022 |
| OBJ-003 | Fully auditable delivery timeline | CAP-036, CAP-028 |
| OBJ-004 | Configurable human gates for production | CAP-026, CAP-031 |
| OBJ-005 | Integrate with enterprise SDLC tools | CAP-033–CAP-035 |
| OBJ-006 | Accurate plan and dependency fidelity | CAP-009–CAP-011 |
| OBJ-007 | Reconstructable incident history | CAP-036, CAP-017 |
| OBJ-008 | Reliable platform availability | CAP-052 (insight); platform NFRs via CAP-040 |

---

## 5. Strategic Objectives

| Strategy | Capability enablers | SRS / ADR source |
|----------|--------------------|--------------------|
| Become default orchestration layer | CAP-033–035, CAP-020 | SRS BG-001 |
| Reduce time-to-production | CAP-009–CAP-016, CAP-029–031 | SRS BG-002 |
| Enterprise upsell via governance | CAP-023–028, CAP-036–042 | SRS BG-003, ADR-001 |
| Ecosystem extensibility | CAP-018, CAP-022 | SRS BG-004, ADR-007 |
| Margin protection via cost control | CAP-046–048 | SRS BG-005, NFR-020 |

---

## 6. Operating Model

AIPM operates as a **governed delivery orchestration service**:

```
Customer Intent → Requirements → Plan → Schedule → Delegate Work → Validate → Gate → Release → Monitor → Maintain
                      ↑              ↑           ↑              ↑
                 Human Approval  Policy      Quality       Audit &
                 & Explainability Enforcement  Gates         Compliance
```

**Operating principles (business, not technical):**

1. **Governance before execution** — policy and approval capabilities precede irreversible actions (ADR-003, PC Section 38).
2. **Single source of delivery truth** — AIPM holds authoritative delivery state (ADR-005).
3. **Human sovereignty** — customers configure how much autonomy is permitted (SRS §42, PC Section 36).
4. **Evidence by default** — every decision and delegation is recorded (FR-080, SC-003).

---

## 7. Business Functions

| Function | Description | Primary L1 capability |
|----------|-------------|----------------------|
| F-01 Portfolio management | Manage projects and programs | L1-01 |
| F-02 Intent management | Capture and normalize what to build | L1-02 |
| F-03 Planning | Turn intent into executable plans | L1-03 |
| F-04 Orchestration | Schedule and coordinate work | L1-04, L1-05 |
| F-05 Governance | Policies, approvals, gates | L1-06, L1-07, L1-08 |
| F-06 Assurance | Audit, security, privacy, compliance | L1-09–L1-12 |
| F-07 Stewardship | Cost, knowledge, operations | L1-13–L1-15 |
| F-08 Administration | Tenant and platform configuration | L1-16 |

---

## 8. Business Services

Business services are **outward-facing offerings** composed of capabilities:

| Service ID | Business service | Composed capabilities |
|------------|------------------|----------------------|
| BS-01 | Governed Project Delivery | CAP-001–004, CAP-009–016, CAP-026–031 |
| BS-02 | Workforce Coordination Service | CAP-018–022 |
| BS-03 | Enterprise Integration Service | CAP-033–035 |
| BS-04 | Compliance Evidence Service | CAP-036–038, CAP-028 |
| BS-05 | Trust & Identity Service | CAP-039–042 |
| BS-06 | Financial Governance Service | CAP-046–048 |
| BS-07 | Operational Insight Service | CAP-052–054 |
| BS-08 | Platform Administration Service | CAP-055–056 |

---

## 9. Business Capability Overview

AIPM possesses **16 Level-1 capabilities** decomposed into **56 Level-3 leaf capabilities**. Capabilities are grouped as:

- **Core (22):** Directly deliver customer delivery outcomes
- **Supporting (20):** Enable core delivery (governance, integration, admin)
- **Shared (8):** Cross-cutting (audit, identity, cost insight)
- **Enterprise (6):** Scale, residency, compliance export

---

## 10. Capability Hierarchy

```
L1-01 Portfolio & Delivery Management
  L2-01-01 Project Lifecycle          → CAP-001
  L2-01-02 Portfolio Hierarchy        → CAP-002
  L2-01-03 Delivery Templates         → CAP-003
  L2-01-04 Scope Change Control        → CAP-004

L1-02 Intent & Requirements Management
  L2-02-01 Intent Intake              → CAP-005
  L2-02-02 Requirements Normalization → CAP-006
  L2-02-03 Ambiguity Detection        → CAP-007
  L2-02-04 Requirements Traceability  → CAP-008

L1-03 Delivery Planning
  L2-03-01 Plan Creation              → CAP-009
  L2-03-02 Dependency Management      → CAP-010
  L2-03-03 Replanning & Simulation    → CAP-011
  L2-03-04 Plan Baseline Approval     → CAP-012

L1-04 Work Scheduling & Execution
  L2-04-01 Work Scheduling            → CAP-013
  L2-04-02 Work Delegation            → CAP-014
  L2-04-03 Execution Monitoring       → CAP-015
  L2-04-04 Failure Remediation        → CAP-016
  L2-04-05 Emergency Halt & Recovery  → CAP-017

L1-05 Specialist Workforce Coordination
  L2-05-01 Workforce Registration     → CAP-018
  L2-05-02 Workforce Health Assurance → CAP-019
  L2-05-03 Credential Stewardship     → CAP-041 (shared)
  L2-05-04 Work Delegation Assurance  → CAP-020
  L2-05-05 Output Validation          → CAP-021
  L2-05-06 Workforce Certification    → CAP-022

L1-06 Policy & Autonomy Governance
  L2-06-01 Policy Management          → CAP-023
  L2-06-02 Autonomy Level Control     → CAP-024
  L2-06-03 Policy Enforcement         → CAP-025

L1-07 Approval & Accountability
  L2-07-01 Approval Orchestration     → CAP-026
  L2-07-02 Segregation of Duties      → CAP-027
  L2-07-03 Decision Explainability    → CAP-028

L1-08 Quality & Release Assurance
  L2-08-01 Quality Gate Management    → CAP-029
  L2-08-02 Environment Promotion      → CAP-030
  L2-08-03 Release Authorization      → CAP-031
  L2-08-04 Gate Exception Management  → CAP-032

L1-09 Enterprise Connectivity
  L2-09-01 Connection Management      → CAP-033
  L2-09-02 Work Item Synchronization  → CAP-034
  L2-09-03 SDLC Pipeline Awareness    → CAP-035

L1-10 Audit & Compliance Evidence
  L2-10-01 Immutable Activity Record  → CAP-036
  L2-10-02 Compliance Export          → CAP-037
  L2-10-03 Legal Hold                 → CAP-038

L1-11 Security & Trust
  L2-11-01 Identity & Access          → CAP-039
  L2-11-02 Tenant Isolation           → CAP-040
  L2-11-03 Data Residency             → CAP-042

L1-12 Privacy & Data Stewardship
  L2-12-01 Data Minimization          → CAP-043
  L2-12-02 Data Subject Rights        → CAP-044
  L2-12-03 Retention Governance       → CAP-045

L1-13 Financial Stewardship
  L2-13-01 Cost Accounting            → CAP-046
  L2-13-02 Budget Control             → CAP-047
  L2-13-03 Cost Anomaly Awareness     → CAP-048

L1-14 Knowledge & Traceability
  L2-14-01 Delivery Knowledge         → CAP-049
  L2-14-02 Work Context Provision     → CAP-050
  L2-14-03 Delivery Conflict Resolution → CAP-051

L1-15 Operational Assurance
  L2-15-01 Performance Insight        → CAP-052
  L2-15-02 Incident Response Loop     → CAP-053
  L2-15-03 Post-Release Maintenance   → CAP-054

L1-16 Platform Administration
  L2-16-01 Tenant Configuration       → CAP-055
  L2-16-02 Stakeholder Notification   → CAP-056
```

---

## 11. Level 1 Capabilities

| L1 ID | Name | Purpose | Child count |
|-------|------|---------|-------------|
| L1-01 | Portfolio & Delivery Management | Govern delivery portfolios and projects | 4 |
| L1-02 | Intent & Requirements Management | Capture and structure what must be built | 4 |
| L1-03 | Delivery Planning | Produce approved plans from intent | 4 |
| L1-04 | Work Scheduling & Execution | Schedule and monitor delivery work | 5 |
| L1-05 | Specialist Workforce Coordination | Coordinate certified specialist contributors | 6 |
| L1-06 | Policy & Autonomy Governance | Define and enforce autonomy rules | 3 |
| L1-07 | Approval & Accountability | Human accountability for key decisions | 3 |
| L1-08 | Quality & Release Assurance | Objective quality and release control | 4 |
| L1-09 | Enterprise Connectivity | Connect to customer SDLC ecosystem | 3 |
| L1-10 | Audit & Compliance Evidence | Record and export evidence | 3 |
| L1-11 | Security & Trust | Protect tenants and data | 3 |
| L1-12 | Privacy & Data Stewardship | Honor privacy obligations | 3 |
| L1-13 | Financial Stewardship | Control and account for delivery cost | 3 |
| L1-14 | Knowledge & Traceability | Preserve delivery knowledge | 3 |
| L1-15 | Operational Assurance | Sustain operations post-release | 3 |
| L1-16 | Platform Administration | Configure tenant platform behavior | 2 |

---

## 12. Level 2 Capabilities

Level-2 capabilities are summarized in Section 10 hierarchy. Each L2 maps to exactly one leaf CAP (CAP-001–CAP-056) in this BCM version. Future BCM minor versions MAY decompose high-criticality L2 items into multiple L3 leaves without changing L1 business intent.

---

## 13. Level 3 Capabilities

All 56 leaf capabilities (CAP-001 through CAP-056) constitute Level 3 in this model. Full definitions appear in the **Capability Catalog** below.

---

## 14. Capability Definitions

**Definition standard:** A capability is an ability that a business possesses—the what, not the how. Capabilities:

- Are stable when technology changes
- Are outcome-oriented and measurable
- Are owned by a business role
- Consume inputs and produce outputs of business value

**Not capabilities:** databases, APIs, agents, microservices, programming languages, workflows (these implement capabilities in later blueprint documents).

---

## 15. Capability Responsibilities

| Responsibility domain | Capabilities | Business responsibility |
|----------------------|--------------|------------------------|
| Delivery authority | CAP-001–016 | Maintain coherent delivery from intent to execution |
| Workforce governance | CAP-018–022 | Ensure only certified contributors receive work |
| Risk control | CAP-023–032, CAP-017 | Prevent unsafe autonomous action |
| Trust | CAP-036–045 | Protect data and demonstrate compliance |
| Value control | CAP-046–048 | Prevent runaway cost |
| Insight | CAP-049–054 | Learn and improve from delivery history |

---

## 16. Capability Inputs

| Input category | Examples | Consuming capabilities |
|----------------|----------|------------------------|
| Business intent | Goals, constraints, priorities | CAP-005, CAP-009 |
| Requirements | Features, regulations, NFRs | CAP-006–008 |
| Policies | Autonomy rules, approval chains | CAP-023–025 |
| Workforce availability | Certified contributor status | CAP-013, CAP-020 |
| External signals | Tickets, builds, repository events | CAP-033–035 |
| Financial limits | Budgets, quotas | CAP-047 |
| Human decisions | Approvals, waivers, halts | CAP-026, CAP-032, CAP-017 |

---

## 17. Capability Outputs

| Output category | Examples | Producing capabilities |
|-----------------|----------|------------------------|
| Delivery artifacts | Plans, baselines, status | CAP-009–015 |
| Decisions | Approvals, denials, explainability records | CAP-026–028 |
| Evidence | Audit trails, compliance packages | CAP-036–037 |
| Controlled releases | Release authorization outcomes | CAP-031 |
| Insights | Dashboards, cost reports, anomalies | CAP-046, CAP-048, CAP-052 |
| Remediation | Incident work, maintenance work | CAP-053–054 |

---

## 18. Capability Consumers

| Consumer | Capabilities consumed |
|----------|----------------------|
| Project Owner | CAP-012, CAP-026, CAP-031, CAP-052 |
| Engineering Lead | CAP-011, CAP-051, CAP-016 |
| Compliance Officer | CAP-037, CAP-036, CAP-038 |
| Organization Admin | CAP-023, CAP-039, CAP-055 |
| Operator | CAP-017, CAP-052, CAP-053 |
| Specialist contributor (workforce) | CAP-020, CAP-050 (receives delegated work) |
| Executive stakeholder | CAP-002, CAP-048, CAP-052 |

---

## 19. Capability Providers

| Provider role | Capabilities provided |
|---------------|----------------------|
| AIPM Platform (business service) | All CAP-001–CAP-056 as platform offerings |
| Customer IdP | Identity assertions to CAP-039 |
| Customer SDLC tools | External signals to CAP-033–035 |
| Specialist workforce | Completed work outputs to CAP-021 |
| Customer legal/privacy | Retention and hold rules to CAP-038, CAP-045 |

---

## 20. Capability Dependencies

| Capability | Depends on | Dependency type |
|------------|------------|-----------------|
| CAP-009 | CAP-005–008 | Requires normalized requirements |
| CAP-013 | CAP-009, CAP-012 | Requires approved plan |
| CAP-014 | CAP-013, CAP-025 | Schedule + policy permit |
| CAP-020 | CAP-018–019, CAP-022 | Registered healthy certified workforce |
| CAP-031 | CAP-029–030, CAP-026 | Gates + approval |
| CAP-037 | CAP-036 | Audit records exist |
| CAP-047 | CAP-046 | Cost data available |
| CAP-050 | CAP-049, CAP-008 | Knowledge + traceability |

Full dependency matrix: see Catalog entries and Section 50 validation.

---

## 21. Capability Relationships

| Relationship | Example |
|--------------|---------|
| **Enables** | CAP-023 enables safe CAP-014 |
| **Feeds** | CAP-008 feeds CAP-049 |
| **Governs** | CAP-025 governs CAP-014, CAP-030 |
| **Records** | CAP-036 records all CAP-014, CAP-026 outcomes |
| **Escalates to** | CAP-007 escalates to human via CAP-056 |

---

## 22. Capability Interactions

**Primary interaction flows (business level):**

1. **Intent-to-Release:** CAP-005 → CAP-009 → CAP-012 → CAP-013 → CAP-020 → CAP-021 → CAP-029 → CAP-031  
2. **Governance overlay:** CAP-023–025 evaluate at plan, delegation, and release stages  
3. **Evidence trail:** CAP-036 captures interactions from flows 1–2  
4. **Incident loop:** CAP-053 → CAP-054 → CAP-011 (replan if needed)

---

## 23. Capability Ownership

| L1 group | Business owner role | Operational owner |
|----------|--------------------|--------------------|
| L1-01–L1-04 | VP Engineering / Delivery | Product + Platform Engineering |
| L1-05 | Workforce / Partner Management | Agent Platform Team |
| L1-06–L1-08 | Risk & Governance Council | Governance Engineering |
| L1-09 | Integration Architecture | Integrations Team |
| L1-10–L1-12 | CISO / DPO | Security & Compliance |
| L1-13 | FinOps | Analytics & FinOps |
| L1-14–L1-15 | SRE / Engineering Ops | Operations |
| L1-16 | Platform Product | Platform Admin |

---

## 24. Capability Boundaries

| Inside AIPM business scope | Outside AIPM business scope |
|---------------------------|----------------------------|
| Govern delivery orchestration | Write customer application code (CON-001) |
| Coordinate specialist workforce | Host customer production applications (SRS OOS-002) |
| Enforce gates and approvals | Replace full ITSM/legal/HR systems |
| Record audit evidence | Train foundation AI models (SRS OOS-003) |
| Integrate with SDLC tools | Guarantee fully autonomous delivery without human override (OOS-010) |

---

## 25. Capability Constraints

| Constraint | Affected capabilities | Source |
|------------|----------------------|--------|
| No customer app code generation | All; especially CAP-014 | CON-001, PC §2 |
| Enterprise SSO required | CAP-039 | CON-002 |
| Air-gapped deployment option | CAP-033, CAP-042, CAP-055 | CON-005 |
| No per-customer core forks | CAP-003, CAP-023, CAP-055 | CON-006 |
| Automated compliance export | CAP-037 | CON-007 |
| Independent workforce releases | CAP-018, CAP-022 | CON-008 |

---

## 26. Capability Risks

| Risk | Impacted capabilities | Mitigation capability |
|------|----------------------|----------------------|
| Unsafe autonomous release | CAP-031 | CAP-026, CAP-029, CAP-025 |
| Cost runaway | CAP-014 | CAP-047 |
| Tenant data leak | CAP-040 | CAP-039, CAP-042 |
| Workforce delivers invalid output | CAP-021 | CAP-021, CAP-029 |
| Audit gap | CAP-036 | CAP-036 (immutable design requirement) |
| Policy bypass | CAP-025 | CAP-025 fail-closed (PC §25) |

---

## 27. Capability Policies

Business policies capabilities MUST support:

| Policy ID | Statement | Capabilities |
|-----------|-----------|--------------|
| POL-01 | Production changes require human authorization unless tenant policy explicitly permits otherwise | CAP-026, CAP-031 |
| POL-02 | Segregation of duties on self-approved changes | CAP-027 |
| POL-03 | Fail closed when policy cannot be evaluated | CAP-025 |
| POL-04 | Halt must be available at all times | CAP-017 |
| POL-05 | Budget breach stops new work delegation | CAP-047 |
| POL-06 | Cross-tenant data access prohibited | CAP-040 |

---

## 28. Capability Rules

| Rule ID | Rule | Capabilities |
|---------|------|--------------|
| BR-01 | No work delegation without policy permit | CAP-025, CAP-014 |
| BR-02 | No environment promotion without gate pass | CAP-030, CAP-029 |
| BR-03 | Plan baseline requires approval before execution scheduling | CAP-012, CAP-013 |
| BR-04 | Only certified workforce receives delegation | CAP-022, CAP-020 |
| BR-05 | Explainability required for plan and production decisions | CAP-028 |
| BR-06 | Credentials revoked on halt within business SLA | CAP-041, CAP-017 |

---

## 29. Capability KPIs

| KPI | Definition | Capabilities |
|-----|------------|--------------|
| KPI-01 | Plan approval cycle time | CAP-012, CAP-026 |
| KPI-02 | Gate pass rate | CAP-029 |
| KPI-03 | Delegation success rate | CAP-020, CAP-021 |
| KPI-04 | Audit completeness (% decisions recorded) | CAP-036 |
| KPI-05 | Budget adherence | CAP-047 |
| KPI-06 | Mean time to halt activation | CAP-017 |
| KPI-07 | Integration sync fidelity | CAP-034 |
| KPI-08 | Post-release incident resolution time | CAP-053, CAP-054 |

---

## 30. Capability SLAs

| SLA | Target | Capability | SRS/NFR source |
|-----|--------|------------|------------------|
| SLA-01 | Halt stops new delegation ≤30s | CAP-017 | SC-008 |
| SLA-02 | Credential revocation ≤60s on halt | CAP-041 | FR-117 |
| SLA-03 | Budget stop ≤60s | CAP-047 | SC-006 |
| SLA-04 | Platform availability 99.9% | CAP-052 (measurement) | NFR-004 |
| SLA-05 | GDPR DSR fulfillment ≤30 days | CAP-044 | NFR-010 |

---

## 31. Capability Maturity Assessment

**Maturity scale (BIZBOK-aligned):**

| Level | Name | Description |
|-------|------|-------------|
| 1 | Initial | Ad hoc, inconsistent |
| 2 | Managed | Repeatable with documentation |
| 3 | Defined | Standardized across tenants |
| 4 | Measured | KPI-driven optimization |
| 5 | Optimizing | Continuous improvement embedded |

**GA target:** Core capabilities (CAP-001–031) at Level 3; supporting at Level 2–3.

---

## 32. Current vs Future Maturity

| Capability group | Current (pre-implementation) | GA target | Year-2 target |
|------------------|---------------------------|-----------|---------------|
| Core delivery CAP-001–016 | 1 (defined in BCM) | 3 | 4 |
| Governance CAP-023–032 | 1 | 3 | 4 |
| Trust CAP-036–045 | 1 | 3 | 4 |
| Integration CAP-033–035 | 1 | 2 | 4 |
| FinOps CAP-046–048 | 1 | 3 | 4 |
| Marketplace workforce (future) | — | 2 | 4 |

---

## 33. Capability Heat Map

Priority vs criticality (H=High, M=Medium, L=Low):

| CAP range | Priority | Criticality | Heat |
|-----------|----------|-------------|------|
| CAP-017, CAP-025, CAP-036, CAP-040 | H | H | **Critical** |
| CAP-009–016, CAP-026–031 | H | H | **Critical** |
| CAP-018–022, CAP-039, CAP-047 | H | M | High |
| CAP-033–035, CAP-049–051 | M | M | Medium |
| CAP-048, CAP-054 | M | L | Medium |

---

## 34. Core Capabilities

CAP-001–004, CAP-009–016, CAP-020–021, CAP-026, CAP-029–031 — directly produce customer delivery outcomes.

---

## 35. Supporting Capabilities

CAP-005–008, CAP-018–019, CAP-022–025, CAP-033–035, CAP-055–056 — enable core delivery.

---

## 36. Shared Capabilities

CAP-036–042, CAP-046–048 — cross-cutting trust, audit, cost used by multiple L1 groups.

---

## 37. Enterprise Capabilities

CAP-002, CAP-037, CAP-040, CAP-042, CAP-044, CAP-055 — multi-tenant enterprise scale, residency, compliance.

---

## 38. AI Governance Capabilities

| Capability | AI governance role |
|------------|-------------------|
| CAP-024 | Define autonomy levels for AI-assisted work |
| CAP-025 | Enforce policy on AI-proposed actions |
| CAP-028 | Explain AI-influenced decisions (not raw model internals) |
| CAP-021 | Validate AI-produced outputs before acceptance |
| CAP-043 | Minimize data exposure to AI processing |

**Business rule:** AI proposes; policies and humans dispose (PC Section 6, ADR-003).

---

## 39. Security Capabilities

CAP-039, CAP-040, CAP-041, CAP-042 — identity, isolation, credential stewardship, residency. Align with PC Section 8, SRS §31.

---

## 40. Compliance Capabilities

CAP-036, CAP-037, CAP-038, CAP-044, CAP-045 — audit, export, legal hold, DSR, retention. Align with SRS §33, ADR-006.

---

## 41. Planning Capabilities

CAP-005–012, CAP-011 — full planning business function from intent through approved baseline.

---

## 42. Monitoring Capabilities

CAP-015, CAP-052, CAP-053, CAP-048 — execution monitoring, performance insight, incident loop, cost anomalies. Align with FR-081, FR-084, FR-116.

---

## 43. Decision-Making Capabilities

CAP-023–028, CAP-032 — policies, approvals, explainability, waivers. Human-AI decision boundary enforced.

---

## 44. Knowledge Management Capabilities

CAP-008, CAP-049, CAP-050, CAP-051 — traceability, delivery knowledge, work context, conflict resolution. Align with FR-100–102, FR-110–112.

---

## 45. Human Oversight Capabilities

CAP-026, CAP-027, CAP-028, CAP-017, CAP-031 — approvals, SoD, explainability, halt, release authorization. Align with PC Sections 36–38.

---

## 46. Continuous Improvement Capabilities

CAP-011 (replan), CAP-048 (anomalies), CAP-054 (maintenance), CAP-052 (insight) — learn and adapt delivery approach.

---

## 47. Future Capability Roadmap

| Horizon | New / enhanced capabilities | Source |
|---------|----------------------------|--------|
| GA+6mo | Certified third-party workforce marketplace | SRS §44, FR-034 |
| Year 2 | Portfolio AI capacity planning (extends CAP-002) | SRS §43 |
| Year 2 | Cross-tenant anonymized benchmarks (opt-in) | SRS §44 |
| Year 3 | Federated delivery governance for conglomerates | SRS §44 |
| Year 3 | Continuous compliance live attestation (extends CAP-037) | SRS §44 |

---

## 48. Capability Traceability

See **Capability Traceability Matrix** and per-CAP trace fields in Catalog. Every CAP maps to ≥1 SRS FR/NFR/SC/CON, ≥1 SAD module, ≥1 PC section, and ≥1 ADR where applicable.

---

## 49. Business Capability Summary

AIPM requires **56 business capabilities** across **16 strategic areas** to govern autonomous software delivery. The model separates business what from technical how. Core themes:

1. **Governed orchestration** — not code generation  
2. **Human accountability** — configurable, never absent for production  
3. **Evidence and trust** — audit, security, privacy by design  
4. **Enterprise scale** — multi-tenant, integratable, cost-controlled  
5. **Workforce coordination** — certified specialists under central authority  

---

## 50. Final Capability Assessment

| Criterion | Result |
|-----------|--------|
| All SRS functional groups represented | Pass |
| All success criteria addressable | Pass |
| Constitutional invariants reflected | Pass |
| SAD module coverage | Pass (MOD-01–030 mapped) |
| No implementation leakage | Pass |
| 56 capabilities with unique IDs | Pass |
| Full traceability | Pass |

**Assessment:** BCM-AIPM-001 v1.0.0 is complete and ready to authorize Domain Model work (`05-Domain-Model/`).

---

## Capability Catalog

**Catalog conventions:** Priority (P1=critical path to GA), Criticality (C1=regulatory/safety), Maturity at BCM publication = 1 (Initial/defined), Future Status = GA unless noted.

### CAP-001 — Project Lifecycle Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-001 |
| **Name** | Project Lifecycle Management |
| **Parent Capability** | L1-01 Portfolio & Delivery Management |
| **Child Capabilities** | None (leaf) |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Establish and maintain authoritative project records through delivery lifecycle |
| **Description** | Create, read, update, archive projects with status, ownership, and metadata |
| **Business Value** | Single source of truth for delivery engagements |
| **Inputs** | Project charter, owner assignment, tenant context |
| **Outputs** | Active project record, lifecycle state transitions |
| **Responsibilities** | Maintain project integrity; enforce valid state transitions |
| **Business Rules** | Project belongs to exactly one tenant; archived projects are read-only |
| **Dependencies** | CAP-039 (identity), CAP-040 (tenant) |
| **Risks** | Orphan projects without owner |
| **Constraints** | CON-006 configuration not forks |
| **Success Metrics** | 100% projects have owner; zero cross-tenant visibility |
| **Owner** | Delivery Product |
| **Future Evolution** | Program-level rollup (CAP-002 integration) |
| **Traceability** | FR-001, FR-005; MOD-02; PC §2; ADR-SAD-001 |

### CAP-002 — Portfolio Hierarchy Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-002 |
| **Name** | Portfolio Hierarchy Management |
| **Parent Capability** | L1-01 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Organize projects into programs and portfolios |
| **Description** | Define hierarchy, aggregate status, support executive visibility |
| **Business Value** | Executive oversight across multiple deliveries |
| **Inputs** | Project records, hierarchy definitions |
| **Outputs** | Portfolio views, aggregated status |
| **Responsibilities** | Maintain hierarchy integrity; async aggregation |
| **Business Rules** | Child project cannot belong to conflicting portfolios |
| **Dependencies** | CAP-001 |
| **Risks** | Stale aggregates mislead executives |
| **Constraints** | FR-122 async aggregation |
| **Success Metrics** | Aggregate freshness within business SLA |
| **Owner** | Delivery Product |
| **Future Evolution** | Cross-portfolio capacity planning (Year 2) |
| **Traceability** | FR-003, FR-122; MOD-02, MOD-23; PC §2; ADR-SAD-008 |

### CAP-003 — Delivery Template Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-003 |
| **Name** | Delivery Template Management |
| **Parent Capability** | L1-01 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Standardize repeatable delivery patterns |
| **Description** | Define templates with workforce sequences, gates, and default policies |
| **Business Value** | Faster, consistent project startup |
| **Inputs** | Template definitions, certified workforce types |
| **Outputs** | Instantiated project patterns |
| **Responsibilities** | Version templates; certify template changes |
| **Business Rules** | Templates reference only registered workforce types |
| **Dependencies** | CAP-018, CAP-029 |
| **Risks** | Outdated templates cause gate failures |
| **Constraints** | CON-006, FR-002 |
| **Success Metrics** | Template instantiation success rate |
| **Owner** | Delivery Product |
| **Future Evolution** | Marketplace-published templates |
| **Traceability** | FR-002; MOD-02; PC §3; ADR-SAD-001 |

### CAP-004 — Scope Change Control

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-004 |
| **Name** | Scope Change Control |
| **Parent Capability** | L1-01 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Govern changes to project scope |
| **Description** | Record scope changes, trigger replanning and approval when needed |
| **Business Value** | Controlled scope creep management |
| **Inputs** | Change requests, current scope baseline |
| **Outputs** | Approved scope revisions, replan triggers |
| **Responsibilities** | Link scope changes to requirements and plans |
| **Business Rules** | Material scope change requires approval |
| **Dependencies** | CAP-008, CAP-011, CAP-026 |
| **Risks** | Uncontrolled scope expansion |
| **Constraints** | FR-004 |
| **Success Metrics** | % scope changes with approval record |
| **Owner** | Delivery Product |
| **Future Evolution** | Automated impact scoring |
| **Traceability** | FR-004; MOD-02; PC §36; ADR-003 |

### CAP-005 — Intent Intake

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-005 |
| **Name** | Intent Intake |
| **Parent Capability** | L1-02 Intent & Requirements Management |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Capture business intent from multiple channels |
| **Description** | Accept natural language, structured specs, and imported tickets as delivery intent |
| **Business Value** | Low-friction start of delivery |
| **Inputs** | User narratives, documents, external tickets |
| **Outputs** | Raw intent artifacts |
| **Responsibilities** | Preserve original intent; route to normalization |
| **Business Rules** | All intake attributed to project and tenant |
| **Dependencies** | CAP-001, CAP-033 |
| **Risks** | Ambiguous intent propagates downstream |
| **Constraints** | FR-010 |
| **Success Metrics** | Intake-to-normalization latency |
| **Owner** | Requirements Product |
| **Future Evolution** | Multi-language intake |
| **Traceability** | FR-010; MOD-03; PC §2; ADR-005 |

### CAP-006 — Requirements Normalization

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-006 |
| **Name** | Requirements Normalization |
| **Parent Capability** | L1-02 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Structure raw intent into actionable requirements |
| **Description** | Parse, classify, and structure requirements with acceptance criteria |
| **Business Value** | Executable input for planning |
| **Inputs** | Raw intent (CAP-005) |
| **Outputs** | Structured requirement set |
| **Responsibilities** | Flag parse failures for human review |
| **Business Rules** | Failed parse routes to human queue |
| **Dependencies** | CAP-005, CAP-007 |
| **Risks** | Misinterpreted requirements |
| **Constraints** | FR-011, NFR-019 |
| **Success Metrics** | Normalization accuracy; human review rate |
| **Owner** | Requirements Product |
| **Future Evolution** | Domain-specific requirement packs |
| **Traceability** | FR-011; MOD-03, MOD-21; PC §6; ADR-003 |

### CAP-007 — Ambiguity & Conflict Detection

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-007 |
| **Name** | Ambiguity & Conflict Detection |
| **Parent Capability** | L1-02 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Identify unclear or contradictory requirements early |
| **Description** | Detect ambiguity, conflicts, and missing information before planning |
| **Business Value** | Reduces rework and failed deliveries |
| **Inputs** | Structured requirements |
| **Outputs** | Ambiguity reports, conflict alerts |
| **Responsibilities** | Escalate unresolved items to humans |
| **Business Rules** | Critical conflicts block plan creation |
| **Dependencies** | CAP-006, CAP-056 |
| **Risks** | False negatives allow bad plans |
| **Constraints** | FR-012 |
| **Success Metrics** | Conflicts caught pre-plan vs post-release |
| **Owner** | Requirements Product |
| **Future Evolution** | Cross-project conflict detection |
| **Traceability** | FR-012; MOD-03; PC §36; ADR-003 |

### CAP-008 — Requirements Traceability

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-008 |
| **Name** | Requirements Traceability |
| **Parent Capability** | L1-02 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Maintain bidirectional links from intent to delivery artifacts |
| **Description** | Link requirements to plans, work, tests, and releases |
| **Business Value** | Compliance and impact analysis |
| **Inputs** | Requirements, plan nodes, work outcomes |
| **Outputs** | Traceability matrix views |
| **Responsibilities** | Preserve trace links through changes |
| **Business Rules** | Production release must trace to requirements |
| **Dependencies** | CAP-006, CAP-009, CAP-031 |
| **Risks** | Broken trace chains |
| **Constraints** | FR-013, FR-014 |
| **Success Metrics** | Trace coverage % at release |
| **Owner** | Requirements Product |
| **Future Evolution** | Automated gap detection |
| **Traceability** | FR-013, FR-014; MOD-03, MOD-19; PC §9; ADR-005 |

### CAP-009 — Delivery Plan Creation

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-009 |
| **Name** | Delivery Plan Creation |
| **Parent Capability** | L1-03 Delivery Planning |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Produce executable delivery plans from requirements |
| **Description** | Generate phased plans with work breakdown and workforce assignments |
| **Business Value** | Predictable path from intent to delivery |
| **Inputs** | Requirements, policies, workforce catalog |
| **Outputs** | Draft delivery plan |
| **Responsibilities** | Ensure plan completeness; detect cycles |
| **Business Rules** | Cyclic dependencies rejected |
| **Dependencies** | CAP-006–008, CAP-023, CAP-018 |
| **Risks** | Over-optimistic plans |
| **Constraints** | FR-020, OBJ-006 |
| **Success Metrics** | Plan acceptance rate; cycle detection 100% |
| **Owner** | Planning Product |
| **Future Evolution** | Multi-scenario plan comparison |
| **Traceability** | FR-020; MOD-04; PC §2; ADR-003 |

### CAP-010 — Dependency & Critical Path Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-010 |
| **Name** | Dependency & Critical Path Management |
| **Parent Capability** | L1-03 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Manage task dependencies and critical path |
| **Description** | Maintain DAG of work items with dependency constraints |
| **Business Value** | Accurate scheduling and risk visibility |
| **Inputs** | Plan structure, dependency rules |
| **Outputs** | Critical path, dependency graph |
| **Responsibilities** | Validate acyclic graph |
| **Business Rules** | No circular dependencies |
| **Dependencies** | CAP-009 |
| **Risks** | Hidden dependencies cause delays |
| **Constraints** | FR-021 |
| **Success Metrics** | Critical path accuracy |
| **Owner** | Planning Product |
| **Future Evolution** | Dynamic critical path updates |
| **Traceability** | FR-021; MOD-04; PC §3; ADR-SAD-001 |

### CAP-011 — Replanning & Change Simulation

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-011 |
| **Name** | Replanning & Change Simulation |
| **Parent Capability** | L1-03 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Adapt plans to change with impact preview |
| **Description** | Replan on scope, failure, or incident; simulate before commit |
| **Business Value** | Resilience to change |
| **Inputs** | Current plan, change triggers |
| **Outputs** | Revised plan, impact analysis |
| **Responsibilities** | Preserve audit of plan versions |
| **Business Rules** | Simulation does not affect active execution until approved |
| **Dependencies** | CAP-009, CAP-004, CAP-053 |
| **Risks** | Frequent replan causes churn |
| **Constraints** | FR-023, FR-024 |
| **Success Metrics** | Replan cycle time; execution disruption minimized |
| **Owner** | Planning Product |
| **Future Evolution** | What-if portfolio simulation |
| **Traceability** | FR-023, FR-024; MOD-04; PC §36; ADR-003 |

### CAP-012 — Plan Baseline Approval

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-012 |
| **Name** | Plan Baseline Approval |
| **Parent Capability** | L1-03 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Authorize plans before execution scheduling |
| **Description** | Human or policy-approved baseline activation |
| **Business Value** | Accountability for delivery commitments |
| **Inputs** | Draft plan, explainability record |
| **Outputs** | Activated plan baseline |
| **Responsibilities** | Block scheduling until approved |
| **Business Rules** | BR-03; explainability required |
| **Dependencies** | CAP-009, CAP-026, CAP-028 |
| **Risks** | Execution without approved plan |
| **Constraints** | FR-022, FR-119 |
| **Success Metrics** | Zero unapproved executions |
| **Owner** | Planning Product |
| **Future Evolution** | Delegated baseline approval |
| **Traceability** | FR-022; MOD-04, MOD-11; PC §36; ADR-003 |

### CAP-013 — Work Scheduling

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-013 |
| **Name** | Work Scheduling |
| **Parent Capability** | L1-04 Work Scheduling & Execution |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Determine when work items are ready for delegation |
| **Description** | Schedule work respecting dependencies, quotas, and policy |
| **Business Value** | Optimal throughput within constraints |
| **Inputs** | Approved plan, workforce availability, budgets |
| **Outputs** | Scheduled work queue |
| **Responsibilities** | Pause on quota or policy deny |
| **Business Rules** | Policy evaluation before schedule |
| **Dependencies** | CAP-012, CAP-025, CAP-047, CAP-019 |
| **Risks** | Starvation or overload |
| **Constraints** | FR-041, NFR-003 |
| **Success Metrics** | Schedule fairness; quota compliance |
| **Owner** | Execution Product |
| **Future Evolution** | Priority-based SLA scheduling |
| **Traceability** | FR-041; MOD-05; PC §25; ADR-SAD-009 |

### CAP-014 — Work Delegation

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-014 |
| **Name** | Work Delegation |
| **Parent Capability** | L1-04 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Assign work to certified specialist contributors |
| **Description** | Delegate scheduled work with context; never generate application code as platform |
| **Business Value** | Core orchestration value |
| **Inputs** | Scheduled work, context package, policy permit |
| **Outputs** | Delegation records |
| **Responsibilities** | Reference registered workforce types only |
| **Business Rules** | BR-01, BR-04; CON-001 |
| **Dependencies** | CAP-013, CAP-020, CAP-025, CAP-050 |
| **Risks** | Delegation to uncertified workforce |
| **Constraints** | FR-040, FR-118, CON-001 |
| **Success Metrics** | Delegation success rate; P95 delegation latency |
| **Owner** | Execution Product |
| **Future Evolution** | Intelligent workforce matching |
| **Traceability** | FR-040, FR-118; MOD-06, MOD-08; PC §2; ADR-007 |

### CAP-015 — Execution Monitoring

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-015 |
| **Name** | Execution Monitoring |
| **Parent Capability** | L1-04 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Track work progress and outcomes |
| **Description** | Monitor delegated work status, progress, and completion |
| **Business Value** | Real-time delivery visibility |
| **Inputs** | Workforce status reports, delegation records |
| **Outputs** | Execution status, progress dashboards |
| **Responsibilities** | Detect stalled or failed work |
| **Business Rules** | All state changes recorded |
| **Dependencies** | CAP-014, CAP-036 |
| **Risks** | Blind spots in execution |
| **Constraints** | FR-043, FR-081 |
| **Success Metrics** | Status freshness; stall detection time |
| **Owner** | Execution Product |
| **Future Evolution** | Predictive delay alerts |
| **Traceability** | FR-043, FR-081; MOD-06, MOD-23; PC §9; ADR-SAD-008 |

### CAP-016 — Failure Remediation

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-016 |
| **Name** | Failure Remediation |
| **Parent Capability** | L1-04 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Recover from failed work items |
| **Description** | Retry, reassign, or escalate failed work per policy |
| **Business Value** | Delivery resilience |
| **Inputs** | Failure signals, retry policies |
| **Outputs** | Retried work, DLQ items, escalations |
| **Responsibilities** | Enforce retry limits |
| **Business Rules** | Excessive retries trigger human review |
| **Dependencies** | CAP-014, CAP-025, CAP-056 |
| **Risks** | Infinite retry loops |
| **Constraints** | FR-044 |
| **Success Metrics** | Recovery rate; DLQ resolution time |
| **Owner** | Execution Product |
| **Future Evolution** | Root-cause classification |
| **Traceability** | FR-044; MOD-06, MOD-27; PC §38; ADR-SAD-004 |

### CAP-017 — Emergency Halt & Recovery

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-017 |
| **Name** | Emergency Halt & Recovery |
| **Parent Capability** | L1-04 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Stop harmful delivery activity immediately |
| **Description** | Halt new delegations and trigger credential revocation; support controlled recovery |
| **Business Value** | Safety valve for autonomous delivery |
| **Inputs** | Halt command, incident signal |
| **Outputs** | Halted state, recovery procedures |
| **Responsibilities** | Halt within SLA; coordinate credential revoke |
| **Business Rules** | POL-04; BR-06 |
| **Dependencies** | CAP-041, CAP-036 |
| **Risks** | Delayed halt amplifies damage |
| **Constraints** | SC-008, FR-042, FR-117 |
| **Success Metrics** | Halt ≤30s; credential revoke ≤60s |
| **Owner** | Operations |
| **Future Evolution** | Scoped halt (project vs tenant) |
| **Traceability** | FR-042, FR-117; MOD-06, MOD-27; PC §38; ADR-SAD-010 |

### CAP-018 — Workforce Registration

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-018 |
| **Name** | Workforce Registration |
| **Parent Capability** | L1-05 Specialist Workforce Coordination |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Register specialist contributor types and instances |
| **Description** | Maintain catalog of workforce types (14+ at GA) with capabilities |
| **Business Value** | Extensible delivery workforce |
| **Inputs** | Workforce manifests, capability declarations |
| **Outputs** | Registered workforce catalog |
| **Responsibilities** | Validate manifests before registration |
| **Business Rules** | Only registered types receive delegation |
| **Dependencies** | CAP-039 |
| **Risks** | Unvetted workforce in catalog |
| **Constraints** | FR-030, NFR-012, CON-008 |
| **Success Metrics** | 14+ types at GA; registration success rate |
| **Owner** | Workforce Platform |
| **Future Evolution** | Third-party marketplace registration |
| **Traceability** | FR-030; MOD-07; PC §28; ADR-007 |

### CAP-019 — Workforce Health Assurance

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-019 |
| **Name** | Workforce Health Assurance |
| **Parent Capability** | L1-05 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Ensure workforce availability before delegation |
| **Description** | Monitor health; pause scheduling for unavailable types |
| **Business Value** | Reliable delegation |
| **Inputs** | Health signals, heartbeat data |
| **Outputs** | Availability status |
| **Responsibilities** | Pause affected types on outage |
| **Business Rules** | Unhealthy types excluded from schedule |
| **Dependencies** | CAP-018, CAP-013 |
| **Risks** | Delegation to failed workforce |
| **Constraints** | FR-031 |
| **Success Metrics** | False delegation rate near zero |
| **Owner** | Workforce Platform |
| **Future Evolution** | Predictive health scoring |
| **Traceability** | FR-031; MOD-07; PC §28; ADR-007 |

### CAP-020 — Delegation Assurance

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-020 |
| **Name** | Delegation Assurance |
| **Parent Capability** | L1-05 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Ensure delegations meet policy and certification rules |
| **Description** | Validate delegation requests against workforce certification and policy |
| **Business Value** | Safe workforce utilization |
| **Inputs** | Delegation request, certification status |
| **Outputs** | Permitted or denied delegation |
| **Responsibilities** | Enforce BR-04 |
| **Business Rules** | Certified workforce only |
| **Dependencies** | CAP-022, CAP-025, CAP-014 |
| **Risks** | Policy bypass at delegation |
| **Constraints** | FR-032, FR-118 |
| **Success Metrics** | Zero delegations to uncertified types |
| **Owner** | Workforce Platform |
| **Future Evolution** | Delegation risk scoring |
| **Traceability** | FR-032; MOD-08; PC §25; ADR-SAD-009 |

### CAP-021 — Output Validation

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-021 |
| **Name** | Output Validation |
| **Parent Capability** | L1-05 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Validate workforce outputs before acceptance |
| **Description** | Verify outputs meet requirements; reject unvalidated model output |
| **Business Value** | Quality and safety of autonomous work |
| **Inputs** | Workforce outputs, validation rules |
| **Outputs** | Accepted or rejected outputs |
| **Responsibilities** | Block unvalidated outputs |
| **Business Rules** | NFR-019 enforcement |
| **Dependencies** | CAP-014, CAP-029 |
| **Risks** | Harmful output accepted |
| **Constraints** | NFR-019 |
| **Success Metrics** | Validation coverage 100% |
| **Owner** | Workforce Platform |
| **Future Evolution** | Domain-specific validators |
| **Traceability** | NFR-019; MOD-21; PC §6; ADR-003 |

### CAP-022 — Workforce Certification

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-022 |
| **Name** | Workforce Certification |
| **Parent Capability** | L1-05 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Certify workforce and connectors for production use |
| **Description** | Certification pipeline for workforce types and integrations |
| **Business Value** | Trust in ecosystem extensions |
| **Inputs** | Certification requests, test results |
| **Outputs** | Certification status |
| **Responsibilities** | Maintain compatibility matrix |
| **Business Rules** | Uncertified excluded from production |
| **Dependencies** | CAP-018, CAP-033 |
| **Risks** | Certified but incompatible workforce |
| **Constraints** | FR-033, FR-034, CON-008 |
| **Success Metrics** | Certification pass rate; compatibility incidents |
| **Owner** | Workforce Platform |
| **Future Evolution** | Partner self-service certification |
| **Traceability** | FR-033, FR-034; MOD-07; PC §28; ADR-007 |

### CAP-023 — Policy Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-023 |
| **Name** | Policy Management |
| **Parent Capability** | L1-06 Policy & Autonomy Governance |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Define tenant delivery policies |
| **Description** | Create and version policy sets for autonomy, gates, and constraints |
| **Business Value** | Customer-specific governance |
| **Inputs** | Policy definitions, regulatory requirements |
| **Outputs** | Active policy sets |
| **Responsibilities** | Version and audit policy changes |
| **Business Rules** | Policy changes recorded in audit |
| **Dependencies** | CAP-039, CAP-036 |
| **Risks** | Misconfigured policies |
| **Constraints** | FR-050, FR-053 |
| **Success Metrics** | Policy coverage per tenant |
| **Owner** | Governance Product |
| **Future Evolution** | Policy templates by industry |
| **Traceability** | FR-050; MOD-09; PC §25; ADR-003 |

### CAP-024 — Autonomy Level Control

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-024 |
| **Name** | Autonomy Level Control |
| **Parent Capability** | L1-06 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Configure how much autonomy is permitted |
| **Description** | Define autonomy profiles per environment and action type |
| **Business Value** | Customer control over AI-assisted delivery |
| **Inputs** | Autonomy profiles, environment definitions |
| **Outputs** | Effective autonomy rules |
| **Responsibilities** | Default to human-required for production |
| **Business Rules** | Production defaults conservative |
| **Dependencies** | CAP-023, CAP-026 |
| **Risks** | Excessive autonomy |
| **Constraints** | FR-051, SRS §42 |
| **Success Metrics** | Autonomy override audit rate |
| **Owner** | Governance Product |
| **Future Evolution** | Per-action granularity |
| **Traceability** | FR-051; MOD-09; PC §36; ADR-003 |

### CAP-025 — Policy Enforcement

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-025 |
| **Name** | Policy Enforcement |
| **Parent Capability** | L1-06 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Enforce policies at decision points |
| **Description** | Evaluate permit/deny at schedule, delegation, and gate points; fail closed |
| **Business Value** | Non-bypassable governance |
| **Inputs** | Policy sets, action context |
| **Outputs** | Permit, deny, obligations |
| **Responsibilities** | Fail closed on evaluation error |
| **Business Rules** | POL-03, BR-01 |
| **Dependencies** | CAP-023, CAP-013, CAP-014, CAP-030 |
| **Risks** | Policy bypass |
| **Constraints** | FR-052; ADR-SAD-009 |
| **Success Metrics** | Zero bypass incidents |
| **Owner** | Governance Product |
| **Future Evolution** | Real-time policy simulation |
| **Traceability** | FR-052; MOD-09; PC §25; ADR-SAD-009 |

### CAP-026 — Approval Orchestration

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-026 |
| **Name** | Approval Orchestration |
| **Parent Capability** | L1-07 Approval & Accountability |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Route human approvals for gated actions |
| **Description** | Manage approval requests, delegations, timeouts |
| **Business Value** | Human accountability |
| **Inputs** | Approval triggers, approver roles |
| **Outputs** | Granted or denied approvals |
| **Responsibilities** | Block action until resolved |
| **Business Rules** | POL-01 |
| **Dependencies** | CAP-024, CAP-039 |
| **Risks** | Approval bottlenecks |
| **Constraints** | FR-052, OBJ-004 |
| **Success Metrics** | Approval cycle time |
| **Owner** | Governance Product |
| **Future Evolution** | Mobile approval channels |
| **Traceability** | FR-052; MOD-11; PC §36; ADR-003 |

### CAP-027 — Segregation of Duties

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-027 |
| **Name** | Segregation of Duties |
| **Parent Capability** | L1-07 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Prevent self-approval conflicts |
| **Description** | Enforce SoD rules on approvals and sensitive actions |
| **Business Value** | Fraud and error prevention |
| **Inputs** | Actor identity, action type |
| **Outputs** | SoD permit or deny |
| **Responsibilities** | Block self-approval |
| **Business Rules** | POL-02 |
| **Dependencies** | CAP-026, CAP-039 |
| **Risks** | SoD circumvention |
| **Constraints** | FR-054 |
| **Success Metrics** | SoD violation attempts blocked 100% |
| **Owner** | Governance Product |
| **Future Evolution** | Custom SoD rule builder |
| **Traceability** | FR-054; MOD-11; PC §36; ADR-003 |

### CAP-028 — Decision Explainability

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-028 |
| **Name** | Decision Explainability |
| **Parent Capability** | L1-07 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Record human-readable rationale for key decisions |
| **Description** | Capture explainability for plans, approvals, and production decisions |
| **Business Value** | Trust and audit support |
| **Inputs** | Decision context, influencing factors |
| **Outputs** | Explainability records |
| **Responsibilities** | Attach to auditable decisions |
| **Business Rules** | BR-05 |
| **Dependencies** | CAP-012, CAP-026, CAP-036 |
| **Risks** | Opaque decisions erode trust |
| **Constraints** | FR-119 |
| **Success Metrics** | Explainability coverage on required decisions |
| **Owner** | Governance Product |
| **Future Evolution** | Stakeholder-tailored summaries |
| **Traceability** | FR-119; MOD-11, MOD-12; PC §6; ADR-003 |

### CAP-029 — Quality Gate Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-029 |
| **Name** | Quality Gate Management |
| **Parent Capability** | L1-08 Quality & Release Assurance |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Define and evaluate objective quality gates |
| **Description** | Configure gates (tests, security scans, reviews); evaluate pass/fail |
| **Business Value** | Objective release quality |
| **Inputs** | Gate definitions, CI/test results |
| **Outputs** | Gate results |
| **Responsibilities** | Block promotion on fail |
| **Business Rules** | BR-02 |
| **Dependencies** | CAP-035, CAP-025 |
| **Risks** | Gate bypass |
| **Constraints** | FR-060, FR-061 |
| **Success Metrics** | Gate pass rate; false pass rate |
| **Owner** | Quality Product |
| **Future Evolution** | Custom gate types |
| **Traceability** | FR-060; MOD-10; PC §38; ADR-SAD-001 |

### CAP-030 — Environment Promotion Control

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-030 |
| **Name** | Environment Promotion Control |
| **Parent Capability** | L1-08 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Control movement between environments |
| **Description** | Govern dev → staging → production promotions |
| **Business Value** | Safe progressive delivery |
| **Inputs** | Promotion requests, gate results |
| **Outputs** | Promotion permit or deny |
| **Responsibilities** | Require gates and policy |
| **Business Rules** | No promotion without gate pass |
| **Dependencies** | CAP-029, CAP-025 |
| **Risks** | Unauthorized production change |
| **Constraints** | FR-062 |
| **Success Metrics** | Zero ungated promotions |
| **Owner** | Quality Product |
| **Future Evolution** | Blue-green promotion patterns |
| **Traceability** | FR-062; MOD-10; PC §38; ADR-003 |

### CAP-031 — Release Authorization

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-031 |
| **Name** | Release Authorization |
| **Parent Capability** | L1-08 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Authorize production releases |
| **Description** | Final human or policy gate for production deployment |
| **Business Value** | Controlled go-live |
| **Inputs** | Gate results, approvals, traceability |
| **Outputs** | Release authorization |
| **Responsibilities** | Require traceability to requirements |
| **Business Rules** | POL-01; production requires authorization |
| **Dependencies** | CAP-029, CAP-030, CAP-026, CAP-008 |
| **Risks** | Unauthorized production release |
| **Constraints** | OBJ-004 |
| **Success Metrics** | 100% releases with authorization record |
| **Owner** | Quality Product |
| **Future Evolution** | Canary release governance |
| **Traceability** | FR-063; MOD-10; PC §36; ADR-003 |

### CAP-032 — Gate Exception Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-032 |
| **Name** | Gate Exception Management |
| **Parent Capability** | L1-08 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Manage waivers for failed gates |
| **Description** | Time-bound waivers with approval and audit |
| **Business Value** | Controlled exceptions without bypass |
| **Inputs** | Waiver requests, gate failures |
| **Outputs** | Approved waivers |
| **Responsibilities** | Expire waivers; audit all |
| **Business Rules** | Waiver requires approval |
| **Dependencies** | CAP-029, CAP-026, CAP-036 |
| **Risks** | Waiver abuse |
| **Constraints** | FR-063 |
| **Success Metrics** | Waiver rate; post-waiver incident rate |
| **Owner** | Quality Product |
| **Future Evolution** | Risk-scored waiver workflow |
| **Traceability** | FR-063; MOD-10; PC §36; ADR-003 |

### CAP-033 — Connection Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-033 |
| **Name** | Connection Management |
| **Parent Capability** | L1-09 Enterprise Connectivity |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Connect to customer SDLC tools |
| **Description** | Register and manage VCS, CI, ticket system connections |
| **Business Value** | Fits enterprise toolchain |
| **Inputs** | Connection credentials, connector config |
| **Outputs** | Active integrations |
| **Responsibilities** | Certify connectors |
| **Business Rules** | Certified connectors for production |
| **Dependencies** | CAP-022, CAP-041 |
| **Risks** | Broken integrations |
| **Constraints** | FR-070, FR-034, CON-005 |
| **Success Metrics** | Connection uptime |
| **Owner** | Integrations Product |
| **Future Evolution** | Self-service connector SDK |
| **Traceability** | FR-070; MOD-15; PC §3; ADR-007 |

### CAP-034 — Work Item Synchronization

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-034 |
| **Name** | Work Item Synchronization |
| **Parent Capability** | L1-09 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Bidirectional sync with issue trackers |
| **Description** | Sync tickets, status, and comments with external systems |
| **Business Value** | Single pane for stakeholders |
| **Inputs** | External tickets, internal work items |
| **Outputs** | Synchronized records |
| **Responsibilities** | Resolve sync conflicts |
| **Business Rules** | Tenant-scoped sync |
| **Dependencies** | CAP-033, CAP-005 |
| **Risks** | Sync drift |
| **Constraints** | FR-071, FR-074, SC-007 |
| **Success Metrics** | Sync fidelity; conflict resolution time |
| **Owner** | Integrations Product |
| **Future Evolution** | Multi-tracker aggregation |
| **Traceability** | FR-071, FR-074; MOD-17; PC §3; ADR-005 |

### CAP-035 — SDLC Pipeline Awareness

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-035 |
| **Name** | SDLC Pipeline Awareness |
| **Parent Capability** | L1-09 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Ingest CI/CD and repository signals |
| **Description** | Awareness of builds, commits, pipeline status for gates and planning |
| **Business Value** | Closed-loop quality |
| **Inputs** | VCS events, CI results |
| **Outputs** | Pipeline status for gates |
| **Responsibilities** | Feed gate evaluation |
| **Business Rules** | Read-only awareness; no direct deploy |
| **Dependencies** | CAP-033, CAP-029 |
| **Risks** | Stale pipeline data |
| **Constraints** | FR-072, FR-073 |
| **Success Metrics** | Signal freshness |
| **Owner** | Integrations Product |
| **Future Evolution** | Multi-cloud pipeline support |
| **Traceability** | FR-072, FR-073; MOD-15; PC §3; ADR-005 |

### CAP-036 — Immutable Activity Record

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-036 |
| **Name** | Immutable Activity Record |
| **Parent Capability** | L1-10 Audit & Compliance Evidence |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Record all delivery decisions immutably |
| **Description** | Append-only audit of state transitions and decisions |
| **Business Value** | Reconstructable history |
| **Inputs** | All capability decision events |
| **Outputs** | Audit trail |
| **Responsibilities** | Tenant-boundary enforcement on queries |
| **Business Rules** | No deletion; legal hold exception |
| **Dependencies** | All core capabilities (consumers) |
| **Risks** | Audit gaps |
| **Constraints** | FR-080, FR-121, SC-003 |
| **Success Metrics** | 100% replay reconstructability |
| **Owner** | Compliance Engineering |
| **Future Evolution** | Real-time audit streaming |
| **Traceability** | FR-080, FR-121; MOD-12; PC §9; ADR-006 |

### CAP-037 — Compliance Evidence Export

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-037 |
| **Name** | Compliance Evidence Export |
| **Parent Capability** | L1-10 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Export audit packages for auditors |
| **Description** | Generate compliance evidence bundles (SOC 2, GDPR) |
| **Business Value** | Audit readiness |
| **Inputs** | Audit records, export criteria |
| **Outputs** | Evidence packages |
| **Responsibilities** | Automated export per CON-007 |
| **Business Rules** | Export scoped to tenant |
| **Dependencies** | CAP-036 |
| **Risks** | Incomplete evidence |
| **Constraints** | FR-082, CON-007, NFR-009 |
| **Success Metrics** | Auditor acceptance rate |
| **Owner** | Compliance Engineering |
| **Future Evolution** | Live attestation (Year 3) |
| **Traceability** | FR-082; MOD-12; PC §9; ADR-006 |

### CAP-038 — Legal Hold Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-038 |
| **Name** | Legal Hold Management |
| **Parent Capability** | L1-10 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Preserve records under legal hold |
| **Description** | Suspend deletion when hold active |
| **Business Value** | Litigation readiness |
| **Inputs** | Hold orders |
| **Outputs** | Hold status on records |
| **Responsibilities** | Override retention deletion |
| **Business Rules** | Hold blocks purge |
| **Dependencies** | CAP-036, CAP-045 |
| **Risks** | Accidental deletion under hold |
| **Constraints** | FR-103 |
| **Success Metrics** | Zero hold violations |
| **Owner** | Compliance Engineering |
| **Future Evolution** | Hold scope automation |
| **Traceability** | FR-103; MOD-19; PC §9; ADR-006 |

### CAP-039 — Identity & Access Governance

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-039 |
| **Name** | Identity & Access Governance |
| **Parent Capability** | L1-11 Security & Trust |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Govern who can access platform functions |
| **Description** | SSO, RBAC, role assignments |
| **Business Value** | Enterprise identity integration |
| **Inputs** | IdP assertions, role definitions |
| **Outputs** | Access decisions |
| **Responsibilities** | Enforce least privilege |
| **Business Rules** | Enterprise SSO required |
| **Dependencies** | Customer IdP |
| **Risks** | Over-privileged access |
| **Constraints** | FR-091, CON-002 |
| **Success Metrics** | Access review compliance |
| **Owner** | Security Engineering |
| **Future Evolution** | ABAC extensions |
| **Traceability** | FR-091; MOD-13; PC §8; ADR-SAD-007 |

### CAP-040 — Tenant Isolation Assurance

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-040 |
| **Name** | Tenant Isolation Assurance |
| **Parent Capability** | L1-11 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Ensure strict multi-tenant separation |
| **Description** | Guarantee no cross-tenant data or action leakage |
| **Business Value** | Enterprise trust |
| **Inputs** | Tenant context on all operations |
| **Outputs** | Isolated data views and actions |
| **Responsibilities** | POL-06 enforcement |
| **Business Rules** | tenant_id on all operations |
| **Dependencies** | CAP-039 |
| **Risks** | Cross-tenant leak |
| **Constraints** | FR-090, OBJ-001 |
| **Success Metrics** | Zero cross-tenant incidents |
| **Owner** | Security Engineering |
| **Future Evolution** | Dedicated tenant silos |
| **Traceability** | FR-090; MOD-13; PC §8; ADR-SAD-006 |

### CAP-041 — Credential Stewardship

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-041 |
| **Name** | Credential Stewardship |
| **Parent Capability** | L1-11 (shared with L1-05) |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Securely manage and revoke credentials |
| **Description** | Vault credentials; revoke on halt within SLA |
| **Business Value** | Blast radius limitation |
| **Inputs** | Credential references, halt signals |
| **Outputs** | Issued/revoked credentials |
| **Responsibilities** | Revoke ≤60s on halt |
| **Business Rules** | BR-06 |
| **Dependencies** | CAP-017 |
| **Risks** | Lingering credentials post-halt |
| **Constraints** | FR-117, NFR-006, NFR-007 |
| **Success Metrics** | Revocation SLA compliance |
| **Owner** | Security Engineering |
| **Future Evolution** | Just-in-time credentials |
| **Traceability** | FR-117; MOD-14; PC §8; ADR-SAD-010 |

### CAP-042 — Data Residency Control

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-042 |
| **Name** | Data Residency Control |
| **Parent Capability** | L1-11 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Pin tenant data to approved regions |
| **Description** | Enforce region pinning for data and processing |
| **Business Value** | Regulatory compliance |
| **Inputs** | Tenant residency policy |
| **Outputs** | Region-bound operations |
| **Responsibilities** | Reject cross-region processing |
| **Business Rules** | Data stays in pinned region |
| **Dependencies** | CAP-040, CAP-055 |
| **Risks** | Residency violation |
| **Constraints** | FR-120 |
| **Success Metrics** | Zero residency violations |
| **Owner** | Security Engineering |
| **Future Evolution** | Multi-region active-active (policy-gated) |
| **Traceability** | FR-120; MOD-13; PC §8; ADR-SAD-006 |

### CAP-043 — Data Minimization

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-043 |
| **Name** | Data Minimization |
| **Parent Capability** | L1-12 Privacy & Data Stewardship |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Minimize PII in delivery processing |
| **Description** | Redact and limit personal data exposure |
| **Business Value** | Privacy by design |
| **Inputs** | Data classification rules |
| **Outputs** | Minimized data sets |
| **Responsibilities** | Apply minimization before AI processing |
| **Business Rules** | PII only when necessary |
| **Dependencies** | CAP-050 |
| **Risks** | Over-exposure of PII |
| **Constraints** | NFR-008 |
| **Success Metrics** | PII field reduction rate |
| **Owner** | Privacy Officer |
| **Future Evolution** | Automated classification |
| **Traceability** | NFR-008; MOD-19; PC §9; ADR-006 |

### CAP-044 — Data Subject Rights Support

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-044 |
| **Name** | Data Subject Rights Support |
| **Parent Capability** | L1-12 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Fulfill GDPR data subject requests |
| **Description** | Access, rectification, erasure workflows |
| **Business Value** | GDPR readiness |
| **Inputs** | DSR requests |
| **Outputs** | Fulfilled DSR responses |
| **Responsibilities** | SLA-05 compliance |
| **Business Rules** | Legal hold overrides erasure |
| **Dependencies** | CAP-038, CAP-045 |
| **Risks** | Missed DSR deadline |
| **Constraints** | NFR-010, FR-094 |
| **Success Metrics** | DSR fulfillment ≤30 days |
| **Owner** | Privacy Officer |
| **Future Evolution** | Self-service DSR portal |
| **Traceability** | FR-094; MOD-25; PC §9; ADR-006 |

### CAP-045 — Retention & Deletion Governance

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-045 |
| **Name** | Retention & Deletion Governance |
| **Parent Capability** | L1-12 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Govern data lifecycle per policy |
| **Description** | Retention schedules, automated purge, hold respect |
| **Business Value** | Compliance and cost control |
| **Inputs** | Retention policies |
| **Outputs** | Purged or archived data |
| **Responsibilities** | Respect legal hold |
| **Business Rules** | Hold blocks deletion |
| **Dependencies** | CAP-038 |
| **Risks** | Over-retention or premature deletion |
| **Constraints** | FR-093 |
| **Success Metrics** | Retention policy compliance |
| **Owner** | Privacy Officer |
| **Future Evolution** | Tiered storage lifecycle |
| **Traceability** | FR-093; MOD-25; PC §9; ADR-006 |

### CAP-046 — Cost Accounting

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-046 |
| **Name** | Cost Accounting |
| **Parent Capability** | L1-13 Financial Stewardship |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Account for delivery resource consumption |
| **Description** | Roll up costs by project, workforce, and tenant |
| **Business Value** | Financial visibility |
| **Inputs** | Resource consumption events |
| **Outputs** | Cost reports, rollups |
| **Responsibilities** | Accurate attribution |
| **Business Rules** | Tenant-scoped accounting |
| **Dependencies** | CAP-014, CAP-015 |
| **Risks** | Cost blind spots |
| **Constraints** | FR-083 |
| **Success Metrics** | Cost attribution accuracy |
| **Owner** | FinOps |
| **Future Evolution** | Chargeback integration |
| **Traceability** | FR-083; MOD-22; PC §3; ADR-SAD-001 |

### CAP-047 — Budget Control

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-047 |
| **Name** | Budget Control |
| **Parent Capability** | L1-13 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C1 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Stop work when budget exceeded |
| **Description** | Enforce per-tenant and per-project budgets |
| **Business Value** | Margin protection |
| **Inputs** | Budget limits, consumption |
| **Outputs** | Budget stop signals |
| **Responsibilities** | Stop ≤60s on breach |
| **Business Rules** | POL-05 |
| **Dependencies** | CAP-046, CAP-013 |
| **Risks** | Runaway cost |
| **Constraints** | FR-084, NFR-020, SC-006 |
| **Success Metrics** | Budget stop SLA |
| **Owner** | FinOps |
| **Future Evolution** | Predictive budget alerts |
| **Traceability** | FR-084; MOD-22; PC §3; ADR-SAD-001 |

### CAP-048 — Cost Anomaly Awareness

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-048 |
| **Name** | Cost Anomaly Awareness |
| **Parent Capability** | L1-13 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Detect unusual spending patterns |
| **Description** | Alert on cost anomalies before budget breach |
| **Business Value** | Early financial risk detection |
| **Inputs** | Cost time series |
| **Outputs** | Anomaly alerts |
| **Responsibilities** | Notify stakeholders |
| **Business Rules** | Alerts tenant-scoped |
| **Dependencies** | CAP-046, CAP-056 |
| **Risks** | Late detection |
| **Constraints** | FR-084 |
| **Success Metrics** | Mean time to anomaly detection |
| **Owner** | FinOps |
| **Future Evolution** | ML-based forecasting |
| **Traceability** | FR-084; MOD-23; PC §3; ADR-SAD-008 |

### CAP-049 — Delivery Knowledge Management

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-049 |
| **Name** | Delivery Knowledge Management |
| **Parent Capability** | L1-14 Knowledge & Traceability |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Preserve organizational delivery knowledge |
| **Description** | Maintain knowledge graph of projects, decisions, artifacts |
| **Business Value** | Institutional memory |
| **Inputs** | Delivery artifacts, decisions |
| **Outputs** | Searchable knowledge |
| **Responsibilities** | Tenant isolation of knowledge |
| **Business Rules** | Knowledge scoped to tenant |
| **Dependencies** | CAP-008, CAP-036 |
| **Risks** | Knowledge silos |
| **Constraints** | FR-100 |
| **Success Metrics** | Knowledge retrieval success |
| **Owner** | Knowledge Product |
| **Future Evolution** | Cross-project pattern mining |
| **Traceability** | FR-100; MOD-19; PC §9; ADR-005 |

### CAP-050 — Work Context Provision

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-050 |
| **Name** | Work Context Provision |
| **Parent Capability** | L1-14 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Assemble relevant context for delegated work |
| **Description** | Package requirements, history, and constraints for workforce |
| **Business Value** | Quality of delegated work |
| **Inputs** | Requirements, knowledge, policies |
| **Outputs** | Context packages |
| **Responsibilities** | Apply minimization (CAP-043) |
| **Business Rules** | Context complete but minimized |
| **Dependencies** | CAP-049, CAP-008, CAP-043 |
| **Risks** | Insufficient context |
| **Constraints** | FR-101 |
| **Success Metrics** | Workforce rework rate |
| **Owner** | Knowledge Product |
| **Future Evolution** | Adaptive context sizing |
| **Traceability** | FR-101; MOD-19; PC §6; ADR-005 |

### CAP-051 — Delivery Conflict Resolution

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-051 |
| **Name** | Delivery Conflict Resolution |
| **Parent Capability** | L1-14 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Resolve conflicts between concurrent delivery changes |
| **Description** | Detect and resolve conflicting work outcomes |
| **Business Value** | Coherent multi-contributor delivery |
| **Inputs** | Conflicting outputs, merge rules |
| **Outputs** | Resolved artifacts or escalations |
| **Responsibilities** | Escalate unresolvable conflicts |
| **Business Rules** | Human escalation for critical conflicts |
| **Dependencies** | CAP-021, CAP-026 |
| **Risks** | Silent overwrite |
| **Constraints** | FR-110, FR-111, FR-112 |
| **Success Metrics** | Conflict resolution rate |
| **Owner** | Knowledge Product |
| **Future Evolution** | Semantic merge assistance |
| **Traceability** | FR-110–112; MOD-28; PC §36; ADR-007 |

### CAP-052 — Performance Insight

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-052 |
| **Name** | Performance Insight |
| **Parent Capability** | L1-15 Operational Assurance |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Provide dashboards and delivery metrics |
| **Description** | Executive and operator views of delivery health |
| **Business Value** | Operational visibility |
| **Inputs** | Execution, cost, gate data |
| **Outputs** | Dashboards, reports |
| **Responsibilities** | Tenant-scoped views |
| **Business Rules** | Read-only insight |
| **Dependencies** | CAP-015, CAP-046, CAP-036 |
| **Risks** | Misleading aggregates |
| **Constraints** | FR-081, NFR-001 |
| **Success Metrics** | Dashboard freshness |
| **Owner** | Operations |
| **Future Evolution** | Custom KPI builder |
| **Traceability** | FR-081; MOD-23; PC §9; ADR-SAD-008 |

### CAP-053 — Incident Response Loop

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-053 |
| **Name** | Incident Response Loop |
| **Parent Capability** | L1-15 |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Convert production incidents into remedial work |
| **Description** | Ingest incidents; create and prioritize fix work |
| **Business Value** | Closed-loop reliability |
| **Inputs** | Incident signals, monitoring alerts |
| **Outputs** | Incident-linked work items |
| **Responsibilities** | Link to audit trail |
| **Business Rules** | Critical incidents escalate |
| **Dependencies** | CAP-011, CAP-056 |
| **Risks** | Slow incident response |
| **Constraints** | FR-116 |
| **Success Metrics** | Incident-to-work creation time |
| **Owner** | Operations |
| **Future Evolution** | Auto-severity classification |
| **Traceability** | FR-116; MOD-06; PC §38; ADR-SAD-004 |

### CAP-054 — Post-Release Maintenance Coordination

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-054 |
| **Name** | Post-Release Maintenance Coordination |
| **Parent Capability** | L1-15 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Coordinate ongoing maintenance after release |
| **Description** | Schedule patches, updates, and technical debt work |
| **Business Value** | Long-term software health |
| **Inputs** | Released systems, maintenance policies |
| **Outputs** | Maintenance work plans |
| **Responsibilities** | Integrate with incident loop |
| **Business Rules** | Maintenance work governed like delivery |
| **Dependencies** | CAP-031, CAP-053 |
| **Risks** | Neglected post-release systems |
| **Constraints** | FR-115 |
| **Success Metrics** | Maintenance backlog age |
| **Owner** | Operations |
| **Future Evolution** | Predictive maintenance scheduling |
| **Traceability** | FR-115; MOD-06; PC §2; ADR-SAD-001 |

### CAP-055 — Tenant Configuration

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-055 |
| **Name** | Tenant Configuration |
| **Parent Capability** | L1-16 Platform Administration |
| **Child Capabilities** | None |
| **Priority** | P1 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Configure tenant-specific platform behavior |
| **Description** | Settings for integrations, policies, limits, deployment profile |
| **Business Value** | Customer fit without forks |
| **Inputs** | Admin configuration |
| **Outputs** | Tenant configuration state |
| **Responsibilities** | No per-customer core forks |
| **Business Rules** | CON-006 |
| **Dependencies** | CAP-039, CAP-040 |
| **Risks** | Misconfiguration |
| **Constraints** | CON-005, CON-006 |
| **Success Metrics** | Config change success rate |
| **Owner** | Platform Admin |
| **Future Evolution** | Configuration templates |
| **Traceability** | FR-092; MOD-24; PC §3; ADR-SAD-001 |

### CAP-056 — Stakeholder Notification

| Field | Value |
|-------|-------|
| **Unique Identifier** | CAP-056 |
| **Name** | Stakeholder Notification |
| **Parent Capability** | L1-16 |
| **Child Capabilities** | None |
| **Priority** | P2 |
| **Criticality** | C2 |
| **Maturity** | 1 |
| **Future Status** | GA |
| **Purpose** | Notify humans of events requiring attention |
| **Description** | Escalations for approvals, failures, anomalies, conflicts |
| **Business Value** | Timely human response |
| **Inputs** | Event triggers, notification preferences |
| **Outputs** | Delivered notifications |
| **Responsibilities** | Respect quiet hours and channels |
| **Business Rules** | Critical events always notify |
| **Dependencies** | CAP-039 |
| **Risks** | Alert fatigue or missed alerts |
| **Constraints** | FR-081 (status visibility) |
| **Success Metrics** | Notification delivery rate |
| **Owner** | Platform Admin |
| **Future Evolution** | Multi-channel orchestration |
| **Traceability** | FR-081; MOD-24; PC §36; ADR-SAD-001 |

---

## Capability Traceability Matrix

### Capability → SRS Requirements

| CAP ID | SRS Requirements |
|--------|------------------|
| CAP-001 | FR-001, FR-005 |
| CAP-002 | FR-003, FR-122 |
| CAP-003 | FR-002 |
| CAP-004 | FR-004 |
| CAP-005 | FR-010 |
| CAP-006 | FR-011 |
| CAP-007 | FR-012 |
| CAP-008 | FR-013, FR-014 |
| CAP-009 | FR-020 |
| CAP-010 | FR-021 |
| CAP-011 | FR-023, FR-024 |
| CAP-012 | FR-022, FR-119 |
| CAP-013 | FR-041 |
| CAP-014 | FR-040, FR-118 |
| CAP-015 | FR-043, FR-081 |
| CAP-016 | FR-044 |
| CAP-017 | FR-042, FR-117 |
| CAP-018 | FR-030 |
| CAP-019 | FR-031 |
| CAP-020 | FR-032 |
| CAP-021 | NFR-019 |
| CAP-022 | FR-033, FR-034 |
| CAP-023 | FR-050, FR-053 |
| CAP-024 | FR-051 |
| CAP-025 | FR-052 |
| CAP-026 | FR-052 |
| CAP-027 | FR-054 |
| CAP-028 | FR-119 |
| CAP-029 | FR-060, FR-061 |
| CAP-030 | FR-062 |
| CAP-031 | FR-063, OBJ-004 |
| CAP-032 | FR-063 |
| CAP-033 | FR-070 |
| CAP-034 | FR-071, FR-074 |
| CAP-035 | FR-072, FR-073 |
| CAP-036 | FR-080, FR-121 |
| CAP-037 | FR-082 |
| CAP-038 | FR-103 |
| CAP-039 | FR-091 |
| CAP-040 | FR-090 |
| CAP-041 | FR-117 |
| CAP-042 | FR-120 |
| CAP-043 | NFR-008 |
| CAP-044 | FR-094, NFR-010 |
| CAP-045 | FR-093 |
| CAP-046 | FR-083 |
| CAP-047 | FR-084, NFR-020 |
| CAP-048 | FR-084 |
| CAP-049 | FR-100 |
| CAP-050 | FR-101 |
| CAP-051 | FR-110, FR-111, FR-112 |
| CAP-052 | FR-081, NFR-001 |
| CAP-053 | FR-116 |
| CAP-054 | FR-115 |
| CAP-055 | FR-092 |
| CAP-056 | FR-081 |

**SRS coverage:** All FR-001–FR-122 (except FR-102 mapped via CAP-049/050), all SC-001–SC-008, all CON-001–CON-008, and relevant NFRs mapped.

### Capability → SAD Modules

| CAP ID | Primary SAD Modules |
|--------|---------------------|
| CAP-001–004 | MOD-02 |
| CAP-005–008 | MOD-03, MOD-19 |
| CAP-009–012 | MOD-04, MOD-20 |
| CAP-013 | MOD-05 |
| CAP-014–017 | MOD-06, MOD-08, MOD-27 |
| CAP-018–022 | MOD-07, MOD-08 |
| CAP-023–025 | MOD-09 |
| CAP-026–028 | MOD-11, MOD-12 |
| CAP-029–032 | MOD-10 |
| CAP-033–035 | MOD-15, MOD-17 |
| CAP-036–037 | MOD-12 |
| CAP-038, CAP-049–051 | MOD-19, MOD-28 |
| CAP-039–042 | MOD-13, MOD-14, MOD-25 |
| CAP-043–045 | MOD-19, MOD-25 |
| CAP-046–048 | MOD-22, MOD-23 |
| CAP-052 | MOD-23, MOD-30 |
| CAP-053–054 | MOD-06 |
| CAP-055–056 | MOD-24 |
| All | MOD-01 (edge), MOD-18 (events), MOD-30 (performance) — supporting |

**Module coverage:** MOD-01 through MOD-30 represented.

### Capability → Constitution Principles

| CAP Group | PC Sections |
|-----------|-------------|
| CAP-001–016 | §2 Vision, §3 Architecture, §36 Human Sovereignty |
| CAP-017, CAP-041 | §38 Emergency Controls |
| CAP-018–022 | §28 Agent Certification |
| CAP-023–032 | §25 Policy Fail-Closed, §36–38 Governance |
| CAP-033–035 | §3 Architecture (integration) |
| CAP-036–045 | §8 Security, §9 Audit & Compliance |
| CAP-046–048 | §3 Architecture (FinOps) |
| CAP-049–051 | §6 AI Safety, §9 Knowledge |
| CAP-052–054 | §9 Observability |
| CAP-055–056 | §3 Configuration not forks |

### Capability → ADRs

| CAP Group | ADRs |
|-----------|------|
| CAP-014, CAP-018–022 | ADR-007 (no agent-to-agent) |
| CAP-025, CAP-013–014 | ADR-SAD-009 (fail-closed PEP) |
| CAP-017, CAP-041 | ADR-SAD-010 (halt credential revoke) |
| CAP-040, CAP-042 | ADR-SAD-006 (tenant sharding) |
| CAP-039 | ADR-SAD-007 (zero trust) |
| CAP-036–037 | ADR-006 (audit), ADR-SAD-008 (CQRS) |
| CAP-001–056 (platform) | ADR-SAD-001 (five DUs) |
| CAP-023–028 | ADR-003 (human gates) |
| CAP-009–012 | ADR-005 (authoritative state) |
| CAP-033–035 | ADR-007 (ecosystem) |
| BCM process | ADR-GOV-002 (BCM before DDD) |

---

## Validation Audits

### Business Completeness Audit

| Check | Result | Notes |
|-------|--------|-------|
| Mission represented | **PASS** | §2, L1-04, L1-05 |
| Vision (control plane) | **PASS** | CON-001 in CAP-014 |
| All stakeholder needs | **PASS** | §3, §18 |
| Out-of-scope excluded | **PASS** | §24 boundaries |
| Golden path covered | **PASS** | §22 Intent-to-Release |

### Capability Coverage Audit

| SRS domain | Capabilities | Result |
|------------|--------------|--------|
| Project (FR-001–005) | CAP-001–004 | **PASS** |
| Requirements (FR-010–014) | CAP-005–008 | **PASS** |
| Planning (FR-020–024) | CAP-009–012 | **PASS** |
| Agents (FR-030–034) | CAP-018–022 | **PASS** |
| Execution (FR-040–044) | CAP-013–017 | **PASS** |
| Policy (FR-050–054) | CAP-023–027 | **PASS** |
| Gates (FR-060–063) | CAP-029–032 | **PASS** |
| Integration (FR-070–074) | CAP-033–035 | **PASS** |
| Audit (FR-080–084) | CAP-036–037, CAP-046–048 | **PASS** |
| Security (FR-090–094) | CAP-039–045 | **PASS** |
| Knowledge (FR-100–103) | CAP-049–050, CAP-038 | **PASS** |
| Conflict (FR-110–112) | CAP-051 | **PASS** |
| Maintenance (FR-115–116) | CAP-053–054 | **PASS** |
| Halt/credentials (FR-117–122) | CAP-017, CAP-041, CAP-042, CAP-036 | **PASS** |

### Duplicate Capability Audit

| Check | Result |
|-------|--------|
| Unique CAP IDs | **PASS** — 56 unique |
| Overlapping scope | **PASS** — CAP-014 (delegation) vs CAP-020 (assurance) distinct: execution vs governance |
| CAP-041 dual parent | **PASS** — documented shared capability |

### Missing Capability Audit

| Potential gap | Resolution | Result |
|-------------|------------|--------|
| FR-102 legal hold context | CAP-038 + CAP-049 | **PASS** |
| Explainability | CAP-028 | **PASS** |
| Air-gapped | CAP-055, CAP-033 constraints | **PASS** |
| Notification | CAP-056 added | **PASS** |

### Dependency Audit

| Check | Result |
|-------|--------|
| Circular dependencies | **PASS** — DAG validated |
| Orphan capabilities | **PASS** — all in L1 hierarchy |
| Critical path deps | **PASS** — §20 documented |

### Boundary Audit

| Check | Result |
|-------|--------|
| No implementation terms as capabilities | **PASS** |
| No API/DB/entity definitions | **PASS** |
| CON-001 respected | **PASS** |

### Governance Audit

| Check | Result |
|-------|--------|
| Human oversight | CAP-026–028, CAP-031 |
| Fail-closed policy | CAP-025 |
| Audit completeness | CAP-036 |
| SoD | CAP-027 |
| **Result** | **PASS** |

### Traceability Audit

| Check | Result |
|-------|--------|
| All CAPs have SRS mapping | **PASS** |
| All CAPs have module mapping | **PASS** |
| All CAPs have PC reference | **PASS** |
| All CAPs have ADR where applicable | **PASS** |
| Orphan requirements | **PASS** — FR-102 via CAP-049/038; NFRs via supporting caps |

### Future Expansion Audit

| Check | Result |
|-------|--------|
| Roadmap in §47 | **PASS** |
| Marketplace | CAP-022 evolution |
| No premature capabilities | **PASS** |

### Consistency Audit

| Check | Result |
|-------|--------|
| Terminology vs SRS/Glossary | **PASS** — workforce not agent in business layer |
| Hierarchy matches catalog | **PASS** |
| KPIs align with SC/NFR | **PASS** |
| **Result** | **PASS** |

**All audits: ZERO issues remaining.**

---

## Document Control

| Field | Value |
|-------|-------|
| Author | AIPM Business Architecture |
| Reviewers | Product, Governance, Security, Compliance |
| Approver | Engineering Blueprint Authority |
| Next document | `05-Domain-Model/` (blocked until this BCM approved) |
| References | SRS-AIPM-001, SAD-AIPM-001, PC-AIPM-001, RTM-AIPM-001, ADR-GOV-002 |

---

**BUSINESS CAPABILITY MODEL STATUS: APPROVED**

