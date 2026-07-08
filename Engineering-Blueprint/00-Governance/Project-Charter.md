# Project Charter

**Product:** AI Project Manager (AIPM)  
**Document:** Project Charter  
**Version:** 1.0.0  
**Status:** Approved  
**Parent:** SRS-AIPM-001 v1.0.0

---

## Purpose

Authorize and bound the engineering program to build AIPM as a production-grade, multi-tenant enterprise platform—the control plane for an autonomous AI software engineering company.

## Objectives (GA)

| ID | Objective |
|----|-----------|
| OBJ-001 | Support ≥1,000 tenants architecture capacity |
| OBJ-002 | Orchestrate ≥14 agent types via stable contracts |
| OBJ-003 | 100% decision events logged (auditable timeline) |
| OBJ-004 | Configurable mandatory human gates for production |
| OBJ-005 | ≥5 certified external integrations |
| OBJ-006 | ≥90% task dependency accuracy (defined metric) |
| OBJ-007 | Full incident replay within retention window |
| OBJ-008 | 99.9% monthly API availability |

## Scope (summary)

**In scope:** Project/portfolio lifecycle, planning, agent orchestration, policy/gates, integrations (VCS, CI/CD, issue trackers), audit/compliance, multi-tenant RBAC/ABAC, cost governance.

**Out of scope:** PM-generated application code, hosting customer production apps, LLM training, replacing full ITSM suites, guaranteed fully autonomous delivery without human override option.

## Stakeholders

| Stakeholder | Interest |
|-------------|----------|
| Customer executive sponsor | ROI, risk reduction |
| Engineering director | Delivery throughput |
| Platform engineering | Build and operate AIPM |
| Agent team owners | Contract stability |
| Security & compliance | Data protection, audit |
| Legal / privacy | DPA, subprocessors |

## Governance

- **Architecture Review Board (ARB)** — structural changes, security veto on autonomy and data handling.
- **Change control** — SRS changes via formal CR; architecture changes via ADR.
- **Approvers (pending):** Engineering, Security, Product, Legal.

## Success criteria (release gate)

- SC-001 through SC-008 per SRS §11 (availability, security, audit, golden path, scale, cost control, integration, halt).

## Constraints

- CON-001: PM must not execute application codebase changes.
- CON-002: Enterprise SSO and tenant isolation mandatory.
- CON-005: Air-gapped dedicated deployments required for select customers.
- CON-006: No per-customer core forks.

## Target customers

Mid-market and enterprise first (ADR-002).

## Deployment model

Hybrid SaaS default + dedicated enterprise tier + air-gapped profile (ADR-001, ADR-SAD-005).
