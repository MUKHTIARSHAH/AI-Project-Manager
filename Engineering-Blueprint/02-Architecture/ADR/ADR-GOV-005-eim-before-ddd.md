# Enterprise Information Model Before DDD

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

The blueprint progressed Business Architecture (BCM) directly to Application Architecture (DDD). Enterprise practice inserts an **Information Architecture** layer between business capabilities and software domain models:

```
Business Architecture → Information Architecture → Application Architecture → Technology Architecture
```

Without a conceptual information model, DDD aggregates may encode unstable vocabulary. Late discovery that `Project` should decompose into `Program`, `Portfolio`, and `Initiative` forces redesign of aggregates, commands, events, database, APIs, memory, and agent contracts.

## Decision

1. Insert `05-Enterprise-Information-Model/` before `06-Domain-Model/`.
2. Renumber downstream folders: Domain Model `06`, Events `07`, State Machines `08`, Database `09`, through Roadmap `19`.
3. Add implementation-support folders `21-Quality`, `22-Risk`, `23-Decisions`, `24-Standards`, `25-Templates`.
4. **EIM-AIPM-001** MUST be APPROVED before **DM-AIPM-001** (DDD).
5. Every DDD aggregate, entity, and value object MUST trace to an **information concept** (`CON-###`) defined in EIM.
6. Prompt 05 (DDD) is **paused**; Prompt 05 is now **Enterprise Information Model**.

## Consequences

+ Stable business vocabulary before technical modeling.  
+ Reduced aggregate redesign risk.  
+ Clear BCM → EIM → DDD → Events traceability chain.  
- One additional approval gate (intentional).  
- Folder renumbering touches README, methodology, and cursor rules.

## References

- [Blueprint Authoring Methodology](../../00-Governance/Blueprint-Authoring-Methodology.md)
- [PROMPT-05-SPECIFICATION](../../05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md)
- ADR-GOV-002, ADR-GOV-003, ADR-GOV-004
- BCM-AIPM-001
