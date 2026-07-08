# Canonical Concepts and Frozen Roadmap

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

As the blueprint shifts to world-class engineering documentation, two risks emerge: (1) duplicate or overlapping information concepts causing DDD/API/memory drift; (2) repeated folder renumbering destabilizing traceability and authoring specs.

## Decision

### 1. Canonical concept rule (EIM and downstream)

> **Every concept in the Enterprise Information Model must have exactly one canonical definition and one canonical owner. No duplicate concepts, synonyms, or overlapping responsibilities are allowed. If two concepts overlap, they must either be merged or their boundaries explicitly documented.**

Applies to all `CON-###` in EIM-AIPM-001 and MUST be preserved in DDD, Events, APIs, Memory, and Agent Contracts.

### 2. Frozen blueprint roadmap

The folder sequence `00`–`19` and support folders `21`–`25` is **frozen**. No reordering unless a new ADR explicitly approves it.

```text
00 Governance → 01 SRS → 02 Architecture → 03 Constitution → 04 BCM
→ 05 EIM → 06 DDD → 07 Events → 08 State Machines → 09 Database
→ 10 APIs → 11 Agent Contracts → 12 Memory → 13 Workflow Engine
→ 14 Security → 15 Deployment → 16 Implementation → 17 Testing
→ 18 Operations → 19 Roadmap
→ 21 Quality, 22 Risk, 23 Decisions, 24 Standards, 25 Templates
```

### 3. Mandatory cross-document registers (Prompt 05+)

Every blueprint document seeking APPROVED status MUST include or reference:

- **Assumption Register** (`ASS-###`)
- **Open Questions Register** (`Q-###`)

### 4. Terminology Consistency Audit

EIM approval requires passing the Terminology Consistency Audit defined in PROMPT-05-SPECIFICATION v1.1+.

## Consequences

+ Stable vocabulary and ownership before implementation.  
+ Predictable blueprint structure for authors and tooling.  
+ Visible assumptions and open questions reduce hidden debt.  
- Stricter authoring bar for EIM and downstream docs.

## References

- [PROMPT-05-SPECIFICATION](../../05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md)
- ADR-GOV-003, ADR-GOV-005
- BCM-AIPM-001
