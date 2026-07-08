# Changelog

All notable implementation changes to the AIPM control plane repository.

Format based on [Keep a Changelog](https://keepachangelog.com/). Blueprint document versions are tracked separately in `Engineering-Blueprint/00-Governance/Change-Log.md`.

## [Unreleased]

### Phase 2

- P2-M2 and beyond (not started).

## [0.2.0] — 2026-07-08

### Added — P2-M1 BC-10 Identity & Access Core

- Domain aggregates and value objects: `Tenant`, `User`, `Role`, `Permission`, `RoleAssignment`, `PermissionAssignment`, `IdentityPolicy`.
- Application commands/queries (MediatR): provision/suspend tenant; create user/role; assign role/permission; list tenants/users/roles.
- EF Core persistence (`IdentityDbContext`) with migration `InitialIdentity` (SQLite dev default).
- Repository implementations: `TenantRepository`, `UserRepository`, `RoleRepository`.
- BC-10 HTTP API under `/api/v1/identity/*` with fail-closed API key authorization.
- Integration events: `TenantProvisionedIntegrationEvent` (EVT-001), `TenantSuspendedIntegrationEvent` (EVT-002).
- Tests: domain unit, repository integration, migration, API integration, authorization (43 total solution tests).

### Changed

- Host startup applies EF migrations for identity database.
- `appsettings.json` no longer contains API key values (use user secrets).

## [0.1.0] — 2026-07-07

### Added — Phase 1 Platform (M1–M6)

- Clean Architecture solution structure (.NET 9).
- Platform runtime: execution context, plugin loader, agent registry, command bus, event dispatcher, workflow runtime, background workers, resilience.
- API hardening: RFC 7807 Problem Details, deployment profiles, structured configuration validation.
- Agent SDK contracts and agent type catalog endpoint.
- Messaging production path: MassTransit, idempotency, dead-letter sink, messaging health.
- AI provider abstraction foundation (no external providers).
- Minimal Next.js admin shell connectivity checks.
- Docker Compose stack, CI pipeline, architecture tests, milestone quality gates.
