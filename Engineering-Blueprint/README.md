# AI Project Manager — Engineering Blueprint

**Repository:** Engineering-Blueprint  
**Product:** AI Project Manager (AIPM)  
**Bundle version:** 1.8.0  
**Last updated:** 2026-07-07

---

## Project overview

AI Project Manager (AIPM) is the **control plane** for an AI software company—a production-grade, multi-tenant platform that coordinates requirements analysis, planning, development, testing, deployment, documentation, monitoring, and maintenance through 14+ specialized contributors.

AIPM does not generate customer application code. It maintains authoritative delivery state, enforces policies and quality gates, manages human approvals, integrates with enterprise SDLC tools, and produces audit-grade compliance evidence.

This repository is the **single engineering documentation foundation** for the platform.

---

## Purpose of this repository

| Goal | Description |
|------|-------------|
| **Authoritative specifications** | Lock approved requirements and architecture as immutable baselines. |
| **Constitutional law** | Encode non-negotiable rules that prevent contradictions at scale. |
| **Layered enterprise architecture** | Business → Information → Application → Technology, in order. |
| **Ordered engineering** | Enforce a dependency chain so downstream work never precedes prerequisites. |
| **Traceability** | Link every document to capabilities, concepts, requirements, and ADRs. |

---

## Approved foundation (complete)

| # | Layer | Status |
|---|--------|--------|
| 00 | Governance | ✅ Complete |
| 01 | SRS | ✅ APPROVED — LOCKED |
| 02 | Architecture | ✅ APPROVED |
| 03 | Constitution | ✅ APPROVED |
| 04 | Business Capability Model | ✅ APPROVED |
| 05 | Enterprise Information Model | ✅ APPROVED |
| 06 | Domain Model (DDD) | ✅ APPROVED — **FROZEN** (ADR-GOV-007) |
| 16 | Implementation Phase 1 | ✅ IAD approved — **realign to .NET per ADR-TECH-001 before M1** |
| 24 | Technology Architecture | ✅ **LOCKED** (ADR-TECH-001) |

**Canonical constitution:** [00-Governance/Project-Constitution.md](00-Governance/Project-Constitution.md) (PC-AIPM-001)

**Authoring methodology:** [00-Governance/Blueprint-Authoring-Methodology.md](00-Governance/Blueprint-Authoring-Methodology.md)

**Document quality gates:** [21-Quality/Document-Quality-Gates.md](21-Quality/Document-Quality-Gates.md)

---

## Folder descriptions

```
Engineering-Blueprint/
│
├── 00-Governance/                      Vision, charter, constitution, methodology, logs
├── 01-SRS/                             Software Requirements Specification (LOCKED)
├── 02-Architecture/                    Software Architecture Document and ADRs
├── 03-Project-Constitution/            Reserved; canonical PC in 00-Governance
├── 04-Business-Capability-Model/       What the business does (APPROVED)
├── 05-Enterprise-Information-Model/    What information exists (conceptual) ← NEXT
├── 06-Domain-Model/                    DDD bounded contexts, aggregates
├── 07-Domain-Events/                   Event catalog, envelopes, flows
├── 08-State-Machines/                  Lifecycles (linked to commands/events)
├── 09-Database/                        Logical and physical data architecture
├── 10-APIs/                            External and internal API specifications
├── 11-Agent-Contracts/                 Agent manifests, dispatch protocol
├── 12-Memory/                          Context assembly, knowledge graph
├── 13-Workflow-Engine/                 Durable workflows, orchestration semantics
├── 14-Security/                        Threat models, controls, compliance
├── 15-Deployment/                      Topology, deployment profiles
├── 16-Implementation/                  Coding standards, module ownership
├── 17-Testing/                         Test strategy, conformance suites
├── 18-Operations/                      Runbooks, SLOs, incident response, DR
├── 19-Roadmap/                         Phased delivery milestones
│
├── 21-Quality/                         Document quality gates, review records
├── 22-Risk/                            Risk register and treatments
├── 23-Decisions/                       Cross-cutting decision index
├── 24-Standards/                       Engineering standards catalog
├── 25-Templates/                       Document and artifact templates
│
└── Assets/                             Shared diagrams and reference artifacts
```

---

## Enterprise architecture flow

```
Business Architecture          →  04 Business Capability Model     ✅ APPROVED
        ↓
Information Architecture       →  05 Enterprise Information Model   ← NEXT
        ↓
Application Architecture       →  06 Domain Model (DDD)
        ↓                              07 Domain Events
        ↓                              08 State Machines
Technology Architecture        →  09 Database … 18 Operations
```

### Why Information Model before DDD

| Without EIM | With EIM |
|-------------|----------|
| `Project` aggregate may be wrong | `Project` / `Program` / `Portfolio` resolved first |
| Late vocabulary changes break everything | Stable CON-### concepts anchor DDD, DB, APIs |
| Jump from business to application | Standard enterprise BA → IA → AA → TA flow |

**Rule (ADR-GOV-005):** `06-Domain-Model` MUST NOT be approved until `05-Enterprise-Information-Model` is approved.

### Traceability rule (Prompt 05+)

Per [ADR-GOV-003](02-Architecture/ADR/ADR-GOV-003-traceability-before-new-concepts.md): no new concept without CAP, CON, FR/NFR, PC, or ADR trace. New concepts require ADR first.

### Command–event–state linkage

Per [ADR-GOV-004](02-Architecture/ADR/ADR-GOV-004-command-event-state-linkage.md): every state transition in `08` links to `CMD-###` and/or `EVT-###` from `06`/`07`.

---

## Document quality gates

Before **APPROVED**, every document passes eleven gates: Business, Architecture, Governance, Consistency, Traceability, Terminology, Future Expansion, AI, Security, Documentation, and Approval reviews.

See [21-Quality/Document-Quality-Gates.md](21-Quality/Document-Quality-Gates.md).

---

## Parallel authoring (after parent approval)

| Tier | Folders |
|------|---------|
| After `04` approved | `05` only |
| After `05` approved | `06` only |
| After `06` approved | `07`, `08` |
| After `07` + `08` approved | `09`–`13` |
| After `13` + `14` approved | `15`, `16` |
| After `15` + `14` approved | `18` |
| Any time (non-binding) | `19-Roadmap`, `21`–`25` |

---

## Repository status

| Item | Path | Status |
|------|------|--------|
| Governance + Constitution | `00-Governance/` | Active |
| SRS | `01-SRS/SRS-AI-Project-Manager.md` | **LOCKED** v1.0.0 |
| SAD | `02-Architecture/SAD-AI-Project-Manager.md` | **APPROVED** v1.0.0 |
| Constitution | `00-Governance/Project-Constitution.md` | **APPROVED** v1.0.0 |
| Business Capability Model | `04-Business-Capability-Model/` | **APPROVED** v1.0.0 |
| **Enterprise Information Model** | `05-Enterprise-Information-Model/` | **APPROVED** v1.0.0 |
| **Domain Model** | `06-Domain-Model/Domain-Model.md` | **APPROVED** v1.0.0 — FROZEN |
| **Technology Architecture** | `24-Standards/Technology-Architecture.md` | **LOCKED** v1.0.0 |
| **Implementation Architecture (Phase 1)** | `16-Implementation/Implementation-Architecture-Phase1.md` | APPROVED (runtime superseded by ADR-TECH-001) |
| Domain Events through Operations | `07`–`18` | Pending |
| Quality / Risk / Standards | `21`–`25` | Framework ready |

---

## Quick links

- [Vision](00-Governance/Vision.md)
- [Project Charter](00-Governance/Project-Charter.md)
- [Glossary](00-Governance/Glossary.md)
- [ADR Index](00-Governance/ADR-Index.md)
- [**Project Constitution**](00-Governance/Project-Constitution.md)
- [SRS](01-SRS/SRS-AI-Project-Manager.md)
- [SAD](02-Architecture/SAD-AI-Project-Manager.md)
- [Business Capability Model](04-Business-Capability-Model/Business-Capability-Model.md)
- [Blueprint Authoring Methodology](00-Governance/Blueprint-Authoring-Methodology.md)
- [**Enterprise Information Model**](05-Enterprise-Information-Model/Enterprise-Information-Model.md)
- [Prompt 05 — EIM Specification](05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md)
- [Domain Model](06-Domain-Model/Domain-Model.md)
- [**Technology Architecture**](24-Standards/Technology-Architecture.md) — LOCKED stack (ADR-TECH-001)
- [**Implementation Architecture Phase 1**](16-Implementation/Implementation-Architecture-Phase1.md)
- [Document Quality Gates](21-Quality/Document-Quality-Gates.md)
