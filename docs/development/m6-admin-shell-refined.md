# M6 — Admin Shell (Refined by ADR-TECH-002)

**Status:** Complete  
**Dependencies:** M1–M5

## Objective

Deliver M6 in roadmap order while prioritizing AI platform foundation contracts and providing a minimal admin shell connectivity check.

## Deliverables

| Deliverable | Location |
|---|---|
| ADR for M6 scope refinement | `Engineering-Blueprint/02-Architecture/ADR/ADR-TECH-002-M6-admin-shell-ai-foundation.md` |
| Provider-independent AI abstraction interfaces/contracts | `src/AIPM.Application/AI/` |
| AI provider registration + DI foundation (no external providers) | `src/AIPM.Infrastructure/AI/` |
| Minimal API for provider foundation visibility | `GET /api/v1/ai/providers` |
| Minimal Next.js admin shell connectivity page | `apps/admin-console/app/page.tsx` |

## Acceptance

- M6 remains Admin Shell and roadmap position unchanged.
- AI abstraction layer compiles and is wired through DI.
- No OpenAI/Anthropic/Gemini/Azure/local model integration.
- UI verifies connectivity to `/health`, `/ready`, `/api/v1/platform/deployment`, `/api/v1/agent-types`.
- Existing quality gates remain green.

## Explicitly out of scope

- AI workflow execution
- Business workflows and business aggregates
- Provider SDK integrations
