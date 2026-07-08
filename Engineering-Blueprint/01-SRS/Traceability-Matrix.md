# Requirements Traceability Matrix

**Document ID:** RTM-AIPM-001  
**Version:** 1.0.0  
**Date:** 2026-07-07  
**Sources:** SRS-AIPM-001 v1.0.0, SAD-AIPM-001 v1.0.0, PC-AIPM-001 v1.0.0  
**Status:** Active

All downstream documents and implementations must comply with [Project Constitution](../03-Project-Constitution/Project-Constitution.md) Articles cited below.

Maps SRS requirements to SAD modules and architecture sections. Update when requirements or architecture change.

---

## Functional requirements → modules

| Requirement | Description (summary) | Primary module(s) | SAD section |
|-------------|----------------------|-------------------|-------------|
| FR-001–005 | Project & portfolio CRUD, templates, scope | MOD-02 | §16 |
| FR-010–014 | Requirements ingest, traceability | MOD-03 | §16 |
| FR-020–024 | Planning, DAG, replan, simulate | MOD-04 | §36 |
| FR-030–034 | Agent registry, health, certification | MOD-07 | §31–32 |
| FR-040–044 | Execution, retry, halt, DLQ | MOD-06, MOD-27 | §37, §41 |
| FR-050–054 | Policies, approvals, SoD | MOD-09, MOD-11 | §67, §42 |
| FR-060–063 | Quality gates, waivers | MOD-10 | §39 |
| FR-070–074 | Integrations VCS/CI/tickets | MOD-15, MOD-17 | §86 |
| FR-080–084 | Audit, dashboards, cost, reports | MOD-12, MOD-23 | §53, §58 |
| FR-090–094 | Tenant isolation, IAM, retention | MOD-13, MOD-25 | §82, §45–49 |
| FR-100–103 | Knowledge graph, context, legal hold | MOD-19 | §71–73 |
| FR-110–112 | Conflict detection and resolution | MOD-28 | §16 |
| FR-115–116 | Post-release maintenance, incident loop | MOD-06, workflows | §40, §38 |
| FR-117 | Credential revoke on halt ≤60s | MOD-14, MOD-27 | §50, ADR-SAD-010 |
| FR-118 | Dispatch references registered agent type | MOD-08 | §29 |
| FR-119 | Explainability records | MOD-11, MOD-12 | §42 |
| FR-120 | Region pinning | MOD-13, storage | §81 |
| FR-121 | Audit tenant boundary | MOD-12 | §53 |
| FR-122 | Async portfolio aggregates | MOD-23 | §24, §76 |

## Non-functional requirements → architecture

| Requirement | Target | Primary sections | Modules |
|-------------|--------|------------------|---------|
| NFR-001 | P95 read ≤200ms | §74, §87, §95 | MOD-30, MOD-23 |
| NFR-002 | P95 dispatch ≤2s | §29, §95 | MOD-08 |
| NFR-003 | 10k concurrent tasks | §76, §61, §62 | MOD-05, MOD-06 |
| NFR-004 | 99.9% API availability | §77 | DU-1, DU-2 |
| NFR-005 | RPO ≤15m, RTO ≤4h | §78, §79 | All DUs |
| NFR-006–007 | Encryption, secrets vault | §50–52 | MOD-14 |
| NFR-008 | PII minimization | §71 | MOD-19 |
| NFR-009–010 | SOC 2, GDPR readiness | §53, SRS §33 | MOD-12 |
| NFR-011 | Modular ownership | §8, §10 | All modules |
| NFR-012 | Extensible agent types | §9, §83 | MOD-07 |
| NFR-013 | Event schema versioning | §19, §89 | MOD-18 |
| NFR-014 | WCAG 2.1 AA admin UI | §38 (SRS) | DU-1 Experience |
| NFR-015 | Trace on all dispatch paths | §56–57, §88 | All dispatch path |
| NFR-016 | Cross-region DR failover | §78, §80 | Infra |
| NFR-017 | i18n-ready, UTC timestamps | §25–26 | All |
| NFR-018 | ACID authoritative writes | §22, §75 | DU-2, DU-3 |
| NFR-019 | No unvalidated model output | §68 | MOD-21 |
| NFR-020 | Per-tenant budget enforcement | §94 | MOD-22 |
| NFR-021 | Tiered event ingestion SLO | §77 | MOD-18 |
| NFR-022 | 500k events/sec Year 5 | §19, §76 | MOD-18 |
| NFR-023 | Agent ack ≤30s P95 | §29, §61 | MOD-08 |

## Success criteria → evidence

| Criterion | Verification | Architecture evidence |
|-----------|--------------|----------------------|
| SC-001 | 30-day availability | §77, §87 |
| SC-002 | Pen test, zero critical vulns | §43 |
| SC-003 | Audit replay 100% | §19, §22, §53 |
| SC-004 | Golden path E2E | §27, §36–40 |
| SC-005 | 10k tasks / 500 tenants load test | §76, §96 |
| SC-006 | Budget stop ≤60s | §94 |
| SC-007 | Bidirectional issue tracker sync | §86 |
| SC-008 | Halt ≤30s | §27, §62, MOD-27 |

## Constraints → architecture enforcement

| Constraint | Enforcement |
|------------|-------------|
| CON-001 | No codegen in MOD-06/MOD-08; agents only |
| CON-002 | MOD-13 SSO; §82 multi-tenant |
| CON-005 | §97 Profile P-AirGapped |
| CON-006 | §83 plugins; §91 configuration not forks |
| CON-008 | §31 agent registration; §90 compatibility matrix |

## Module ownership (implementation teams)

| Module | Team |
|--------|------|
| MOD-01–02 | Platform Edge & Core |
| MOD-03–04 | Planning Domain |
| MOD-05–08 | Execution Domain |
| MOD-09–12 | Governance & Compliance |
| MOD-13–14 | Security & Identity |
| MOD-15–17 | Integrations |
| MOD-18 | Infrastructure / SRE |
| MOD-19–21 | Knowledge & AI Safety |
| MOD-22–23 | Analytics & FinOps |
| MOD-24–27 | Operations & Reliability |
| MOD-28–30 | Platform Engineering |
