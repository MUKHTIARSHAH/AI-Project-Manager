# Prompt 05 — Enterprise Information Model Specification

**Document ID:** PROMPT-05-AIPM  
**Version:** 1.1.0  
**Date:** 2026-07-07  
**Status:** APPROVED  
**Output target:** `05-Enterprise-Information-Model/` (full artifact set)

---

## Permanent rules

| Rule | Source |
|------|--------|
| No new concept without CAP/FR/PC/ADR trace | ADR-GOV-003 |
| EIM before DDD | ADR-GOV-005 |
| **Exactly one canonical definition and one canonical owner per concept; no duplicates or undocumented overlap** | ADR-GOV-006 |
| Frozen roadmap `00`–`19`, `21`–`25` | ADR-GOV-006 |

---

## Output artifacts

```
05-Enterprise-Information-Model/
├── Enterprise-Information-Model.md
├── Concept-Catalog.md
├── Concept-Relationship-Map.md
├── Information-Glossary.md
├── Traceability-Matrix.md
├── Information-Decision-Log.md
├── Assumption-Register.md          ← mandatory (ASS-###)
└── Open-Questions-Register.md      ← mandatory (Q-###)
```

---

## Concept schema (every CON-###)

Each concept MUST define **all** attributes:

| # | Attribute |
|---|-----------|
| 1 | Concept ID (CON-###) |
| 2 | Canonical Name |
| 3 | Canonical Definition |
| 4 | Business Purpose |
| 5 | Business Meaning |
| 6 | Business Owner |
| 7 | Source Capability (CAP-###) |
| 8 | Source Requirement (FR/NFR/SC/CON) |
| 9 | Related ADRs |
| 10 | Parent Concepts |
| 11 | Child Concepts |
| 12 | Peer Concepts |
| 13 | Allowed Relationships |
| 14 | Forbidden Relationships |
| 15 | Cardinality Rules (business level) |
| 16 | Lifecycle Stages |
| 17 | Lifecycle Owner |
| 18 | Business Constraints |
| 19 | Business Policies |
| 20 | Security Classification |
| 21 | Privacy Classification |
| 22 | Retention Requirements |
| 23 | Audit Requirements |
| 24 | Quality Requirements |
| 25 | Synonyms (if any — must not replace canonical name) |
| 26 | Deprecated Terms |
| 27 | Examples |
| 28 | Non-examples |
| 29 | Future Evolution Notes |

**Minimum concepts:** ≥50 covering CAP-001–CAP-056.

---

## Mandatory registers

### Assumption Register (ASS-###)

| Field | Required |
|-------|----------|
| ASS ID | ASS-### |
| Description | |
| Why the assumption exists | |
| Evidence | |
| Risk if incorrect | |
| Validation plan | |
| Current status | Open / Validated / Invalidated |

### Open Questions Register (Q-###)

| Field | Required |
|-------|----------|
| Q ID | Q-### |
| Question | |
| Why it matters | |
| Options | |
| Recommended answer | |
| Decision deadline | |
| Resolution reference | ADR or approved doc |

---

## Terminology Consistency Audit (required before APPROVED)

| Check | Pass criteria |
|-------|---------------|
| No duplicate concepts | One CON per business meaning |
| No conflicting definitions | Glossary matches catalog |
| No undefined terms | Every used term in glossary |
| No unused definitions | Every glossary entry referenced |
| Capability trace | Every CON → CAP; every CAP → ≥1 CON |
| Bidirectional relationships | REL-### has inverse where appropriate |
| No orphan concepts | All CON in relationship map |
| Canonical owner | Exactly one owner per CON |
| Overlap documented | Merged or boundary in IDL |

---

## Approval

```
ENTERPRISE INFORMATION MODEL STATUS: APPROVED
```

Only after all audits and quality gates pass with zero issues.

---

## References

- ADR-GOV-003, ADR-GOV-005, ADR-GOV-006
- BCM-AIPM-001, DQG-AIPM-001
- [Blueprint Authoring Methodology](../00-Governance/Blueprint-Authoring-Methodology.md)
