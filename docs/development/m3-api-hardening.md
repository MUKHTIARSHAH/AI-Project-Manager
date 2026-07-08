# M3 — API Hardening & Configuration

**Status:** Complete  
**Dependencies:** M2

## Objective

Production API error patterns and secure, validated configuration per ADR-SAD-005.

## Acceptance criteria (met)

| Criterion | Evidence |
|-----------|----------|
| Errors return `application/problem+json` | `GlobalExceptionHandler` + `ProblemDetailsTests` |
| Profiles load per ADR-SAD-005 | `deploy/profiles/{saas,dedicated,airgapped}.json` + `DeploymentProfileTests` |
| No secrets in committed config | Empty connection strings; `docs/development/user-secrets.md` |
| Structured config validation | `ValidateDataAnnotations` + `ValidateOnStart` on startup |
| Definition of Done | Build, test, format, architecture tests pass |

## Components

| Component | Location |
|-----------|----------|
| RFC 7807 handler | `AIPM.Host/Http/GlobalExceptionHandler.cs` |
| Domain error types | `AIPM.SharedKernel/Errors/` |
| Deployment profiles | `deploy/profiles/*.json` |
| Config options + validation | `AIPM.Infrastructure/Configuration/` |
| Profile loader | `ConfigurationBuilderExtensions.AddDeploymentProfile` |

## API

| Endpoint | Description |
|----------|-------------|
| `GET /api/v1/platform/deployment` | Active profile + capability flags |
| `GET /api/v1/agents/{capability}` | Resolve agent; 404 Problem Details when missing |

## Switching profiles

Set in `appsettings.json`, environment variable, or launch profile:

```text
Deployment__Profile=dedicated
```

Valid values: `saas`, `dedicated`, `airgapped`.

## Explicitly not included

- Authentication / authorization middleware (later milestone)
- AI provider configuration
- Business aggregates
