# Domain Stability Rule

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board  
**Precedence:** One of the highest governance rules in the AIPM program

## Context

EIM-AIPM-001 stabilizes information vocabulary. DM-AIPM-001 (Domain Model) becomes the **canonical business model** for the platform. Without post-approval stability rules, downstream database, API, workflow, memory, and agent contract documents will invent parallel models—causing architectural drift.

Governance methodology expansion stops here. **System design expands from the Domain Model onward.**

## Decision

After **DM-AIPM-001** is APPROVED:

1. **No Aggregate** may be added without a new **ADR**.
2. **No Entity** may change identity semantics without a new **ADR**.
3. **No Value Object** may change business meaning without a new **ADR**.
4. **No Domain Event** may be introduced unless it originates from an **approved Aggregate**.
5. **No Command** may exist without traceability to an approved **CAP-###** and **CON-###**.
6. Every future **Database**, **API**, **Workflow**, **Memory**, and **Agent Contract** specification MUST derive from DM-AIPM-001—not invent parallel concepts.

**Modification process:** Changes to APPROVED Domain Model require ADR + version bump (minor for additive with ADR, major for breaking identity/meaning changes).

## Consequences

+ Single canonical business model for implementation.  
+ Traceability chain: CAP → CON → Aggregate → CMD/EVT → physical artifacts.  
+ Reduced drift across teams and AI-generated specs.  
- Friction for domain changes (intentional).  
- ADR backlog for legitimate evolution.

## Compliance

- Prompt 07+ authors MUST cite AGG/CMD/EVT IDs from DM-AIPM-001.  
- ARB MUST reject specs introducing untraced aggregates or commands.  
- Domain Purity Audit required at DM approval (no DB/API/code terms).

## References

- EIM-AIPM-001, BCM-AIPM-001
- ADR-GOV-003, ADR-GOV-004, ADR-GOV-006
- PROMPT-06-AIPM v2.0.0
- PC-AIPM-001 Section 1
