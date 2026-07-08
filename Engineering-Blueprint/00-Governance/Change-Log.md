# Change Log

**Product:** AI Project Manager (AIPM)  
**Status:** Active

All notable changes to blueprint documents. Requirement and architecture version bumps require ARB review.

---

## [1.8.0] — 2026-07-07

### Added

- `24-Standards/Technology-Architecture.md` — TAD-AIPM-001 APPROVED LOCKED.
- `02-Architecture/ADR/ADR-TECH-001-Approved-Technology-Stack.md` — official stack; supersedes ADR-008 runtime choices.

### Changed

- ADR-Index — ADR-TECH-001 indexed.
- Coding-Standards-Phase1 — .NET stack per ADR-TECH-001.
- README — technology architecture locked; bundle v1.8.0.

### Rationale

Permanent technology reference before M1 implementation. .NET enterprise alignment.

---

## [1.9.0] — 2026-07-07

### Added

- `02-Architecture/ADR/ADR-TECH-002-M6-admin-shell-ai-foundation.md` — accepted M6 scope refinement under frozen roadmap governance.

### Changed

- ADR-Index — ADR-TECH-002 indexed.
- Decision-Log — D-021 recorded.

### Rationale

Preserve milestone ordering while prioritizing provider-independent AI foundation work inside M6 and keeping UI scope minimal.

---

## [1.7.0] — 2026-07-07

### Added

- `16-Implementation/Implementation-Architecture-Phase1.md` — IAD-AIPM-PH1-001 APPROVED.
- `24-Standards/Coding-Standards-Phase1.md` — developer and AI agent rules.
- Implementation Phase 1: monorepo layout, 7 milestones, no business logic.

### Changed

- README — implementation phase transition; bundle v1.7.0.

### Rationale

Bridge Blueprint to source code. Foundation before BC implementation.

---

## [1.6.0] — 2026-07-07

### Added

- **DM-AIPM-001** — canonical domain specification: 15 BCs, 25 aggregates, CMD/EVT catalogs, tactical catalog.
- `06-Domain-Model/` full artifact set (10 files).
- ADR-GOV-007 — Domain Stability Rule (frozen DM; ADR for changes).
- PROMPT-06-AIPM v2.0.0 — enterprise DDD scope; Domain Purity Audit.

### Changed

- README — DM APPROVED FROZEN; Domain Events next; bundle v1.6.0.
- Cursor rule — ADR-GOV-007 enforcement.
- Governance methodology freeze — no new mandatory doc types before DDD (completed).

### Rationale

Shift from governance expansion to system design expansion. DM is foundation for 07–18.

---

## [1.5.0] — 2026-07-07

### Added

- **EIM-AIPM-001** — full Enterprise Information Model (64 concepts CON-001–064).
- `05-Enterprise-Information-Model/` artifact set: Concept Catalog, Relationship Map, Glossary, Traceability, IDL, Assumption Register, Open Questions.
- ADR-GOV-006 — canonical one-definition/one-owner rule; frozen roadmap; ASS/Q registers mandatory.
- PROMPT-05-AIPM v1.1.0 — rich concept schema; Terminology Consistency Audit.
- `25-Templates/` Assumption and Open Questions templates.

### Changed

- README — EIM APPROVED; Prompt 06 (DDD) next; bundle v1.5.0.
- Blueprint Authoring Methodology — ADR-GOV-006; mandatory registers.

### Rationale

Stabilize business vocabulary before DDD. World-class information architecture discipline.

---

## [1.4.0] — 2026-07-07

### Added

- `05-Enterprise-Information-Model/` — Information Architecture layer (conceptual business concepts, not database/DDD).
- `05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md` — Prompt 05 is now Enterprise Information Model.
- `21-Quality/Document-Quality-Gates.md` — eleven quality gates (QG-01–QG-11) before APPROVED.
- `22-Risk/`, `23-Decisions/`, `24-Standards/`, `25-Templates/` — implementation-support folders.
- `02-Architecture/ADR/ADR-GOV-005-eim-before-ddd.md` — EIM before DDD; DDD paused until EIM approved.

### Changed

- **Folder renumbering:** `05-Domain-Model` → `06-Domain-Model`; `07-State-Machines` → `08-State-Machines`; Roadmap `18` → `19`; all intermediate folders created at new numbers.
- DDD spec moved to `06-Domain-Model/PROMPT-06-SPECIFICATION.md` (Prompt 06).
- [Blueprint-Authoring-Methodology.md](Blueprint-Authoring-Methodology.md) v1.1.0 — BA → IA → AA → TA flow; quality gates.
- [README.md](../README.md) — bundle v1.4.0; control-plane framing; EIM next.
- `.cursor/rules/aipm-blueprint-traceability.mdc` — CON-### trace; layer order.

### Rationale

Insert Information Architecture between BCM and DDD to stabilize business vocabulary before aggregates, commands, events, database, and APIs. Prevents costly late redesign.

---

## [1.3.0] — 2026-07-07

### Added

- `04-Business-Capability-Model/Business-Capability-Model.md` — BCM-AIPM-001 v1.0.0 **APPROVED** (56 capabilities CAP-001–056).
- `00-Governance/Blueprint-Authoring-Methodology.md` — BAM-AIPM-001; permanent traceability rule for Prompt 05+.
- `05-Domain-Model/PROMPT-05-SPECIFICATION.md` — enterprise DDD authoring scope (not simplified class diagram).
- `02-Architecture/ADR/ADR-GOV-003-traceability-before-new-concepts.md` — no new concepts without CAP/SRS/PC/ADR; ADR first.
- `02-Architecture/ADR/ADR-GOV-004-command-event-state-linkage.md` — mandatory CMD/EVT linkage for state transitions.
- `.cursor/rules/aipm-blueprint-traceability.mdc` — IDE enforcement of ADR-GOV-003.

### Changed

- [README.md](../README.md) — bundle v1.3.0; BCM approved; Prompt 05 methodology; ADR-GOV-003/004.
- [ADR-Index.md](ADR-Index.md) — ADR-GOV-003, ADR-GOV-004 indexed.

### Rationale

Transition from business architecture to implementation-influencing specs requires anti-scope-creep discipline and full enterprise DDD (not entities-only). State machines and domain events stay separate but must link to commands and events.

---

## [1.2.0] — 2026-07-07

### Added

- `04-Business-Capability-Model/` — business WHAT layer before technical DDD.
- `07-State-Machines/` — explicit lifecycle specs separated from domain model and events.

### Changed

- **Folder renumbering** to insert BCM and separate state machines:
  - `04-Domain-Model` → `05-Domain-Model`
  - `05-Database` → `08-Database`
  - `06-Events` → `06-Domain-Events`
  - `10-Workflow` → `12-Workflow-Engine`
  - `11-Security` → `13-Security` through `16-Roadmap` → `18-Roadmap`
- [README.md](../README.md) — BCM-before-DDD dependency order; bundle v1.2.0.
- **Prompt 04** is now Business Capability Model (not Domain Model).

### Rationale

Prevent mixing business capabilities with technical implementation in large AI orchestration systems.

---

## [1.1.0] — 2026-07-07

### Added

- `03-Project-Constitution/Project-Constitution.md` — PC-AIPM-001 v1.0.0 (**APPROVED**). Immutable project law binding all documents, agents, and implementation.

### Changed

- **Folder renumbering:** `03-Domain-Model` → `04-Domain-Model` through `15-Roadmap` → `16-Roadmap` to insert Constitution at `03`.
- [README.md](../README.md) — updated dependency order, precedence rules, repository status.
- Document dependency: Constitution required before Domain Model and all downstream specs.

### Document status (updated)

| Document | ID | Version | Status |
|----------|-----|---------|--------|
| Project Constitution | PC-AIPM-001 | 1.0.0 | APPROVED |
| SRS | SRS-AIPM-001 | 1.0.0 | APPROVED (locked) |
| SAD | SAD-AIPM-001 | 1.0.0 | APPROVED |

---

## [1.0.0] — 2026-07-07

### Added

- `00-Governance/` — Vision, charter, glossary, ADR index, decision log, change log, version history.
- `01-SRS/SRS-AI-Project-Manager.md` — SRS-AIPM-001 v1.0.0 (**APPROVED**).
- `01-SRS/Traceability-Matrix.md` — FR/NFR to architecture module mapping.
- `02-Architecture/SAD-AI-Project-Manager.md` — SAD-AIPM-001 v1.0.0 (**APPROVED**).
- `02-Architecture/ADR/` — Individual ADR records (ADR-001–008, ADR-SAD-001–010).
- `02-Architecture/Architecture-Diagrams/` — Placeholder for visual artifacts.
- `04-Domain-Model/` through `16-Roadmap/` — Reserved for downstream documents.
- `Assets/` — Shared diagrams and exports.
- `README.md` — Blueprint index and rules.

### SRS changes during review (incorporated in v1.0.0)

- Added FR-115 through FR-122 (maintenance, halt credentials, explainability, audit isolation, portfolio aggregates).
- Added NFR-021 through NFR-023 (tiered availability, event backbone capacity, agent ack SLA).
- Added FR-120 (region pinning).
- Resolved ambiguities AMB-001 through AMB-008 via ADR-001 through ADR-008.

### Architecture audit fixes (SAD v1.0.0)

- Admin console documented as Experience Layer on DU-1.
- Circular dependency analysis completed (no cycles).
- Break-glass identity flow documented.
- Air-gapped LLM routing constraint documented.

### Document status

| Document | ID | Version | Status |
|----------|-----|---------|--------|
| SRS | SRS-AIPM-001 | 1.0.0 | APPROVED (locked) |
| SAD | SAD-AIPM-001 | 1.0.0 | APPROVED |

---

## Change request template

When modifying locked documents:

1. Open CR with: document ID, section affected, rationale, impact on RTM.
2. Obtain ARB approval (architecture) or product + engineering approval (requirements).
3. Bump version per [Version-History.md](Version-History.md).
4. Record ADR if architectural.
5. Update this change log and traceability matrix.
