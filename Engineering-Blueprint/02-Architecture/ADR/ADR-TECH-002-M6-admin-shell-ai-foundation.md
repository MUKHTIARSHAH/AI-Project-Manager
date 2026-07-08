# ADR-TECH-002 — M6 Admin Shell Scope Refinement (AI Foundation First)

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

The implementation roadmap is frozen by governance (ADR-GOV-006), so M6 must remain in sequence as **Admin shell** after M5.  
At the same time, long-term product direction prioritizes the AI orchestration foundation before any substantive presentation features.

Existing architecture already requires a model-agnostic AI layer (ADR-004, TD-15), but M6 wording focuses on UI.

## Decision

Keep M6 in its existing roadmap position and title (**Admin shell**), while refining implementation scope:

1. Primary implementation effort is the backend AI platform integration foundation:
   - provider-independent AI abstractions
   - prompt abstractions
   - completion and streaming contracts
   - tool-calling interfaces
   - memory interfaces
   - provider registration and dependency injection pipeline
2. Implement only a minimal Next.js admin shell to verify connectivity to existing platform APIs.
3. Do not integrate external AI providers in M6.
4. Do not implement business workflows or business aggregates in M6.

## Consequences

+ Governance compliance is preserved (no milestone reorder).
+ M6 still delivers the required admin shell artifact.
+ AI foundation is prepared for later provider integrations without architecture churn.
- M6 includes more backend platform work than originally implied by a UI-only reading.

## Boundaries

- No OpenAI, Anthropic, Gemini, Azure OpenAI, or local model adapters.
- No business/domain feature implementation.
- No changes to milestone ordering.

## References

- ADR-GOV-006 (frozen roadmap)
- ADR-004 (model-agnostic router)
- ADR-TECH-001 / TD-15 (multi-provider abstraction)
- IAD-AIPM-PH1-001 (M6 in approved sequence)
