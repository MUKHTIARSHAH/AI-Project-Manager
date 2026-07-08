# BuildingBlocks extraction roadmap

## Why

Microservices and multiple hosts (DU-1..5) will share:

- Correlation / tenant context
- Event bus abstractions
- Observability bootstrap
- Configuration profiles
- Messaging envelopes
- Security primitives
- Persistence ports

Centralizing these under `src/BuildingBlocks/` reduces duplication when services split.

## When

| Trigger | Action |
|---------|--------|
| M2 Platform Runtime complete | Evaluate `Messaging` + `EventBus` extraction |
| Second deployable host added | Extract `Observability`, `Configuration` |
| Phase 2 BC implementation | Extract `Persistence` adapters |

## Rules

1. Do not break NetArchTest boundaries during extraction.
2. Each BuildingBlock is a class library with its own tests.
3. No business aggregates inside BuildingBlocks — platform primitives only.

## Mapping (current → future)

| Current project | Future BuildingBlock |
|-----------------|----------------------|
| `AIPM.SharedKernel` | `BuildingBlocks/SharedKernel` |
| `AIPM.Infrastructure` (messaging) | `BuildingBlocks/Messaging`, `BuildingBlocks/EventBus` |
| Host OTel/Serilog setup | `BuildingBlocks/Observability` |
| `appsettings` + profiles | `BuildingBlocks/Configuration` |

**Decision:** Deferred from M1 per architect review — implement when M2 runtime stabilizes.
