# Command–Event–State Transition Linkage

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

The blueprint separates Domain Events (`07`) and State Machines (`08`) into distinct documents. Without mandatory linkage, state transitions may be defined in isolation from commands and events—creating inconsistent lifecycle semantics across database, workflow, and API layers.

## Decision

**Domain Events and State Machines remain separate documents**, but linkage is mandatory:

1. **Every state transition** in `08-State-Machines/` MUST be triggered by at least one **domain command** and/or MUST result in at least one **domain event** defined in `06-Domain-Model` or `07-Domain-Events`.
2. **Every domain command** that mutates aggregate state MUST declare its **postcondition state transition(s)** (defined in `06-Domain-Model` or `08-State-Machines`).
3. **Every domain event** that signals state change MUST declare the **source transition** and **subscriber impacts** (catalog in `07-Domain-Events`).
4. Cross-document trace IDs MUST be used:

| Link type | From | To | Trace field |
|-----------|------|-----|-------------|
| Command → transition | `06` Command | `08` Transition | `CMD-###` |
| Transition → event | `08` Transition | `07` Event | `EVT-###` |
| Event → aggregate | `07` Event | `06` Aggregate | `AGG-###` |

5. State Machines MUST NOT be approved until Domain Model commands and Domain Events catalog entries exist for all referenced transitions.

## Consequences

+ Single coherent lifecycle story across DDD, events, and FSM specs.  
+ Workflow engine and implementation can derive behavior mechanically.  
+ Auditors can trace command → state → event → audit record.  
- Higher upfront authoring cost in Prompts 06–08 (intentional).

## References

- [Blueprint Authoring Methodology](../../00-Governance/Blueprint-Authoring-Methodology.md)
- ADR-GOV-002 (folder separation)
- ADR-SAD-002 (workflow engine)
- ADR-SAD-004 (event backbone)
- ADR-GOV-005 (EIM before DDD)
- SAD §11–16 (bounded contexts)
