# Tenant ID Sharding

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

Multi-tenant scale

## Decision

tenant_id as primary shard key; immutable once assigned.

## Consequences

+ Horizontal scale. - Shard migration cost if wrong.

## References

- SRS-AIPM-001 v1.0.0
- SAD-AIPM-001 v1.0.0
- [ADR Index](../../00-Governance/ADR-Index.md)
