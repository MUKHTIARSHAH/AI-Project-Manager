# BC-01 API Inventory

**Auth policy (all routes below):** `Bc10Admin`  
**Tenant enforcement:** `X-Tenant-Id` required; MediatR handlers use `ITenantScope.GetRequiredTenantId()`; persistence filtered by tenant.

| Method | Route | Request | Response | FR |
|--------|-------|---------|----------|-----|
| GET | `/api/v1/portfolio` | — | `PortfolioDto[]` | FR-003 |
| GET | `/api/v1/portfolio/{portfolioId}` | — | `PortfolioDto` | FR-003 |
| GET | `/api/v1/portfolio/{portfolioId}/summary` | — | `PortfolioSummaryDto` | FR-122 |
| POST | `/api/v1/portfolio` | `CreatePortfolioCommand` `{ name }` | `PortfolioDto` | FR-003 |
| GET | `/api/v1/programs` | — | `ProgramDto[]` | FR-003 |
| GET | `/api/v1/programs/{programId}` | — | `ProgramDto` | FR-003 |
| GET | `/api/v1/programs/{programId}/summary` | — | `ProgramSummaryDto` | FR-122 |
| POST | `/api/v1/programs` | `CreateProgramCommand` `{ portfolioId, name }` | `ProgramDto` | FR-003 |
| GET | `/api/v1/projects` | — | `ProjectDto[]` | FR-001 |
| GET | `/api/v1/projects/{projectId}` | — | `ProjectDto` | FR-001 |
| GET | `/api/v1/projects/{projectId}/summary` | — | `ProjectSummaryDto` | FR-122 |
| POST | `/api/v1/projects` | `CreateProjectCommand` `{ programId, workspaceId, ownerUserId, name }` | `ProjectDto` | FR-001 |
| PUT | `/api/v1/projects/{projectId}` | `UpdateProjectRequest` `{ workspaceId, ownerUserId, name }` | `ProjectDto` | FR-001 |
| POST | `/api/v1/projects/{projectId}/archive` | — | `ProjectDto` | FR-001 |
| POST | `/api/v1/projects/{projectId}/clone` | `CloneProjectRequest` `{ name }` | `CloneProjectResponse` | FR-005 |
| GET | `/api/v1/projects/{projectId}/scope-changes` | — | `ScopeChangeDto[]` | FR-004 |
| GET | `/api/v1/projects/{projectId}/scope-changes/{scopeChangeId}` | — | `ScopeChangeDto` | FR-004 |
| POST | `/api/v1/projects/{projectId}/scope-changes` | `RecordScopeChangeRequest` `{ title, description, affectedRequirementCitation? }` | `ScopeChangeDto` | FR-004 |
| POST | `/api/v1/projects/{projectId}/scope-changes/{scopeChangeId}/approve` | — | `ScopeChangeDto` | FR-004 |
| POST | `/api/v1/projects/{projectId}/scope-changes/{scopeChangeId}/reject` | — | `ScopeChangeDto` | FR-004 |
| POST | `/api/v1/projects/{projectId}/scope-changes/{scopeChangeId}/implement` | — | `ScopeChangeDto` | FR-004 |

## DTO field reference

**PortfolioDto:** `id`, `tenantId`, `name`, `createdAt`  
**ProgramDto:** `id`, `tenantId`, `portfolioId`, `name`, `createdAt`  
**ProjectDto:** `id`, `tenantId`, `programId`, `workspaceId`, `ownerUserId`, `name`, `status`, `createdAt`, `archivedAt`  
**CloneProjectResponse:** `sourceProjectId` + ProjectDto-equivalent clone fields  
**ScopeChangeDto:** `id`, `projectId`, `tenantId`, `title`, `description`, `affectedRequirementCitation`, `status`, `recordedAt`, `decidedAt`  
**PortfolioSummaryDto:** `portfolioId`, `portfolioName`, `programCount`, `projectCount`, status counts (draft/active/onHold/completed/archived)  
**ProgramSummaryDto:** `programId`, `programName`, `portfolioId`, `projectCount`, status counts  
**ProjectSummaryDto:** `projectId`, `projectName`, `programId`, `portfolioId`, `ownerUserId`, `workspaceId`, `status`, `scopeChangeCount`

**ProblemDetails:** domain `ValidationError` / `NotFoundError` / `ForbiddenError` mapped by existing Host middleware.
