# Decision Log

**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Status:** Active

Chronological record of significant engineering decisions. Detailed rationale lives in linked ADRs.

---

| Date | ID | Decision | Rationale (summary) | Document |
|------|-----|----------|---------------------|----------|
| 2026-07-07 | D-001 | Approve SRS-AIPM-001 v1.0.0 | Complete requirements foundation for enterprise AIPM | [SRS](../01-SRS/SRS-AI-Project-Manager.md) |
| 2026-07-07 | D-002 | Hybrid SaaS + dedicated tier | Balance scale economics with enterprise isolation needs | ADR-001 |
| 2026-07-07 | D-003 | Enterprise-first segment | Security and governance drive adoption, not SMB volume | ADR-002 |
| 2026-07-07 | D-004 | Mandatory production gates | Autonomous delivery unsafe without human-gated prod path | ADR-003 |
| 2026-07-07 | D-005 | Model-agnostic LLM router | Avoid vendor lock-in; survive provider outages | ADR-004 |
| 2026-07-07 | D-006 | AIPM as authoritative state | Orchestration requires single system of record | ADR-005 |
| 2026-07-07 | D-007 | US + EU at launch | Broadest enterprise addressability with bounded scope | ADR-006 |
| 2026-07-07 | D-008 | Isolated agent runtimes | Separation of duties and blast-radius control | ADR-007 |
| 2026-07-07 | D-009 | Go/Java + TypeScript stack | Enterprise familiarity, concurrency, typing for control plane | ADR-008 |
| 2026-07-07 | D-010 | Approve SAD-AIPM-001 v1.0.0 | Architecture baseline for all implementation | [SAD](../02-Architecture/SAD-AI-Project-Manager.md) |
| 2026-07-07 | D-011 | Five deployable units at GA | Modular monolith balance; not 20+ microservices day one | ADR-SAD-001 |
| 2026-07-07 | D-012 | Event sourcing for audit paths | SC-003 reconstructability requirement | ADR-SAD-004, ADR-SAD-008 |
| 2026-07-07 | D-013 | Fail-closed policy engine | Enterprise safety default for autonomous systems | ADR-SAD-009 |
| 2026-07-07 | D-014 | Task-scoped credential broker | Least privilege for agent dispatch; FR-117 | ADR-SAD-010 |
| 2026-07-07 | D-015 | Engineering Blueprint structure | Ordered documentation hierarchy for downstream work | [README](../README.md) |
| 2026-07-07 | D-016 | Insert Project Constitution at folder 03 | Prevent contradictions at scale; supreme operational law before domain model | [Constitution](../03-Project-Constitution/Project-Constitution.md) |
| 2026-07-07 | D-018 | Insert Business Capability Model at folder 04 | Separate business WHAT from technical HOW before DDD | [README](../README.md) |
| 2026-07-07 | D-019 | Split State Machines to folder 07 | Lifecycles distinct from aggregates and event catalog | [Change-Log](Change-Log.md) |
| 2026-07-07 | D-020 | Renumber folders 04–16 → 04–18 | Accommodate BCM and state machines | [Change-Log](Change-Log.md) |
| 2026-07-07 | D-021 | Refine M6 scope without reordering | Keep frozen roadmap; implement AI abstraction foundation plus minimal admin shell connectivity checks | ADR-TECH-002 |
| 2026-07-08 | D-022 | Complete P2-M1 BC-10 Identity Core | First business BC delivered per Phase 2 plan; tenant/user/role RBAC foundation | [p2-m1-identity-access-core.md](../../../docs/development/p2-m1-identity-access-core.md) |
| 2026-07-08 | D-023 | Accept Pre-P2-M2 Hardening Sprint (H-01–H-09) | Close audit findings before P2-M2; FK constraints, tenant scope, arch tests, idempotency, plugin security | [CHANGELOG.md](../../../CHANGELOG.md#021--2026-07-08) |
| 2026-07-08 | D-024 | Tenant isolation fail-closed (AUD-DM-01) | `EnsureMatches()` must reject missing `X-Tenant-Id` on tenant-scoped mutations; platform-scoped ops remain header-optional | [CHANGELOG.md](../../../CHANGELOG.md#021--2026-07-08) |
| 2026-07-08 | D-025 | Freeze foundation before P2-M2 | No further refactoring unless required by future milestones; tag and commit hardening baseline | [milestone-progress.md](../../../docs/development/milestone-progress.md) |

---

## Pending decisions

| Topic | Options under consideration | Target |
|-------|----------------------------|--------|
| Active-active multi-region | Warm standby vs active-active writes | Post-GA enterprise tier |
| Public agent marketplace | Certification pipeline timing | GA + 6 months |
| Native graph database | Projection-only vs dedicated graph engine | When traversal SLO breached |
