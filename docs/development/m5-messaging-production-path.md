# M5 — Messaging Production Path

**Status:** Complete  
**Dependencies:** M2, M3, M4

## Objective

Provide a production-grade platform messaging backbone (RabbitMQ/MassTransit path) with immutable contracts, correlation/causation tracing, idempotent consumers, dead-letter abstraction, and readiness visibility.

## Deliverables

| Deliverable | Location |
|---|---|
| Immutable/versioned message contracts | `AIPM.Infrastructure/Messaging/Contracts/` |
| Correlation + causation IDs in platform integration events | `PlatformStartedEvent`, `PlatformHealthEvent` |
| MassTransit consumers | `AIPM.Infrastructure/Messaging/Consumers/` |
| Idempotency protection for consumers | `Messaging/Idempotency/` |
| Dead-letter abstraction + in-memory implementation | `Messaging/DeadLetter/` |
| Structured publish/consume logs + OTel activity source | `MassTransitMessageBus`, consumers, `MessagingTelemetry` |
| Messaging readiness check | `MessagingPipelineHealthCheck` + `/ready` |

## Acceptance

- `PlatformStartedEvent` and `PlatformHealthEvent` publish/consume via MassTransit.
- Consumer processing is idempotent by message ID.
- Failed consumer processing is routed to dead-letter abstraction.
- Readiness includes messaging pipeline health.
- Messaging remains platform-scoped (no business/domain event coupling).
