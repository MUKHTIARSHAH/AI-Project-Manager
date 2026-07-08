# ADR-TECH-001 — Approved Technology Stack

**Status:** APPROVED (amended 2026-07-07)  
**Date:** 2026-07-07  
**Amendment:** Runtime updated from .NET 8 LTS to **.NET 9 / ASP.NET Core 9** — architecture, DDD, and Clean Architecture unchanged.
**Deciders:** Architecture Review Board  
**Classification:** Technology — LOCKED after approval  
**Supersedes:** [ADR-008](ADR-008-implementation-languages.md) *(implementation language and framework choices only; ADR-008 file remains unchanged)*

---

## Context

AIPM requires a single, authoritative technology stack before implementation proceeds beyond architecture documents. Prior guidance (ADR-008) allowed Go/Java control plane with TypeScript UI and isolated Python analytics. The program standardizes on **.NET** for the control plane to maximize enterprise alignment, hiring pool overlap with target customers, and unified tooling—while preserving all locked blueprint domain and architecture decisions.

IAD-AIPM-PH1-001 (Go-based scaffold plan) is **superseded for language/runtime choices** by this ADR. Structural patterns (Clean Architecture, layers, DUs) remain valid.

Locked constraints preserved: five deployable units (ADR-SAD-001), CQRS (ADR-SAD-008), log-based events (ADR-SAD-004), model-agnostic router (ADR-004), PostgreSQL authoritative store (ADR-SAD-003 alignment), hybrid SaaS (ADR-001).

---

## Decision

**Approve and LOCK** the technology stack defined in [Technology-Architecture.md](../../24-Standards/Technology-Architecture.md) (TAD-AIPM-001 v1.1.0).

### Summary of locked choices

| Layer | Technology |
|-------|------------|
| Control plane backend | **C# / ASP.NET Core 9 (.NET 9)** |
| Public API | REST (OpenAPI) |
| Internal API | gRPC (service-to-service) |
| Auth | JWT + refresh tokens, RBAC |
| Real-time | SignalR |
| Validation | FluentValidation |
| Mapping | **Mapster** |
| Primary database | **PostgreSQL 16+** |
| Cache | **Redis 7+** |
| Vector (AI memory/RAG) | **pgvector** (primary); Qdrant escalation path |
| Object storage | **MinIO** (dev) · **Azure Blob** (Azure prod) · **S3** (AWS prod) |
| Search | **OpenSearch 2.x** |
| Message broker (initial) | **RabbitMQ** |
| Message broker (enterprise scale) | **Apache Kafka** |
| AI providers | Abstraction layer: OpenAI, Anthropic, Gemini, Azure OpenAI, local (Ollama/vLLM) |
| Frontend | **Next.js 14+ (React, TypeScript)** |
| Desktop | **Not required Phase 1** |
| Mobile | **Deferred** beyond Phase 1 |
| Containers | Docker |
| Orchestration | Kubernetes |
| Ingress | NGINX |
| CI/CD | **GitHub Actions** (primary) |
| Cloud (primary) | **Microsoft Azure** |
| Logging | Serilog |
| Metrics | Prometheus |
| Dashboards | Grafana |
| Tracing | OpenTelemetry |
| Unit/integration tests | xUnit, FluentAssertions, Testcontainers |
| E2E | Playwright |

---

## Alternatives Considered

| Area | Alternatives | Outcome |
|------|--------------|---------|
| Backend runtime | Go, Java/Spring | .NET — enterprise fit, unified stack with approved patterns |
| ORM/data | Dapper-only, EF Core | EF Core + Dapper where needed (documented in TAD) |
| Mapper | AutoMapper | Mapster — performance, compile-time safety |
| RDBMS | SQL Server, MySQL, MariaDB | PostgreSQL — OSS, JSON, pgvector, multi-cloud |
| Vector DB | Qdrant, Weaviate, Milvus | pgvector first — operational simplicity (ADR-SAD-003) |
| Search | Elasticsearch | OpenSearch — license/OSI alignment, AWS/Azure managed options |
| Broker (start) | Kafka, Azure Service Bus | RabbitMQ — lower ops for initial scale |
| Broker (scale) | RabbitMQ only | Kafka — ADR-SAD-004 log backbone at Year 5 NFR-022 |
| Frontend | Blazor, Angular | Next.js — UX velocity, TS ecosystem, decoupled from API |
| Desktop | MAUI, Electron, Tauri | None Phase 1 |
| Cloud | AWS-only, GCP-only | Azure primary (ADR-001 hybrid), portable K8s |

---

## Evaluation Matrix (weighted summary)

| Criterion (weight) | .NET + PG + Redis + Next.js | Go + PG stack | Java + PG stack |
|--------------------|----------------------------|---------------|-----------------|
| Enterprise adoption (20%) | 9 | 7 | 9 |
| AI platform suitability (15%) | 8 | 8 | 7 |
| Operational complexity (15%) | 8 | 7 | 6 |
| Multi-cloud portability (15%) | 8 | 9 | 8 |
| Team/hiring (.NET enterprise) (15%) | 9 | 7 | 8 |
| Performance (10%) | 8 | 9 | 8 |
| Licensing cost (10%) | 9 | 9 | 8 |
| **Weighted total** | **8.4** | **7.7** | **7.6** |

Full per-technology matrices: TAD § Technology Decision Matrix.

---

## Consequences

### Positive

+ Single primary backend language simplifies hiring, security review, and SDK maintenance.  
+ PostgreSQL + pgvector reduces persistence sprawl for RAG Phase 1.  
+ OpenSearch + Kafka path aligns with audit replay and analytics (SC-003, NFR-022).  
+ Azure aligns with ADR-001 enterprise hybrid SaaS.  
+ Model abstraction satisfies ADR-004 without vendor lock-in.

### Negative

+ IAD Phase 1 Go scaffold plan must be reworked to .NET before coding.  
+ Polyglot remains (C#, TypeScript, SQL, optional Python ML utilities).  
+ Kafka ops complexity deferred but planned.  
+ pgvector may require Qdrant split at very large embedding scale.

---

## Risks

| Risk | Mitigation |
|------|------------|
| ADR-008 apparent conflict | ADR-TECH-001 explicitly supersedes language choice; ADR-008 file preserved |
| pgvector scale limits | Migration path to Qdrant documented in TAD; ADR-TECH-002 if triggered |
| RabbitMQ throughput ceiling | Kafka adoption trigger metrics in TAD migration strategy |
| Azure concentration | AWS profile parity; K8s portability; ADR-SAD-005 profiles |
| .NET agent SDK vs Python agents | Agent runtimes remain isolated (ADR-007); SDK in any language via gRPC contract |
| Vendor LLM outage | Router fallback per ADR-004; local model profile |

---

## Future Revisions

Changes to **LOCKED** technologies require new ADR (e.g., ADR-TECH-002). Patch version bumps (PostgreSQL 16→17, .NET 9→10) follow TAD Version Policy without ADR unless breaking.

---

## Approval

| Role | Status | Date |
|------|--------|------|
| Architecture Review Board | APPROVED | 2026-07-07 |
| Security | APPROVED | 2026-07-07 |
| Platform Engineering | APPROVED | 2026-07-07 |

---

## References

- TAD-AIPM-001: [Technology-Architecture.md](../../24-Standards/Technology-Architecture.md)
- SRS-AIPM-001, SAD-AIPM-001, PC-AIPM-001, DM-AIPM-001
- ADR-001, ADR-004, ADR-SAD-001–010, ADR-GOV-007

---

**ADR-TECH-001 STATUS: APPROVED**
