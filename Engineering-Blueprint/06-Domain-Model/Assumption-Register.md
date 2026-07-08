# Assumption Register — DM-AIPM-001

| ASS | Description | Why | Evidence | Risk | Validation | Status |
|-----|-------------|-----|----------|------|------------|--------|
| ASS-001 | One AGG-006 Plan per AGG-004 Project | Simplifies baseline | FR-020 | Multi-plan products | ADR if needed | Validated |
| ASS-002 | ContextPackage not aggregate root | Stateless assembly | DDL-004 | Stateful context drift | Memory spec 12 | Validated |
| ASS-003 | 25 aggregates sufficient for GA | SAD BC coverage | SAD §12 | Missing root | Event catalog review | Open |
| ASS-004 | Credential entity under AgentInstance | Revoke with workforce | FR-117 | Wrong BC | Security review 14 | Open |
| ASS-005 | All CMD authorized via BC-10 OHS | Central authz | SAD context map | Distributed authz | API spec 10 | Open |
