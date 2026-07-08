# Domain Decision Log — DM-AIPM-001

| DDL | Decision | Reason | Alternatives | Impact |
|-----|----------|--------|--------------|--------|
| DDL-001 | Separate AGG for Portfolio, Program, Project | IDL-001 EIM hierarchy | Single Project aggregate | BC-01 three roots |
| DDL-002 | Plan and PlanVersion as separate aggregates | Version immutability vs container | Single aggregate | BC-03 partnership with BC-04 |
| DDL-003 | Dispatch as own aggregate not part of Task | IDL-003 audit and assurance | Task.dispatch() | CMD-070 lifecycle |
| DDL-004 | ContextPackage via domain service not AGG | Assembly is stateless operation | AGG-028 ContextPackage | DS-001, FAC-005 |
| DDL-005 | BC-15 no mutable aggregates | CQRS read model ADR-SAD-008 | PerformanceMetric AGG | Projections only in 07+ |
| DDL-006 | AuditRecord append-only aggregate | Compliance INV | Mutable log | BC-09 invariant |
