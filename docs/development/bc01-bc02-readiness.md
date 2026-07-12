# BC-02 Readiness Report

**Do not implement BC-02 in this freeze.** This document only describes reuse and a recommended first slice.

## Reusable infrastructure

| Asset | Location | Reuse for BC-02 |
|-------|----------|-----------------|
| Tenant scope + headers | `ITenantScope`, Host middleware | Required for all Requirement commands |
| MediatR CQRS pipeline | Host registration | Same pattern for Intake/Approve |
| `IdentityDbContext` + EF migrations | Infrastructure | New tables via **new migration only** |
| MassTransit publish abstraction | `IPortfolioEventPublisher` pattern | Introduce BC-02 event publisher or generalize |
| ProblemDetails / domain errors | SharedKernel + Host | Keep Validation/NotFound/Forbidden mapping |
| Auth policy `Bc10Admin` | Host | Reuse or add BC-02-specific policy when SoD requires |
| Architecture tests | `AIPM.Architecture.Tests` | Will catch illegal dependencies |

## Reusable CQRS patterns

- Command record + handler with `GetRequiredTenantId()`
- Query handlers returning DTOs
- Integration event records with MessageId/Correlation/Causation envelope
- Catalog-gap documentation under `docs/development/` when CMD/EVT missing

## Reusable repositories / endpoints patterns

- Tenant-scoped `FindAsync(tenantId, id)` / `ListByTenantAsync`
- Minimal API group with `RequireAuthorization` + MediatR
- Host request DTOs only when path params need remapping (`UpdateProjectRequest` pattern)

## Reusable event patterns

- Domain event on aggregate → persist → publish integration event after `SaveChangesAsync`
- Do not dual-publish Created + Cloned-style events unless blueprint requires

## Recommended first BC-02 vertical slice

**Intake Requirement (CMD-030 / AGG-005 / CON-013 / CAP-005 / FR-010)** under an active Project:

1. Domain: `Requirement` aggregate (or entity if catalog dictates) with Draft intake  
2. Application: `IntakeRequirementCommand` + DTO + tenant checks against Project  
3. Infrastructure: new table + migration; repository  
4. Host: `POST /api/v1/projects/{projectId}/requirements` (illustrative)  
5. Tests: domain + handler + host + cross-tenant  
6. **Out of first slice:** ApproveRequirement (CMD-031), ambiguity detection, external ticket ingest

**Prerequisite:** Confirm Workspace/Project active rules and whether Requirement is AGG-005 root (Domain Model) before coding — stop on ambiguity as in BC-01 practice.

## What not to reuse blindly

- Portfolio projection live-SQL approach (Requirements may need different read models)
- Treating ScopeChange “citation string” as a real Requirement FK (BC-02 must introduce real links carefully)
