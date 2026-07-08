# Implementation ADRs and decisions

Canonical ADRs live in the locked blueprint:

`Engineering-Blueprint/02-Architecture/ADR/`

| ADR | Topic |
|-----|-------|
| [ADR-TECH-001](../../Engineering-Blueprint/02-Architecture/ADR/ADR-TECH-001-Approved-Technology-Stack.md) | Approved stack (.NET 9, PostgreSQL, …) |
| [ADR-TECH-002](../../Engineering-Blueprint/02-Architecture/ADR/ADR-TECH-002-M6-admin-shell-ai-foundation.md) | M6 AI foundation scope refinement |
| [ADR-GOV-007](../../Engineering-Blueprint/02-Architecture/ADR/ADR-GOV-007-Domain-Model-Frozen.md) | Domain model frozen |

New implementation ADRs must be added to the blueprint ADR folder and indexed in `Engineering-Blueprint/00-Governance/ADR-Index.md`.

## Implementation decision log

| Date | ID | Decision | Rationale | Reference |
|------|-----|----------|-----------|-----------|
| 2026-07-07 | IMPL-D-001 | Phase 1 M1–M6 platform-only scope | Prove orchestration before business BCs | [milestone-progress.md](../development/milestone-progress.md) |
| 2026-07-07 | IMPL-D-002 | M6 includes AI abstraction, not providers | ADR-TECH-002; prepare P2 without vendor lock-in | [m6-admin-shell-refined.md](../development/m6-admin-shell-refined.md) |
| 2026-07-08 | IMPL-D-003 | P2-M1 BC-10 SQLite identity for dev | Fast local iteration; PostgreSQL path per ADR-TECH-001 later | [p2-m1-identity-access-core.md](../development/p2-m1-identity-access-core.md) |
| 2026-07-08 | IMPL-D-004 | P2-M1 API key auth for BC-10 endpoints | Fail-closed minimum auth until IdP milestone | [p2-m1-identity-access-core.md](../development/p2-m1-identity-access-core.md) |
| 2026-07-08 | IMPL-D-005 | P2-M1 complete — no P2-M2 scope mixed in | Milestone isolation per governance | [CHANGELOG.md](../../CHANGELOG.md) |
| 2026-07-08 | IMPL-D-006 | Pre-P2-M2 hardening sprint accepted | H-01–H-09 complete; 76 tests; foundation frozen | [CHANGELOG.md](../../CHANGELOG.md#021--2026-07-08) |
| 2026-07-08 | IMPL-D-007 | Tenant-scoped ops require `X-Tenant-Id` | Fail-closed fix for AUD-DM-01; platform ops remain header-optional | [CHANGELOG.md](../../CHANGELOG.md#fixed) |

Blueprint decision log: [Decision-Log.md](../../Engineering-Blueprint/00-Governance/Decision-Log.md)

This folder holds **implementation notes** only — not substitute ADRs.
