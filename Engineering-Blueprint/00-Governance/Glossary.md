# Glossary

**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Status:** Normative  
**Source:** SRS-AIPM-001 §24–26

Conflicts are resolved in favor of this glossary.

---

## Core terms

| Term | Definition |
|------|------------|
| **Agent** | Autonomous or semi-autonomous service that performs specialized work when dispatched by AIPM. |
| **Artifact** | Durable output produced by an agent (code, config, doc, test result). |
| **AIPM** | AI Project Manager — the orchestration and governance control plane. |
| **Authoritative State** | Project data whose canonical copy resides in AIPM; external systems are replicas unless configured otherwise (ADR-005). |
| **Autonomy Level** | Integer 0–4: 0 = recommend only; 4 = execute irreversible production changes if policy allows. |
| **Certified Agent** | Agent that passed security, contract, and interoperability tests for a given version range. |
| **Control Plane** | AIPM components that decide what happens; no customer application execution. |
| **Data Plane** | Agents and integrations that execute work and produce artifacts. |
| **Dispatch** | Sending a task to an agent with context and credentials. |
| **Explainability Record** | Structured summary of why a plan or decision was made (inputs + rule outcomes; not raw chain-of-thought). |
| **Gate** | Controlled checkpoint blocking progression until conditions are satisfied. |
| **Golden Path** | Reference end-to-end scenario for certification and demos. |
| **Irreversible Action** | Action not automatically rollback-able within 5 minutes without data loss (e.g., production deploy). |
| **PEP** | Policy Evaluation Point — moment policies must pass before proceeding. |
| **Plan** | Versioned directed acyclic graph of tasks and dependencies derived from requirements. |
| **Policy** | Machine-evaluable rule set governing autonomy and approvals. |
| **Task** | Atomic unit of work assigned to one agent with defined inputs/outputs. |
| **Tenant** | Isolated customer organization within the platform. |
| **Waiver** | Time-bound approved exception to a gate requirement. |

## Lifecycle states

### Project

`Draft` → `Ready` → `Planning` → `Executing` → `Blocked` | `Gated` → `Released` → `Archived`  
Any state may transition to `Halted` (resume returns to prior state).

### Task

`Pending` → `Scheduled` → `Dispatched` → `Running` → `Validating` → `Succeeded` | `Failed` → `DeadLetter`  
`Cancelled` available from several states.

### Tenant isolation tier

| Tier | Description |
|------|-------------|
| **Logical** | Shared compute; isolated data |
| **Dedicated** | Single-tenant compute |
| **AirGapped** | No public internet egress |

## Acronyms (selected)

| Acronym | Expansion |
|---------|-----------|
| ABAC | Attribute-Based Access Control |
| ADR | Architecture Decision Record |
| CQRS | Command Query Responsibility Segregation |
| FR / NFR | Functional / Non-Functional Requirement |
| PEP | Policy Evaluation Point |
| RBAC | Role-Based Access Control |
| RTM | Requirements Traceability Matrix |
| SAD | Software Architecture Document |
| SRS | Software Requirements Specification |

Full acronym list: SRS-AIPM-001 §25.
