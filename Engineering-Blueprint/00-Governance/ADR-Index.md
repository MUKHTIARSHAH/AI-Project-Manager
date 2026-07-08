# ADR Index

**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Status:** Active  
**Location of records:** `02-Architecture/ADR/`

---

## Technology decisions

| ADR | Title | Decision | Status |
|-----|-------|----------|--------|
| [ADR-TECH-001](../02-Architecture/ADR/ADR-TECH-001-Approved-Technology-Stack.md) | Approved technology stack | .NET 9, PostgreSQL, Redis, pgvector, OpenSearch, RabbitMQ→Kafka, Next.js, Azure | **APPROVED — LOCKED** |
| [ADR-TECH-002](../02-Architecture/ADR/ADR-TECH-002-M6-admin-shell-ai-foundation.md) | M6 scope refinement | Keep M6 position; prioritize provider-independent AI foundation + minimal admin connectivity shell | Accepted |

**Note:** ADR-TECH-001 supersedes ADR-008 for implementation language/framework choices. ADR-008 file is unchanged (locked).

## SRS decisions (immutable baseline)

| ADR | Title | Decision | Status |
|-----|-------|----------|--------|
| [ADR-001](../02-Architecture/ADR/ADR-001-hybrid-saas-deployment.md) | Deployment model | Hybrid SaaS default + dedicated enterprise tier | Accepted |
| [ADR-002](../02-Architecture/ADR/ADR-002-enterprise-first-segment.md) | Customer segment | Mid-market and enterprise first | Accepted |
| [ADR-003](../02-Architecture/ADR/ADR-003-policy-driven-autonomy.md) | Human authority | Policy-driven hybrid autonomy; mandatory production gates | Accepted |
| [ADR-004](../02-Architecture/ADR/ADR-004-model-agnostic-router.md) | LLM strategy | Model-agnostic router with approved catalog | Accepted |
| [ADR-005](../02-Architecture/ADR/ADR-005-authoritative-state.md) | Source of truth | AIPM authoritative; optional bidirectional external PM sync | Accepted |
| [ADR-006](../02-Architecture/ADR/ADR-006-us-eu-compliance.md) | Regulatory scope | US + EU compliance at launch | Accepted |
| [ADR-007](../02-Architecture/ADR/ADR-007-isolated-agent-runtimes.md) | Agent ownership | PM orchestrates; agents in isolated runtimes | Accepted |
| [ADR-008](../02-Architecture/ADR/ADR-008-implementation-languages.md) | Languages | Go/Java control plane; TypeScript UI; Python analytics isolated | Accepted |

## Architecture decisions (SAD)

| ADR | Title | Decision | Status |
|-----|-------|----------|--------|
| [ADR-SAD-001](../02-Architecture/ADR/ADR-SAD-001-five-deployable-units.md) | Deployable topology | Five DUs: Edge, Core, Governance, Integration, Analytics | Accepted |
| [ADR-SAD-002](../02-Architecture/ADR/ADR-SAD-002-workflow-engine.md) | Workflow | Embedded FSM + durable workflow adapter | Accepted |
| [ADR-SAD-003](../02-Architecture/ADR/ADR-SAD-003-knowledge-graph-storage.md) | Knowledge graph | Relational authoritative + graph projection | Accepted |
| [ADR-SAD-004](../02-Architecture/ADR/ADR-SAD-004-event-backbone.md) | Events | Log-based event backbone with abstraction layer | Accepted |
| [ADR-SAD-005](../02-Architecture/ADR/ADR-SAD-005-deployment-profiles.md) | Profiles | Standard SaaS, Dedicated, AirGapped (capability flags) | Accepted |
| [ADR-SAD-006](../02-Architecture/ADR/ADR-SAD-006-tenant-sharding.md) | Sharding | `tenant_id` as primary shard key; immutable once assigned | Accepted |
| [ADR-SAD-007](../02-Architecture/ADR/ADR-SAD-007-mtls-workload-identity.md) | Transport security | mTLS + workload identity for all internal and agent channels | Accepted |
| [ADR-SAD-008](../02-Architecture/ADR/ADR-SAD-008-cqrs.md) | Read/write split | CQRS: write model in Core; read models in Analytics plane | Accepted |
| [ADR-SAD-009](../02-Architecture/ADR/ADR-SAD-009-policy-peps.md) | Policy PEPs | Evaluate at Schedule, Dispatch, and Gate | Accepted |
| [ADR-GOV-001](../02-Architecture/ADR/ADR-GOV-001-constitution-canonical-path.md) | Constitution path | Canonical PC in 00-Governance | Proposed |
| [ADR-GOV-002](../02-Architecture/ADR/ADR-GOV-002-bcm-before-ddd.md) | Blueprint order | BCM before DDD; state machines at 07 | Accepted |
| [ADR-GOV-003](../02-Architecture/ADR/ADR-GOV-003-traceability-before-new-concepts.md) | Scope control | No new concepts without CAP/SRS/PC/ADR trace; ADR first | Accepted |
| [ADR-GOV-004](../02-Architecture/ADR/ADR-GOV-004-command-event-state-linkage.md) | Lifecycle linkage | Every state transition linked to CMD and/or EVT | Accepted |
| [ADR-GOV-005](../02-Architecture/ADR/ADR-GOV-005-eim-before-ddd.md) | Information layer | EIM before DDD; folder renumber 05–19; support folders 21–25 | Accepted |
| [ADR-GOV-006](../02-Architecture/ADR/ADR-GOV-006-canonical-concepts-frozen-roadmap.md) | EIM discipline | One definition/owner per concept; frozen roadmap; ASS/Q registers | Accepted |
| [ADR-GOV-007](../02-Architecture/ADR/ADR-GOV-007-domain-stability-rule.md) | Domain stability | DM canonical; no aggregate/CMD/EVT change without ADR | Accepted |

## Process

1. Propose ADR using template in `02-Architecture/ADR/`.
2. Review at ARB.
3. On acceptance: update this index, Decision-Log, and Change-Log.
4. Link ADR from affected blueprint documents and RTM.
