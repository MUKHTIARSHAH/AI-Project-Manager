# P2-M1 — BC-10 Identity & Access Core

**Status:** COMPLETE  
**Date:** 2026-07-08  
**Bounded context:** BC-10 Identity & Access  
**Trace:** AGG-001 Tenant · CON-001, CON-004, CON-005 · CMD-001, CMD-002 · EVT-001, EVT-002

## Objective

Deliver the first Phase 2 business bounded context: identity and access primitives required by all downstream BCs, without authentication UI or external IdP integration.

## Features implemented

| Concept | Implementation | Layer |
|---------|----------------|-------|
| Tenant (AGG-001) | `Tenant` aggregate, provision/suspend | Domain |
| User (CON-004) | `User` aggregate, create, role assignment | Domain |
| Role (CON-005) | `Role` aggregate, permission assignment | Domain |
| Permission | `Permission` value object | Domain |
| RoleAssignment | `RoleAssignment` record on User | Domain |
| PermissionAssignment | `PermissionAssignment` on Role | Domain |
| IdentityPolicy | `IdentityPolicy` (fail-closed default) | Domain |

Business rules remain in the Domain layer. EF Core is isolated to Infrastructure persistence records and repositories.

## APIs added

All endpoints require `X-Api-Key` header and `Bc10Admin` authorization policy unless noted.

| Method | Path | Command/Query |
|--------|------|---------------|
| `GET` | `/api/v1/identity/tenants` | `ListTenantsQuery` |
| `POST` | `/api/v1/identity/tenants` | `ProvisionTenantCommand` |
| `POST` | `/api/v1/identity/tenants/{tenantId}/suspend` | `SuspendTenantCommand` |
| `GET` | `/api/v1/identity/users` | `ListUsersQuery` |
| `POST` | `/api/v1/identity/users` | `CreateUserCommand` |
| `GET` | `/api/v1/identity/roles` | `ListRolesQuery` |
| `POST` | `/api/v1/identity/roles` | `CreateRoleCommand` |
| `POST` | `/api/v1/identity/users/{userId}/roles/{roleId}` | `AssignRoleCommand` |
| `POST` | `/api/v1/identity/roles/{roleId}/permissions` | `AssignPermissionCommand` |

Public (unversioned): `GET /health`, `GET /ready`.

## Database changes

- **Context:** `IdentityDbContext` (`AIPM.Infrastructure`)
- **Migration:** `20260707160607_InitialIdentity`
- **Tables:** `identity_tenants`, `identity_users`, `identity_roles`, `identity_role_assignments`, `identity_permission_assignments`
- **Dev default:** SQLite (`ConnectionStrings:IdentityDb`)
- **Startup:** `Database.MigrateAsync()` on host boot

## Events added

| Event | Type | Trigger |
|-------|------|---------|
| EVT-001 TenantProvisioned | `TenantProvisionedIntegrationEvent` | `ProvisionTenantCommand` |
| EVT-002 TenantSuspended | `TenantSuspendedIntegrationEvent` | `SuspendTenantCommand` |

Published via `IIdentityEventPublisher` → existing MassTransit message bus.

Domain events (`TenantProvisionedDomainEvent`, `TenantSuspendedDomainEvent`) are raised inside aggregates.

## Tests added

| Category | File | Count |
|----------|------|-------|
| Domain unit | `IdentityDomainTests.cs` | 3 |
| Repository integration | `IdentityRepositoryIntegrationTests.cs` | 2 |
| Migration | `IdentityMigrationTests.cs` | 1 |
| API + authorization | `ApiV1EndpointTests.cs` (identity cases) | 2 |

**Solution total after P2-M1:** 43 tests (4 architecture + 30 shared kernel + 9 host).

## Security

- Fail-closed: BC-10 endpoints return 401 without valid API key.
- API key configured via user secrets (`Security:ApiKey`), not committed.
- No login UI, no OAuth/OIDC (deferred).

## Known limitations

| Limitation | Rationale / follow-up |
|------------|----------------------|
| SQLite for identity in dev | PostgreSQL identity store deferred to infrastructure hardening |
| API key service auth only | Enterprise SSO/IdP integration is post-P2-M1 |
| Tenant lifecycle subset (Active/Suspended) | Full Provisioned→Deprovisioned lifecycle in later BC-10 milestones |
| User lifecycle not modeled (Invited/Removed) | Minimal P2-M1 scope per approved plan |
| Organization/Workspace not in P2-M1 | BC-10 OHS scope for P2-M1 focused on core RBAC primitives |
| `RoleAssigned` published language event not emitted | Deferred; assignment persisted, no separate EVT yet |
| Permission catalog not seeded | Permissions assigned ad hoc per role |
| No ABAC attributes | EIM notes future evolution (CON-004) |

## Quality gate results

| Gate | Result |
|------|--------|
| `dotnet restore` | PASS |
| `dotnet build -c Release` | PASS |
| `dotnet test -c Release` | PASS (43/43) |
| `dotnet format --verify-no-changes` | PASS |
| Architecture tests | PASS (4/4) |
