# Traceability Before New Concepts

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

From Prompt 05 onward, blueprint documents directly influence database design, APIs, agent contracts, and workflow implementation. AI-assisted authoring risks introducing concepts not grounded in approved capabilities, requirements, or architecture decisions—causing scope creep that bypasses governance.

## Decision

**Permanent rule for all blueprint documents from `05-Domain-Model` through `18-Roadmap`:**

> No new concept may be introduced unless it is traceable to an approved **business capability** (BCM CAP-###), **SRS requirement** (FR/NFR/SC/CON), **constitutional article** (PC-AIPM-001), or **accepted ADR**.
>
> If a genuinely new concept is required, it MUST first be proposed as a **new ADR**, reviewed at ARB, and accepted before inclusion in any downstream document.

**Traceability minimum per new concept:**

| Field | Required |
|-------|----------|
| Concept name | Yes |
| Parent capability (CAP-###) | Yes, when business-facing |
| SRS reference | Yes, when requirement-driven |
| SAD module / BC reference | Yes, when architecture-aligned |
| ADR reference | Yes, when decision-driven |
| Proposing document | Yes |

## Consequences

+ Prevents undocumented scope expansion in AI-generated specs.  
+ Forces architectural decisions into ADR trail.  
+ Keeps Domain Model, Events, and State Machines aligned with BCM and SRS.  
- Additional ADR overhead for novel concepts (intentional friction).

## Compliance

- Authors MUST run traceability audit before marking any `05+` document APPROVED.  
- Reviewers MUST reject documents containing untraceable concepts.  
- Cursor rule: `.cursor/rules/aipm-blueprint-traceability.mdc` enforces this in IDE sessions.

## References

- [Blueprint Authoring Methodology](../../00-Governance/Blueprint-Authoring-Methodology.md)
- [Business Capability Model](../../04-Business-Capability-Model/Business-Capability-Model.md) — BCM-AIPM-001
- PC-AIPM-001 Section 1 (Purpose)
- ADR-GOV-002 (BCM before DDD)
