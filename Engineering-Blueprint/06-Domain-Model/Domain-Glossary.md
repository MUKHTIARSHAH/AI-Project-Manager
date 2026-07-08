# Domain Glossary — DM-AIPM-001

**Version:** 1.0.0 | Aligns with EIM-AIPM-001. Ubiquitous language for DDD.

| Term | Definition |
|------|------------|
| AgentInstance | Runtime deployment of an AgentType with health status (AGG-013). |
| AgentType | Registered specialist workforce class (AGG-012). |
| Approval | Human authorization decision on a gated action (AGG-015). |
| AuditRecord | Immutable record of a material domain action (AGG-018). |
| Bounded Context | Explicit linguistic boundary with consistent models (BC-01–15). |
| ContextPackage | Assembled knowledge bundle for a Dispatch (DS-001 output). |
| Dispatch | Domain act of delegating a Task to certified AgentType (AGG-010). |
| Domain Event | Past-tense fact raised by an aggregate (EVT-###). |
| GateDefinition | Configuration of objective quality criteria (AGG-016). |
| Halt | Emergency cessation of new dispatches (AGG-011). |
| PlanVersion | Immutable planning snapshot; one active baseline (AGG-007). |
| PolicySet | Versioned collection of tenant policies (AGG-014). |
| Portfolio | Executive collection of programs and projects (AGG-002). |
| Program | Coordinated project group within a portfolio (AGG-003). |
| Project | Primary governed delivery unit (AGG-004). |
| Release | Authorized production promotion (AGG-017). |
| Requirement | Structured delivery intent (AGG-005). |
| Task | Delegable work unit under a PlanVersion (AGG-008). |
| TaskNode | Structural plan element in a DAG (ENT-010); not delegable alone. |
| Tenant | Customer partition root (AGG-001). |
| WorkAssignment | Assured binding of Dispatch to AgentInstance (ENT-020). |
| WorkSchedule | Ordered queue of Tasks ready for delegation (AGG-009). |

*Full EIM terms: see Information-Glossary.md. Domain adds aggregate and BC vocabulary only.*
