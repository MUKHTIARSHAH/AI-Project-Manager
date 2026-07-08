# Information Decision Log — EIM-AIPM-001

| IDL | Decision | Reason | Alternatives | Impact on DDD | Status |
|-----|----------|--------|--------------|---------------|--------|
| IDL-001 | Portfolio → Program → Project → Initiative hierarchy is explicit; Project may attach to Program or Portfolio directly | Prevents late redesign of portfolio aggregates; matches enterprise PM practice | Flat Project-only; merge Program into Portfolio | Separate aggregates for Portfolio, Program, Project, Initiative in BC-01 | Accepted |
| IDL-002 | TaskNode (structure) separated from Task (delegable unit) | DAG structure vs executable work have different lifecycles | Single Task concept only | TaskNode in planning BC; Task in execution BC | Accepted |
| IDL-003 | Dispatch (orchestration act) separated from WorkAssignment (assured binding to AgentInstance) | Assurance and audit require distinct records | Single delegation record | Dispatch aggregate + WorkAssignment entity | Accepted |
| IDL-004 | AgentType (catalog) separated from AgentInstance (runtime) | Registration vs health monitoring | Single Agent concept | BC-05 split roots | Accepted |
| IDL-005 | AuditRecord (immutable system record) vs ExplainabilityRecord (human rationale) | Compliance vs interpretability; different retention and consumers | Merge into single log | BC-09 Audit vs cross-cutting explainability value object | Accepted |
