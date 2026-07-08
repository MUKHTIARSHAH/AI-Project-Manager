# Blueprint Authoring Methodology

**Document ID:** BAM-AIPM-001  
**Version:** 1.2.0  
**Date:** 2026-07-07  
**Status:** APPROVED  
**Applies to:** All documents in folders `05` through `25`

---

## Purpose

From Prompt 05 onward, blueprint artifacts directly influence implementation. This methodology tightens authoring discipline to prevent scope creep, ensure internal consistency, and preserve traceability from business capabilities through information concepts to domain design and runtime behavior.

**Supreme authority unchanged:** PC-AIPM-001 → SRS-AIPM-001 → SAD-AIPM-001 → ADRs → other blueprint documents.

**Framing:** AIPM is the **operating system for an AI software company**—the control plane coordinating requirements, planning, development, testing, deployment, documentation, monitoring, and maintenance. Blueprint documents describe that control plane, not "another AI agent."

---

## Permanent rule (Prompt 05+)

> **No new concept may be introduced unless it is traceable to an approved capability (CAP-###), information concept (CON-###, after EIM), requirement (FR/NFR/SC/CON), constitutional article, or accepted ADR. If a genuinely new concept is required, it must first be proposed as a new ADR before being added.**

**Governance record:** [ADR-GOV-003](../02-Architecture/ADR/ADR-GOV-003-traceability-before-new-concepts.md)

**Canonical concept rule (ADR-GOV-006):** Every `CON-###` has exactly one canonical definition and one canonical owner. No duplicate concepts, synonyms as alternate roots, or overlapping responsibilities without merge or explicit boundary documentation.

**Frozen roadmap (ADR-GOV-006):** Folder order `00`–`19` and `21`–`25` is frozen. Reordering requires a new accepted ADR.

### Mandatory registers (every document seeking APPROVED)

| Register | ID pattern | Template |
|----------|------------|----------|
| Assumption Register | ASS-### | `25-Templates/Assumption-Register-Template.md` |
| Open Questions Register | Q-### | `25-Templates/Open-Questions-Register-Template.md` |

### Author checklist (every new concept)

- [ ] Named with stable ID (CON-, CMD-, EVT-, AGG-, BC-, etc.)
- [ ] Mapped to ≥1 CAP-### from BCM-AIPM-001
- [ ] Mapped to ≥1 CON-### from EIM-AIPM-001 (for `06+`)
- [ ] Mapped to ≥1 SRS ID where applicable
- [ ] Mapped to SAD module (MOD-##) or bounded context (BC-##) where applicable
- [ ] Mapped to PC section or ADR if decision-driven
- [ ] If no mapping exists → **stop** and draft ADR first

---

## Enterprise architecture layers

```
Business Architecture     → 04 Business Capability Model (APPROVED)
Information Architecture  → 05 Enterprise Information Model (NEXT)
Application Architecture  → 06 Domain Model (DDD)
Technology Architecture   → 09+ Database, APIs, Deployment, etc.
```

---

## Approved foundation (do not rework)

| # | Layer | Status |
|---|--------|--------|
| 00 | Governance | Complete |
| 01 | SRS | APPROVED — LOCKED |
| 02 | Architecture | APPROVED |
| 03 | Constitution | APPROVED |
| 04 | Business Capability Model | APPROVED |
| 05 | Enterprise Information Model | APPROVED |
| 06 | Domain Model | APPROVED — **FROZEN** (ADR-GOV-007) |

---

## Frozen roadmap (ADR-GOV-006)

Folder order is **frozen**. No reordering without a new accepted ADR:

`00`–`19` core sequence; `21` Quality, `22` Risk, `23` Decisions, `24` Standards, `25` Templates.

---

```
04 BCM (APPROVED)
    └── 05 Enterprise Information Model   ← NEXT (Prompt 05)
            └── 06 Domain-Driven Design   (Prompt 06; paused until EIM approved)
                    ├── 07 Domain Events
                    ├── 08 State Machines   (linked to 06 commands + 07 events)
                    ├── 09 Database Model
                    ├── 10 API Specifications
                    ├── 11 Agent Contracts
                    ├── 12 Memory Architecture
                    └── 13 Workflow Engine
                            ├── 14 Security Details
                            ├── 15 Deployment
                            ├── 16 Implementation Blueprint
                            ├── 17 Testing
                            └── 18 Operations
                                    └── 19 Roadmap

Support (parallel when needed):
    21 Quality    — Document quality gates
    22 Risk       — Risk register and treatments
    23 Decisions  — Cross-cutting decision index
    24 Standards  — Engineering standards catalog
    25 Templates  — Document and code templates
```

**Rules:**

- `05` MUST NOT be skipped before `06`.
- `06` MUST NOT be APPROVED until `05` is APPROVED (ADR-GOV-005).
- `07` and `08` may draft in parallel after `06` structure is stable; neither APPROVED until CMD/EVT linkage complete (ADR-GOV-004).

---

## Command–event–state linkage (mandatory)

**Governance record:** [ADR-GOV-004](../02-Architecture/ADR/ADR-GOV-004-command-event-state-linkage.md)

| Rule | Requirement |
|------|-------------|
| State transition | MUST reference triggering command and/or resulting event |
| Domain command | MUST declare postcondition state transition(s) |
| Domain event | MUST declare source transition and business impact |
| Approval gate | `08` not APPROVED until `06` commands and `07` events exist |

---

## Document quality gates

Every document seeking APPROVED status MUST pass eleven quality gates. See [Document Quality Gates](../21-Quality/Document-Quality-Gates.md) (DQG-AIPM-001).

---

## Prompt specifications

| Prompt | Target folder | Specification |
|--------|---------------|---------------|
| **05** | `05-Enterprise-Information-Model/` | [PROMPT-05-SPECIFICATION.md](../05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md) |
| **06** | `06-Domain-Model/` | [PROMPT-06-SPECIFICATION.md](../06-Domain-Model/PROMPT-06-SPECIFICATION.md) |
| 07 | `07-Domain-Events/` | Derive from `06` events; extend envelope and flow catalog |
| 08 | `08-State-Machines/` | Derive from `06`/`07`; mandatory CMD/EVT linkage |
| 09+ | Respective folders | Trace to `05` concepts and `06` aggregates |

---

## Validation standard (all Prompt 05+ documents)

Before APPROVED status, run and document:

| Audit | Pass criteria |
|-------|---------------|
| Traceability | Zero orphan concepts |
| Capability coverage | Every CAP has concepts (EIM) or BC/aggregate (DDD) |
| Duplicate | No duplicate concepts or aggregates |
| Boundary | Correct layer (no DB in EIM, no tables in DDD) |
| Terminology | Matches Information/Domain Glossary |
| Linkage (07/08) | 100% transitions linked per ADR-GOV-004 |
| Governance | PC invariants preserved |
| Quality gates | All applicable QG-01–QG-11 PASS |

---

## References

- PC-AIPM-001, SRS-AIPM-001, SAD-AIPM-001
- BCM-AIPM-001, EIM-AIPM-001 (when approved)
- ADR-GOV-002 through ADR-GOV-007
- DQG-AIPM-001
- RTM-AIPM-001
