# Task-Scoped Credential Broker

**Status:** Accepted  
**Date:** 2026-07-07  
**Deciders:** Architecture Review Board

## Context

FR-117 halt credential revoke

## Decision

Broker issues task-scoped tokens; revoke within 60s on halt.

## Consequences

+ Least privilege. - Broker as critical path.

## References

- SRS-AIPM-001 v1.0.0
- SAD-AIPM-001 v1.0.0
- [ADR Index](../../00-Governance/ADR-Index.md)
