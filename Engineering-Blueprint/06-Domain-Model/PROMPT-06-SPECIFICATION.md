# Prompt 06 — Enterprise Domain Model Specification

**Document ID:** PROMPT-06-AIPM  
**Version:** 2.0.0  
**Date:** 2026-07-07  
**Status:** APPROVED  
**Governance:** Methodology expansion **stops** after this prompt. System design expands from DM-AIPM-001.

**Domain stability:** [ADR-GOV-007](../02-Architecture/ADR/ADR-GOV-007-domain-stability-rule.md)

---

## Prerequisites

| Parent | ID | Status |
|--------|-----|--------|
| Business Capability Model | BCM-AIPM-001 | APPROVED |
| Enterprise Information Model | EIM-AIPM-001 | APPROVED |
| SRS | SRS-AIPM-001 | LOCKED |
| SAD | SAD-AIPM-001 | APPROVED |
| Constitution | PC-AIPM-001 | APPROVED |

Every aggregate, entity, command, and event MUST trace to **CAP-###** and **CON-###**.

---

## Output artifacts

```
06-Domain-Model/
├── Domain-Model.md                 # Strategic design, vision, context map, audits
├── Bounded-Contexts.md               # BC-01–BC-15 tactical strategic design
├── Aggregate-Catalog.md              # AGG-### full definitions
├── Domain-Tactical-Catalog.md        # ENT, VO, DS, REP, FAC, POL
├── Commands-Events-Catalog.md        # CMD-###, EVT-###
├── Domain-Glossary.md
├── Domain-Traceability-Matrix.md
├── Domain-Decision-Log.md
├── Assumption-Register.md
└── Open-Questions-Register.md
```

---

## Required sections

### Strategic design (Domain-Model.md)

Domain Vision · Domain Mission · Ubiquitous Language · Core Domain · Supporting Domains · Generic Domains · Subdomains · Context Map · Context Relationships

### Tactical design (Bounded-Contexts.md)

Per BC: Purpose · Responsibilities · Inputs · Outputs · Upstream · Downstream · Shared Kernel · ACL · OHS · Published Language · Partnership · Customer/Supplier · Separate Ways

### Aggregates (Aggregate-Catalog.md)

AGG-### · Root · Business Purpose · Responsibilities · Consistency Boundary · Transaction Boundary · Lifecycle · Invariants · Commands · Events · Policies · Dependencies · Traceability (CON, CAP, FR, MOD, PC, ADR)

### Entities · Value Objects · Domain Services · Repositories · Factories · Policies

In Domain-Tactical-Catalog.md with IDs: ENT-###, VO-###, DS-###, REP-###, FAC-###, POL-###

### Domain lifecycle (per aggregate)

Creation · Activation · Modification · Suspension · Completion · Archival · Deletion

### Invariants

Per aggregate: what must always be true · what must never occur

---

## Audits (required before APPROVED)

### Domain Purity Audit

| Forbidden in DM | Pass |
|-----------------|------|
| Database terminology (table, column, index) | Zero occurrences |
| API terminology (endpoint, REST, gRPC) | Zero |
| Programming languages | Zero |
| Frameworks (Spring, Django, etc.) | Zero |
| Infrastructure (Kubernetes, S3, etc.) | Zero |
| Implementation details | Zero |

### Traceability Audit

Every artifact → CAP, CON, FR/NFR, MOD, PC section, ADR where applicable.

### Terminology Audit

Aligns with EIM Information Glossary; no new CON without ADR.

---

## Approval

```
DOMAIN MODEL STATUS: APPROVED
```

After approval, DM-AIPM-001 is **frozen** per ADR-GOV-007. Changes require ADR.

---

## References

ADR-GOV-003 through ADR-GOV-007 · EIM-AIPM-001 · SAD §12–13
