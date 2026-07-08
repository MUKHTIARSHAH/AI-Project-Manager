# BCM Before DDD

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

Engineers typically jump from Architecture directly to Domain-Driven Design. For AI orchestration platforms, this causes business capabilities (what the system must do for customers) to be entangled with technical aggregates, APIs, and agent implementations.

## Decision

Insert `04-Business-Capability-Model/` before `05-Domain-Model/`. Separate `07-State-Machines/` from domain model and `06-Domain-Events/`. Renumber downstream folders through `18-Roadmap`.

## Consequences

+ Stable business vocabulary independent of DDD refactoring.  
+ Clear mapping: Capability → Bounded Context → Aggregate → Event.  
+ Agent types align to capabilities before technical contracts.  
- One additional approval gate before DDD (intentional).

## References

- [README](../../README.md) — dependency order v1.2.0
- [Project Constitution](../../00-Governance/Project-Constitution.md) — Section 22
- SRS-AIPM-001 (locked), SAD-AIPM-001 (approved)
