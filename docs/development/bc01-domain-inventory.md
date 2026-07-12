# BC-01 Domain Inventory

## Aggregates

| ID | Type | File | Blueprint |
|----|------|------|-----------|
| AGG-002 | `PortfolioAggregate` | `src/AIPM.Domain/Portfolio/Portfolio.cs` | CON-006, CAP-002, FR-003, FR-122 |
| AGG-003 | `ProgramAggregate` | `src/AIPM.Domain/Portfolio/Program.cs` | CON-007, CAP-002, FR-003 |
| AGG-004 | `ProjectAggregate` | `src/AIPM.Domain/Portfolio/Project.cs` | CON-008, CAP-001/004, FR-001/004/005 |

## Entities

| Type | Owner | File | Blueprint |
|------|-------|------|-----------|
| `ScopeChange` | AGG-004 | `src/AIPM.Domain/Portfolio/ScopeChange.cs` | CON-011, CAP-004, FR-004, CMD-022 (no ENT-### in catalog) |

## Value objects

| Type | Notes |
|------|-------|
| _(none in BC-01 Portfolio)_ | IDs and names are primitives/`Guid`; SharedKernel `TenantId` used at application boundary |

## Enums (domain)

| Type | Values | Trace |
|------|--------|-------|
| `ProjectStatus` | Draft, Active, OnHold, Completed, Archived | CON-008 |
| `ScopeChangeStatus` | Proposed, Approved, Rejected, Implemented | CON-011 |

## Repositories (application contracts → infrastructure)

| Interface | Implementation | Trace |
|-----------|----------------|-------|
| `IPortfolioRepository` | `PortfolioRepository` | AGG-002, FR-003 |
| `IProgramRepository` | `ProgramRepository` | AGG-003, FR-003 |
| `IProjectRepository` | `ProjectRepository` | AGG-004, FR-001/004 |
| `IPortfolioProjectionReadRepository` | `PortfolioProjectionReadRepository` | FR-122, ADR-SAD-008 |

## Domain events

| Type | Catalog | Trace |
|------|---------|-------|
| `PortfolioCreatedDomainEvent` | EVT-010 | CMD-010, FR-003 |
| `ProgramCreatedDomainEvent` | EVT-011 | CMD-011, FR-003 |
| `ProjectCreatedDomainEvent` | EVT-020 | CMD-020, FR-001 |
| `ProjectArchivedDomainEvent` | EVT-021 | CMD-021, FR-001 |
| `ProjectClonedDomainEvent` | gap ≈ EVT-023 | FR-005 |
| `ScopeChangeRecordedDomainEvent` | gap ≈ EVT-022 | CMD-022, FR-004 |
| `ScopeChangeApprovedDomainEvent` | — | FR-004 |
| `ScopeChangeRejectedDomainEvent` | — | FR-004 |
| `ScopeChangeImplementedDomainEvent` | — | FR-004 / CON-011 |

## Integration events

| Type | Catalog | Trace |
|------|---------|-------|
| `PortfolioCreatedIntegrationEvent` | EVT-010 | ADR-SAD-004 |
| `ProgramCreatedIntegrationEvent` | EVT-011 | ADR-SAD-004 |
| `ProjectCreatedIntegrationEvent` | EVT-020 | ADR-SAD-004 |
| `ProjectArchivedIntegrationEvent` | EVT-021 | ADR-SAD-004 |
| `ProjectClonedIntegrationEvent` | gap | FR-005, ADR-SAD-004 |
| `ScopeChangeRecordedIntegrationEvent` | gap | FR-004, ADR-SAD-004 |
| `ScopeChangeApprovedIntegrationEvent` | — | FR-004 |
| `ScopeChangeRejectedIntegrationEvent` | — | FR-004 |
| `ScopeChangeImplementedIntegrationEvent` | — | FR-004 |

## Publisher

| Contract | Implementation |
|----------|----------------|
| `IPortfolioEventPublisher` | `PortfolioEventPublisher` (MassTransit) |
