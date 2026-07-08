# Version History

**Product:** AI Project Manager (AIPM)

---

## Blueprint bundle

| Version | Date | Summary |
|---------|------|---------|
| **1.8.0** | 2026-07-07 | TAD-AIPM-001 + ADR-TECH-001 LOCKED technology stack (.NET, PostgreSQL, etc.) |
| **1.7.0** | 2026-07-07 | IAD-AIPM-PH1-001 — Implementation Phase 1 architecture APPROVED |
| **1.6.0** | 2026-07-07 | DM-AIPM-001 APPROVED FROZEN; ADR-GOV-007 domain stability |
| **1.5.0** | 2026-07-07 | EIM-AIPM-001 APPROVED; ADR-GOV-006 canonical concepts; frozen roadmap; quality gates |
| **1.4.0** | 2026-07-07 | Enterprise Information Model layer at 05; DDD deferred to 06; folders 21–25; quality gates |
| **1.3.0** | 2026-07-07 | BCM approved; Prompt 05+ methodology; ADR-GOV-003/004; enterprise DDD spec |
| **1.2.0** | 2026-07-07 | Business Capability Model inserted at 04; state machines at 07; folders 05–18 |
| **1.1.0** | 2026-07-07 | Project Constitution added; folders renumbered 03–16 |
| **1.0.0** | 2026-07-07 | Initial Engineering Blueprint: approved SRS + SAD, governance pack, folder structure |

## Document versions

| Document | ID | Version | Date | Status | Path |
|----------|-----|---------|------|--------|------|
| Software Requirements Specification | SRS-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED (locked) | `01-SRS/SRS-AI-Project-Manager.md` |
| Software Architecture Document | SAD-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `02-Architecture/SAD-AI-Project-Manager.md` |
| **Project Constitution** | PC-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `03-Project-Constitution/Project-Constitution.md` |
| Traceability Matrix | RTM-AIPM-001 | 1.0.0 | 2026-07-07 | Active | `01-SRS/Traceability-Matrix.md` |
| Vision | — | 1.0.0 | 2026-07-07 | Approved | `00-Governance/Vision.md` |
| Project Charter | — | 1.0.0 | 2026-07-07 | Approved | `00-Governance/Project-Charter.md` |
| Glossary | — | 1.0.0 | 2026-07-07 | Normative | `00-Governance/Glossary.md` |
| ADR Index | — | 1.0.0 | 2026-07-07 | Active | `00-Governance/ADR-Index.md` |

## Versioning policy

| Change type | Version bump | Approval |
|-------------|--------------|----------|
| Typo / clarity, no semantic change | Patch (x.x.+1) | Document owner |
| New FR/NFR or modified requirement | Minor (+1.0) | Product + Engineering + ARB |
| Breaking architecture invariant | Major (+1.0.0) | ARB + Security |
| Constitutional amendment (immutable articles) | Major (+1.0.0) | Unanimous ARB + Security + Product |
| New ADR superseding old | New ADR file; mark old **Superseded** | ARB |

## Planned versions

| Document | ID | Target version | Target folder | Dependency |
|----------|-----|----------------|---------------|------------|
| Business Capability Model | BCM-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `04-Business-Capability-Model/Business-Capability-Model.md` |
| Blueprint Authoring Methodology | BAM-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `00-Governance/Blueprint-Authoring-Methodology.md` |
| Prompt 05 Specification (EIM) | PROMPT-05-AIPM | 1.0.0 | 2026-07-07 | APPROVED | `05-Enterprise-Information-Model/PROMPT-05-SPECIFICATION.md` |
| Prompt 06 Specification (DDD) | PROMPT-06-AIPM | 1.1.0 | 2026-07-07 | APPROVED | `06-Domain-Model/PROMPT-06-SPECIFICATION.md` |
| Document Quality Gates | DQG-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `21-Quality/Document-Quality-Gates.md` |
| Enterprise Information Model | EIM-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED | `05-Enterprise-Information-Model/Enterprise-Information-Model.md` |
| Domain Model | DM-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED — FROZEN | `06-Domain-Model/Domain-Model.md` |
| Technology Architecture | TAD-AIPM-001 | 1.0.0 | 2026-07-07 | APPROVED — LOCKED | `24-Standards/Technology-Architecture.md` |
| ADR Technology Stack | ADR-TECH-001 | 1.0.0 | 2026-07-07 | APPROVED — LOCKED | `02-Architecture/ADR/ADR-TECH-001-Approved-Technology-Stack.md` |
| State Machines | SM-AIPM-001 | 1.0.0 | `07-State-Machines/` | DM-AIPM-001, EVT-AIPM-001 |
| Database Architecture | DBA-AIPM-001 | 1.0.0 | `08-Database/` | DM-AIPM-001, ADR-SAD-003 |
| API Specification | API-AIPM-001 | 1.0.0 | `09-APIs/` | DM-AIPM-001, SAD §87 |
| Agent Contracts | AC-AIPM-001 | 1.0.0 | `10-Agent-Contracts/` | BCM, DM, PC Article 28 |
