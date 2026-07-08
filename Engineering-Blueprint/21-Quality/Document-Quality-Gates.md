# Document Quality Gates

**Document ID:** DQG-AIPM-001  
**Version:** 1.0.0  
**Date:** 2026-07-07  
**Status:** APPROVED  
**Applies to:** All blueprint documents seeking **APPROVED** status

---

## Purpose

As the blueprint approaches implementation-influencing artifacts, every document MUST pass formal quality gates before approval. This prevents inconsistent, untraceable, or premature specifications from becoming engineering law.

---

## Approval pipeline

```
Draft → In Review → [11 Quality Gates] → APPROVED
```

No document may be marked **APPROVED** until **all applicable gates** record **PASS**.

---

## The eleven quality gates

| Gate | ID | Owner | Pass criteria |
|------|-----|-------|---------------|
| **Business Review** | QG-01 | Product / Business Architecture | Aligns with BCM capabilities and business intent; no scope creep |
| **Architecture Review** | QG-02 | ARB | Aligns with SAD modules, bounded contexts, ADRs; structural consistency |
| **Governance Review** | QG-03 | Governance Council | PC-AIPM-001 invariants preserved; fail-closed and human sovereignty intact |
| **Consistency Review** | QG-04 | Document owner + peer | No internal contradictions; cross-references resolve |
| **Traceability Review** | QG-05 | Engineering | Every concept maps to CAP, FR/NFR, PC, or ADR (ADR-GOV-003) |
| **Terminology Review** | QG-06 | Business + Domain | Terms match Information Glossary / Domain Glossary; no synonym drift |
| **Future Expansion Review** | QG-07 | ARB + Product | Roadmap items flagged; no premature implementation of future features |
| **AI Review** | QG-08 | AI Safety / Platform | AI governance, autonomy, explainability, output validation addressed where relevant |
| **Security Review** | QG-09 | Security | Tenant isolation, IAM, data residency, secrets, audit — per document scope |
| **Documentation Review** | QG-10 | Technical Writing | Complete sections, IDs, status block, references, readable structure |
| **Approval Review** | QG-11 | Named approver | Sign-off recorded in Change-Log and Version-History |

---

## Gate applicability matrix

| Document tier | Mandatory gates |
|---------------|-----------------|
| `04` BCM | QG-01, QG-03, QG-05, QG-10, QG-11 |
| `05` EIM | QG-01–07, QG-10, QG-11 |
| `06` DDD | QG-01–08, QG-10, QG-11 |
| `07`–`08` Events / FSM | QG-02, QG-04, QG-05, QG-06, QG-10, QG-11 |
| `09`–`12` Data / API / Agents / Workflow | QG-02, QG-05, QG-09, QG-10, QG-11 |
| `13`–`18` Security / Ops / Impl | QG-02, QG-03, QG-09, QG-10, QG-11 |
| `21`–`25` Support folders | QG-04, QG-10, QG-11 |

---

## Gate record template

Each approved document SHOULD include:

```markdown
## Quality Gate Record

| Gate | Result | Reviewer | Date |
|------|--------|----------|------|
| QG-01 Business | PASS | | |
| ... | | | |
```

---

## Relationship to other governance

| Artifact | Role |
|----------|------|
| ADR-GOV-003 | Traceability rule (feeds QG-05) |
| ADR-GOV-004 | CMD/EVT linkage (feeds QG-04 for `07`/`08`) |
| ADR-GOV-005 | EIM before DDD (feeds QG-01, QG-06) |
| BAM-AIPM-001 | Authoring methodology and validation audits |

---

## References

- [Blueprint Authoring Methodology](../00-Governance/Blueprint-Authoring-Methodology.md)
- PC-AIPM-001 Section 1
