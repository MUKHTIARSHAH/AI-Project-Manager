# CQRS Read/Write Split

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

Read performance NFR-001

## Decision

Write model in Core DU; read models in Analytics DU.

## Consequences

+ Read scale. - Eventual consistency.

## References

- SRS-AIPM-001 v1.0.0
- SAD-AIPM-001 v1.0.0
- [ADR Index](../../00-Governance/ADR-Index.md)
