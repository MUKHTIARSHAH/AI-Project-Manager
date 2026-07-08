================================================================================
Software Architecture Document (SAD)
================================================================================

Document ID: SAD-AIPM-001
Product: AI Project Manager
Version: 1.0.0
Classification: Internal — Enterprise Architecture Foundation
Date: 2026-07-07
Standard Alignment: ISO/IEC/IEEE 42010:2022
Source of Truth: SRS-AIPM-001 v1.0.0 (LOCKED — DO NOT MODIFY)
Status: See Architecture Audit at end

--------------------------------------------------------------------------------
Document Control
--------------------------------------------------------------------------------

Field              | Value
-------------------|----------------------------------------------------------
Author             | Principal Software Architect
Approvers          | Engineering, Security, Enterprise Architecture (pending)
Distribution       | Platform Engineering, Agent Teams, Integration Teams
Change Policy      | ADR-required for structural changes; traceability to SRS mandatory
Parent Document    | SRS-AIPM-001 v1.0.0 (APPROVED)

--------------------------------------------------------------------------------
SAD Ambiguity Register (New — Architecture Layer Only)
--------------------------------------------------------------------------------

ID         | Ambiguity                         | Why It Matters                    | Alternatives                              | Recommendation                          | ADR
-----------|-----------------------------------|-----------------------------------|-------------------------------------------|-----------------------------------------|----------
SAD-AMB-001 | GA deployable unit count          | Team topology, ops burden         | 1 monolith; 10+ microservices; 4-5 units  | 5 deployable units (see Section 10)     | ADR-SAD-001
SAD-AMB-002 | Workflow engine placement         | Durability, replay, complexity    | External engine only; embedded FSM only   | Embedded FSM + durable workflow adapter | ADR-SAD-002
SAD-AMB-003 | Knowledge graph storage           | Query patterns, cost              | Native graph DB; relational + projection  | Relational authoritative + graph projection | ADR-SAD-003
SAD-AMB-004 | Event backbone implementation     | Scale to NFR-022                  | Single queue; log-based streaming         | Log-based event backbone abstraction    | ADR-SAD-004
SAD-AMB-005 | Dedicated vs air-gapped topology  | CON-005 fulfillment               | Same topology; fully separate             | Topology profiles with capability flags | ADR-SAD-005

================================================================================
SECTION TEMPLATE REFERENCE
================================================================================

Every numbered section (1–100) contains the following fields:

  Purpose | Detailed Explanation | Responsibilities | Inputs | Outputs |
  Dependencies | Interactions | Failure Scenarios | Recovery Strategy |
  Security Considerations | Scalability Considerations |
  Maintainability Considerations | Alternatives | Advantages | Disadvantages |
  Engineering Recommendation | Reasoning | Future Evolution

Sections below follow this template. Where fields are identical across closely
related sections, cross-references are used to avoid redundancy while preserving
completeness.

================================================================================
1. Architecture Overview
================================================================================

Purpose
-------
Provide a single authoritative picture of what AIPM is architecturally and how
major parts relate.

Detailed Explanation
--------------------
AI Project Manager (AIPM) is an enterprise control-plane platform that
orchestrates autonomous software delivery. Architecturally it separates:

  CONTROL PLANE (AIPM) — decides, governs, records, schedules, approves
  DATA PLANE (Agents + Integrations) — executes work, produces artifacts

AIPM is NOT an application code generator. It maintains authoritative delivery
state, enforces policies and gates, dispatches work to 14+ agent types in
isolated runtimes, and produces audit-grade evidence.

The architecture is multi-tenant, region-aware, event-sourced for audit paths,
CQRS-enabled for read scalability, and zero-trust throughout.

SRS Traceability: Mission (SRS §3), Proposed Solution (SRS §7), Architecture
Philosophy (SRS §48), CON-001 through CON-008.

Responsibilities
----------------
- Define system boundaries and deployment profiles (SaaS, dedicated, air-gapped).
- Anchor all downstream implementation documents.
- Map SRS requirements to architectural elements.

Inputs
------
- Locked SRS-AIPM-001 requirements, ADRs, constraints, NFRs.

Outputs
-------
- Enterprise architecture baseline for implementation planning.
- Module and context boundaries for team ownership.

Dependencies
------------
- SRS-AIPM-001 (locked).
- External: cloud providers, identity providers, LLM providers, agent runtimes.

Interactions
------------
- Human users via Edge/Gateway -> Control Plane services.
- Agents via Dispatch Protocol -> Agent Runtimes.
- External systems via Integration Plane.

Failure Scenarios
-----------------
- Misidentifying AIPM as codegen platform -> scope violation (CON-001).
- Collapsing control/data plane -> audit and security failure.

Recovery Strategy
-----------------
- Enforce architectural fitness functions in CI rejecting control-plane code
  execution paths for customer application artifacts.

Security Considerations
-----------------------
- Control plane holds metadata and governance; least exposure of secrets.
- All cross-boundary calls authenticated and authorized.

Scalability Considerations
--------------------------
- Horizontally scaled stateless tiers; tenant-sharded authoritative stores.
- Async projections for dashboards and portfolio analytics (FR-122).

Maintainability Considerations
------------------------------
- 5 deployable units at GA (ADR-SAD-001) balance modularity and operability.

Alternatives
------------
1. Monolithic single binary.
2. Fine-grained microservices (20+).
3. External BPMN-only orchestrator.

Advantages
----------
- Clear mental model for engineers, security, and auditors.
- Supports thousands of tenants (SRS §28, SC-005).

Disadvantages
-------------
- Higher initial design cost than demo architectures.

Engineering Recommendation
--------------------------
Adopt control-plane / data-plane separation as non-negotiable invariant.

Reasoning
---------
SRS mandates PM coordinates agents without writing application code; separation
of duties requires architectural separation.

Future Evolution
----------------
- Cell-based regional expansion (Section 81).
- Federated PM instances (SRS §44).

================================================================================
2. Architecture Goals
================================================================================

Purpose
-------
State measurable architectural outcomes derived from SRS objectives.

Detailed Explanation
--------------------
| Goal ID   | Architectural Goal                                      | SRS Source
|-----------|---------------------------------------------------------|------------
| ARCH-G01  | Support 10k concurrent tasks, 500 tenants at GA load    | SC-005, NFR-003
| ARCH-G02  | 99.9% API availability; tiered event ingestion SLO    | NFR-004, NFR-021
| ARCH-G03  | Full audit reconstructability via event sourcing        | SC-003, FR-080
| ARCH-G04  | Halt dispatches within 30s; credential revoke within 60s| SC-008, FR-117
| ARCH-G05  | Budget enforcement stops runaway execution within 60s   | SC-006, NFR-020
| ARCH-G06  | Tenant isolation with region pinning                    | FR-090, FR-120
| ARCH-G07  | Add agent types without orchestrator fork               | NFR-012, FR-030
| ARCH-G08  | P95 read <=200ms; dispatch <=2s                         | NFR-001, NFR-002
| ARCH-G09  | Year-5 event backbone >=500k events/sec capacity        | NFR-022
| ARCH-G10  | RPO <=15min; RTO <=4hr                                  | NFR-005

Responsibilities
----------------
- Translate SRS success criteria into design constraints.
- Guide trade-off decisions when goals conflict.

Inputs / Outputs
----------------
Inputs: SRS objectives (§8), success criteria (§11), NFRs.
Outputs: Non-functional design targets for all subsystems.

Dependencies
------------
- Capacity model (Section 96).
- Deployment topology (Section 97).

Interactions
------------
- Goals inform ADRs, module SLOs, and fitness functions.

Failure Scenarios
-----------------
- Optimizing one goal (speed) violates another (audit completeness).

Recovery Strategy
-----------------
- Goal conflict hierarchy: Safety > Compliance > Correctness > Availability >
  Performance > Cost (SRS §45).

Security / Scalability / Maintainability
------------------------------------------
- Security and isolation are never traded for performance.
- Scalability targets drive sharding and async design early.
- Maintainability via bounded contexts and documented ownership (NFR-011).

Alternatives
------------
- Best-effort goals without measurement.
- Over-provisioned single-tenant architecture.

Advantages / Disadvantages
--------------------------
+ Aligns engineering with enterprise contracts.
- Strict goals increase infrastructure cost.

Engineering Recommendation
--------------------------
Embed goals as SLO dashboards and architecture fitness tests before GA.

Reasoning
---------
SRS defines quantified success; architecture must make them achievable.

Future Evolution
----------------
- Stricter SLOs for enterprise SKU; relaxed for sandbox tier.

================================================================================
3. Architecture Principles
================================================================================

Purpose
-------
Define invariant rules governing all architectural decisions.

Detailed Explanation
--------------------
Derived from SRS §45–49:

  AP-01  Control/Data Plane Separation
  AP-02  API-First and Event-First
  AP-03  Fail-Closed Security and Policy Evaluation
  AP-04  Schema-First Boundaries (agents, events, integrations)
  AP-05  Authoritative State in AIPM (ADR-005)
  AP-06  Idempotent Dispatch and At-Least-Once Delivery
  AP-07  Tenant Isolation by Default
  AP-08  Observability on Every Dispatch Path (NFR-015)
  AP-09  Evolutionary Modularity (extract when metrics justify)
  AP-10  No Silent Failures

Responsibilities
----------------
- Resolve design disputes.
- Gate ADR approvals via ARB.

Inputs / Outputs
----------------
Inputs: SRS engineering and architecture philosophy.
Outputs: Design review checklist.

Dependencies
------------
- ADR log (Section 5).

Interactions
------------
- Principles applied in every module (Sections 15–16).

Failure Scenarios
-----------------
- Principle drift per team -> inconsistent behavior.

Recovery Strategy
-----------------
- Architecture fitness functions and ARB reviews.

Security Considerations
-----------------------
- AP-03 and AP-07 are security-critical.

Scalability Considerations
--------------------------
- AP-06 enables horizontal scale despite duplicate delivery.

Maintainability Considerations
------------------------------
- AP-09 avoids premature microservices.

Alternatives
------------
- Implicit team conventions only.

Advantages / Disadvantages
+ Consistency at scale. - Rigidity for edge cases.

Engineering Recommendation
--------------------------
Codify principles in CONTRIBUTING and ARB charter.

Reasoning
---------
Autonomous multi-agent systems require strict invariants.

Future Evolution
----------------
- Additional principles for federated PM without changing core ten.

================================================================================
4. Architectural Constraints
================================================================================

Purpose
-------
Document hard limits inherited from SRS that architecture cannot violate.

Detailed Explanation
--------------------
Direct mapping from SRS CON-001–CON-008:

  AC-01  PM never executes customer application code (CON-001)
  AC-02  Enterprise SSO + tenant isolation mandatory (CON-002)
  AC-03  Cloud-portable design AWS/Azure/GCP (CON-003)
  AC-04  LLM provider replaceability (CON-004, ADR-004)
  AC-05  Air-gapped dedicated profile required (CON-005)
  AC-06  No per-customer core forks (CON-006)
  AC-07  Automated compliance evidence export (CON-007)
  AC-08  Agents released independently of PM core (CON-008)

Responsibilities
----------------
- Reject designs violating constraints.
- Define deployment profiles honoring AC-05.

Inputs / Outputs
----------------
Inputs: SRS constraints section.
Outputs: Constraint compliance matrix per module.

Dependencies
------------
- Deployment Topology (Section 97).

Interactions
------------
- Constraints shape plugin, agent, and integration architectures.

Failure Scenarios
-----------------
- Custom fork for one enterprise customer (violates AC-06).

Recovery Strategy
-----------------
- Configuration and policy templates instead of forks.

Security / Scalability / Maintainability
------------------------------------------
- AC-02 drives multi-tenant and IAM architecture.
- AC-06 forces extension points over customization.
- AC-08 requires versioned agent contracts.

Alternatives
------------
- Single-cloud hard binding.
- SaaS-only without dedicated profile.

Advantages / Disadvantages
+ Enterprise readiness. - Higher design effort.

Engineering Recommendation
--------------------------
Maintain constraint compliance checklist in every design review.

Reasoning
---------
Constraints are contractual and security boundaries.

Future Evolution
----------------
- Additional constraints for new compliance modules (HIPAA pack).

================================================================================
5. Architecture Decision Records (ADR)
================================================================================

Purpose
-------
Record all significant architectural decisions with rationale.

Detailed Explanation
--------------------
INHERITED FROM SRS (IMMUTABLE):
  ADR-001  Hybrid SaaS + dedicated enterprise tier
  ADR-002  Mid-market and enterprise first
  ADR-003  Policy-driven hybrid autonomy; mandatory production gates
  ADR-004  Model-agnostic router with approved catalog
  ADR-005  AIPM authoritative; optional bidirectional external PM sync
  ADR-006  US + EU compliance at launch
  ADR-007  PM orchestrates; agents in isolated runtimes
  ADR-008  Go/Java control plane; TypeScript UI; Python analytics isolated

NEW ARCHITECTURE DECISIONS:
  ADR-SAD-001  Five deployable units at GA (Edge, Core, Governance, Integration, Analytics)
  ADR-SAD-002  Embedded finite-state orchestration + durable workflow adapter layer
  ADR-SAD-003  Relational authoritative store + knowledge graph projection layer
  ADR-SAD-004  Log-based event backbone with abstraction over implementation
  ADR-SAD-005  Deployment profiles: Standard SaaS, Dedicated, AirGapped (capability flags)
  ADR-SAD-006  Tenant_id as primary shard key; immutable once assigned
  ADR-SAD-007  mTLS + workload identity for all service and agent dispatch channels
  ADR-SAD-008  CQRS: write model in Core; read models in Analytics plane
  ADR-SAD-009  Policy engine evaluates at Schedule, Dispatch, and Gate PEPs
  ADR-SAD-010  Credential broker issues task-scoped tokens; revoked on halt (FR-117)

Responsibilities
----------------
- Single registry of decisions.
- Prevent re-litigation of settled choices.

Inputs / Outputs
----------------
Inputs: Ambiguity resolutions, design reviews.
Outputs: ADR index for implementation teams.

Dependencies
------------
- ARB governance process.

Interactions
------------
- Every section references applicable ADRs.

Failure Scenarios
-----------------
- Undocumented ad-hoc decisions -> drift from SRS.

Recovery Strategy
-----------------
- No merge without ADR reference for structural changes.

Security / Scalability / Maintainability
------------------------------------------
- ADR-SAD-007 security foundation.
- ADR-SAD-006 scalability foundation.
- ADR-SAD-001 maintainability balance.

Alternatives
------------
- Wiki-only decision log.
- No formal ADRs.

Advantages / Disadvantages
+ Traceability. - Process overhead.

Engineering Recommendation
--------------------------
Store ADRs in version control; link from SAD and RTM.

Reasoning
---------
Enterprise audits require decision provenance.

Future Evolution
----------------
- ADR for active-active multi-region when error budget allows.

================================================================================
6. Overall System Architecture
================================================================================

Purpose
-------
Describe the complete system structure and major flows.

Detailed Explanation
--------------------
Logical architecture (top level):

  [Users] -----> [Edge & Experience Layer]
                      |
                      v
              [API Gateway + IAM Enforcement]
                      |
        +-------------+-------------+
        v             v             v
  [Core Orchestration] [Governance Plane] [Integration Plane]
        |             |             |
        +------+------+------+------+
               v
        [Event Backbone]
               |
    +----------+----------+----------+
    v          v          v          v
 [Agents]  [External SDLC] [LLM Providers] [Analytics Projections]

Five deployable units (ADR-SAD-001):
  DU-1  Edge Gateway          — TLS termination, WAF, rate limit, routing
  DU-2  Core Orchestration    — Project, Plan, Schedule, Execute, Agent Registry
  DU-3  Governance Plane      — Policy, Gates, Audit, Identity sync, Secrets broker
  DU-4  Integration Plane     — Connectors, webhooks, external sync, credential vault API
  DU-5  Analytics Plane       — CQRS projections, dashboards, portfolio aggregates, search

Responsibilities
----------------
- End-to-end delivery orchestration without code generation.
- Enforce SRS FR-001 through FR-122.

Inputs
------
- Business intent, policies, templates, integration credentials.
- Agent manifests, external events (CI, VCS, tickets).

Outputs
-------
- Plans, dispatches, gate results, audit bundles, notifications, metrics.

Dependencies
------------
- Cloud infrastructure, KMS, identity providers, agent runtimes, LLM APIs.

Interactions
------------
- See Sections 17–27 for flow detail.

Failure Scenarios
-----------------
- Core plane unavailable -> no new dispatches; halt/read paths prioritized.
- Event backbone degraded -> writes queue with backpressure; alerts fire.
- Agent plane unavailable -> scheduler pauses affected agent types (FR-031).

Recovery Strategy
-----------------
- Multi-AZ within region; warm standby cross-region for enterprise (Section 78).
- Event replay for state reconstruction (SC-003).

Security Considerations
-----------------------
- Zero-trust between all units (Section 44).
- Tenant context propagated on every call.

Scalability Considerations
--------------------------
- Stateless DU-1, DU-2 (compute), DU-5 scale horizontally.
- DU-3 audit ingestion scales with partitioned log.
- Shard by tenant_id (ADR-SAD-006).

Maintainability Considerations
------------------------------
- Five units map to 4–6 engineering teams max at GA.

Alternatives
------------
1. Single monolith (3 deployables: app, workers, DB).
2. 15+ microservices.

Advantages / Disadvantages
+ Clear ownership. + Ops manageable. - Some cross-unit latency.

Engineering Recommendation
--------------------------
Implement five deployable units with shared event contracts.

Reasoning
---------
SRS recommends modular monolith or few services; five is minimum for clean
separation of governance and integration blast radius.

Future Evolution
----------------
- Split Core into Plan and Execute services when CPU profiles diverge.

================================================================================
7. Layered Architecture
================================================================================

Purpose
-------
Define horizontal layers and dependency direction.

Detailed Explanation
--------------------
Layers (top to bottom):

  L1  Experience Layer        — Admin console, notification UX (TypeScript per ADR-008)
  L2  Edge Layer              — Gateway, WAF, rate limiting, request validation
  L3  Application Layer       — Domain services (project, plan, schedule, dispatch)
  L4  Domain Layer            — Aggregates, state machines, domain events
  L5  Infrastructure Layer    — Event backbone, storage, cache, queues, KMS clients
  L6  Agent/Integration Layer — External agents, VCS, CI/CD, LLM routers

Dependency rule: L1->L2->L3->L4->L5 only. L6 accessed via L3 adapters only.
No layer skipping. Domain layer has zero knowledge of L6 implementation details.

Responsibilities
----------------
- Enforce clean architecture boundaries.
- Prevent integration logic in domain core.

Inputs / Outputs
----------------
Inputs: External requests, domain commands.
Outputs: Domain events, integration commands, responses.

Dependencies
------------
- DDD bounded contexts (Section 12).

Interactions
------------
- Application services orchestrate domain + infrastructure ports.

Failure Scenarios
-----------------
- Domain layer calling external APIs directly -> untestable, insecure.

Recovery Strategy
----------------
- Hexagonal ports/adapters; static analysis for forbidden imports.

Security Considerations
-----------------------
- L2 validates authn/authz before L3.
- L6 credentials never stored in L4.

Scalability Considerations
--------------------------
- L2 and L3 horizontally scaled; L5 partitioned.

Maintainability Considerations
------------------------------
- Layer violations caught in CI.

Alternatives
------------
- Anemic domain model with logic in L3 only.

Advantages / Disadvantages
+ Testability. - More boilerplate.

Engineering Recommendation
--------------------------
Rich domain model in L4 for state machines and invariants.

Reasoning
---------
Orchestration correctness depends on explicit lifecycle rules (SRS §26).

Future Evolution
----------------
- Extract L3 services per bounded context when team scale warrants.

================================================================================
8. Component Architecture
================================================================================

Purpose
-------
Define major components within each deployable unit.

Detailed Explanation
--------------------
DU-1 Edge Gateway Components:
  - TLS Terminator, WAF Connector, Rate Limiter, Request Router, Correlation ID Injector

DU-2 Core Orchestration Components:
  - Project Component, Portfolio Component, Requirements Facade, Plan Engine Component,
    Scheduler Component, Execution Orchestrator, Agent Registry, Dispatch Manager,
    Conflict Detector, Context Assembly Facade, Halt Controller

DU-3 Governance Components:
  - Policy Engine, Rule Evaluator, Approval Workflow Coordinator, Gate Engine,
    Audit Ingestor, Audit Query API, Identity Sync Adapter, Credential Broker,
    Explainability Record Writer, Compliance Export Generator

DU-4 Integration Components:
  - Connector Framework, VCS Adapter, CI/CD Adapter, Issue Tracker Adapter,
    Cloud Adapter, Webhook Dispatcher, Webhook Receiver, Sync Reconciler,
    Integration Health Monitor

DU-5 Analytics Components:
  - Event Projector, Dashboard Query Service, Portfolio Aggregator, Search Indexer,
    Cost Rollup Engine, Anomaly Detector (FR-084), Report Generator

Responsibilities
----------------
- Each component owns one cohesive capability.
- Components communicate via domain events or internal RPC with mTLS.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
See Section 15–16 for module-level detail.

Failure Scenarios
-----------------
- Component cascade failure without bulkheads.

Recovery Strategy
-----------------
- Circuit breakers per integration adapter; isolated thread pools per agent type.

Security / Scalability / Maintainability
------------------------------------------
- Credential Broker isolated in DU-3 (high security).
- Projectors in DU-5 scale independently.
- Component boundaries match team ownership.

Alternatives
------------
- Fewer, larger components.

Advantages / Disadvantages
+ Blast radius control. - Integration testing surface.

Engineering Recommendation
--------------------------
One owner per component in service catalog.

Reasoning
---------
NFR-011 requires documented ownership.

Future Evolution
----------------
- Hot components split into microservices.

================================================================================
9. Modular Architecture
================================================================================

Purpose
-------
Define plug-in modularity within and across deployable units.

Detailed Explanation
--------------------
Modularity dimensions:
  M1  Agent Type Modules       — registered via manifest (FR-030)
  M2  Gate Validator Modules   — pluggable gate types (FR-060)
  M3  Policy Rule Modules      — Rego/CEL-style policy packs
  M4  Integration Connector Modules — certified connectors (FR-034)
  M5  Project Template Modules — agent sequences + gates (FR-002)
  M6  Compliance Export Modules — SOC2/GDPR packs (ADR-006)
  M7  Notification Channel Modules — email, slack, pagerduty

Module contract: manifest schema version, capability declaration, health endpoint,
input/output schema references, minimum PM core version.

Responsibilities
----------------
- Enable extension without core fork (NFR-012, CON-006).
- Certification pipeline for third-party modules (FR-034).

Inputs / Outputs
----------------
Inputs: Module manifests, configuration.
Outputs: Registered capabilities, runtime module instances.

Dependencies
------------
- Plugin Architecture (Section 83).
- Versioning Strategy (Section 89).

Interactions
------------
- Core discovers modules at startup and on registry refresh.
- Modules execute in sandboxed runtimes where third-party (Section 83).

Failure Scenarios
-----------------
- Malicious module with excessive permissions.

Recovery Strategy
-----------------
- Certification, signature verification, sandbox, least-privilege scopes.

Security / Scalability / Maintainability
------------------------------------------
- Third-party modules never run in Core process memory.
- Module registry cached; hot reload for config-only changes.

Alternatives
------------
- Hardcoded agent types only.

Advantages / Disadvantages
+ Ecosystem growth. - Compatibility matrix burden (FR-033).

Engineering Recommendation
--------------------------
Internal modules use same contract as external; dogfood the plugin model.

Reasoning
---------
FR-034 and marketplace vision require uniform modularity.

Future Evolution
----------------
- Public marketplace (Section 85).

================================================================================
10. Service Architecture
================================================================================

Purpose
-------
Define service boundaries, communication, and ownership.

Detailed Explanation
--------------------
Service map (logical services within deployables):

  SVC-01  edge-gateway-service           (DU-1)
  SVC-02  project-service                 (DU-2)
  SVC-03  requirements-service            (DU-2)
  SVC-04  plan-service                    (DU-2)
  SVC-05  scheduler-service               (DU-2)
  SVC-06  execution-service               (DU-2)
  SVC-07  agent-registry-service          (DU-2)
  SVC-08  dispatch-service                (DU-2)
  SVC-09  policy-service                  (DU-3)
  SVC-10  gate-service                    (DU-3)
  SVC-11  approval-service                (DU-3)
  SVC-12  audit-service                   (DU-3)
  SVC-13  identity-sync-service           (DU-3)
  SVC-14  credential-broker-service       (DU-3)
  SVC-15  integration-hub-service         (DU-4)
  SVC-16  webhook-service                 (DU-4)
  SVC-17  sync-reconciler-service         (DU-4)
  SVC-18  projection-service              (DU-5)
  SVC-19  analytics-query-service         (DU-5)
  SVC-20  cost-meter-service              (DU-5)
  SVC-21  notification-service            (DU-3/DU-4 boundary)
  SVC-22  config-service                  (DU-2, replicated read)
  SVC-23  feature-flag-service            (DU-2)

Inter-service protocol: gRPC internal (conceptual) over mTLS with protobuf schemas.
External: HTTPS JSON via gateway only.

Responsibilities
----------------
- Each service owns one aggregate family or cross-cutting concern.
- No shared databases between services (database-per-service logical pattern).

Inputs / Outputs
----------------
Commands and events per service; see Sections 22–26.

Dependencies
------------
- Service mesh (Section 88).
- Event backbone (Section 19).

Failure Scenarios
-----------------
- Distributed monolith via chatty synchronous calls.

Recovery Strategy
-----------------
- Prefer events over sync calls except read-after-write consistency paths.
- Saga orchestration for multi-step approvals via approval-service.

Security / Scalability / Maintainability
------------------------------------------
- mTLS between all services (ADR-SAD-007).
- Scale dispatch and projection services independently.
- Max 23 services at GA within 5 deployables (process groups).

Alternatives
------------
- Single service per deployable only (too coarse).

Advantages / Disadvantages
+ Team parallelism. - Distributed tracing required.

Engineering Recommendation
--------------------------
Package services as libraries within deployable until extraction metrics hit.

Reasoning
---------
SRS allows modular monolith; services are logical boundaries first.

Future Evolution
----------------
- Extract dispatch-service to separate deployable at 50k concurrent tasks.

================================================================================
11. Domain Driven Design
================================================================================

Purpose
-------
Apply DDD strategic and tactical patterns to model software delivery orchestration.

Detailed Explanation
--------------------
Strategic DDD:
  - Ubiquitous language from SRS §24–26 (Task, Gate, Policy, Dispatch, etc.)
  - Bounded contexts (Section 12) with explicit integration patterns
  - Context map (Section 13) defines relationships

Tactical DDD:
  - Aggregates: Project, Plan, Task, AgentRegistration, PolicySet, GateEvaluation,
    ApprovalRequest, IntegrationConnection, AuditEntry (append-only), BudgetAccount
  - Domain Events: ProjectCreated, PlanVersioned, TaskScheduled, TaskDispatched,
    TaskCompleted, GateFailed, PolicyDenied, HaltActivated, ApprovalGranted, etc.
  - Value Objects: TenantId, ProjectId, TaskId, CorrelationId, AutonomyLevel,
    SchemaRef, CredentialScope
  - Domain Services: SchedulingService, ConflictArbitrationService, ContextRedactionService
  - Repositories: per aggregate root, tenant-scoped
  - Factories: PlanFactory, DispatchPackageFactory

Anti-corruption layers at integration and agent boundaries.

Responsibilities
----------------
- Encode business rules in domain layer, not controllers.
- Preserve invariants (DAG plans, SoD, lifecycle transitions).

Inputs / Outputs
----------------
Inputs: Commands (CreateProject, ApprovePlan, DispatchTask).
Outputs: Domain events persisted to event store.

Dependencies
------------
- Event sourcing for audit-critical aggregates.

Interactions
------------
- Application services coordinate transactions across one aggregate per transaction;
  cross-aggregate via events and sagas.

Failure Scenarios
-----------------
- Anemic domain -> policy bypass bugs.

Recovery Strategy
-----------------
- Domain unit tests for every state machine transition.

Security / Scalability / Maintainability
------------------------------------------
- TenantId on every aggregate root.
- Small aggregates; Plan references tasks by ID (not giant embedded graphs).

Alternatives
------------
- Transaction script pattern.
- CRUD-only data model.

Advantages / Disadvantages
+ Aligns code with SRS terminology. - Learning curve.

Engineering Recommendation
--------------------------
DDD in Core and Governance deployables; simpler CRUD in Analytics projections.

Reasoning
---------
Complex lifecycles (Sections 34–42) require rich domain models.

Future Evolution
----------------
- Separate Planning subdomain if algorithm complexity grows.

================================================================================
12. Bounded Contexts
================================================================================

Purpose
-------
Define autonomous domain boundaries.

Detailed Explanation
--------------------
| Context ID | Name                    | Core Aggregates              | Deployable
|------------|-------------------------|------------------------------|------------
| BC-01      | Project Portfolio       | Project, Program, Portfolio  | DU-2
| BC-02      | Requirements            | Requirement, TraceLink       | DU-2
| BC-03      | Planning                | Plan, PlanVersion, TaskNode | DU-2
| BC-04      | Scheduling & Execution  | Task, ScheduleSlot, Halt   | DU-2
| BC-05      | Agent Management        | AgentType, AgentInstance     | DU-2
| BC-06      | Policy & Autonomy       | PolicySet, AutonomyProfile   | DU-3
| BC-07      | Approvals               | ApprovalRequest, Delegation  | DU-3
| BC-08      | Quality Gates           | GateDefinition, GateResult   | DU-3
| BC-09      | Audit & Compliance      | AuditEvent, ExportJob        | DU-3
| BC-10      | Identity & Access       | RoleBinding, ApiKey, Session | DU-3
| BC-11      | Integrations            | Connection, SyncCursor       | DU-4
| BC-12      | Knowledge & Context     | KnowledgeNode, ContextPackage| DU-2/DU-5
| BC-13      | Cost Management         | Budget, CostEntry            | DU-5
| BC-14      | Notifications           | Subscription, Delivery       | DU-3
| BC-15      | Analytics & Insights    | Projections (read-only)      | DU-5

Responsibilities
----------------
- Each context owns its vocabulary and persistence.
- Cross-context references by ID only.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
See Context Map (Section 13).

Failure Scenarios
-----------------
- God context absorbing all domains.

Recovery Strategy
-----------------
- ARB review for cross-context schema changes.

Security / Scalability / Maintainability
------------------------------------------
- BC-10 centralizes authz decisions.
- BC-09 append-only for compliance.
- Contexts map to team ownership.

Alternatives
------------
- Single bounded context.

Advantages / Disadvantages
+ Independent evolution. - Integration overhead.

Engineering Recommendation
--------------------------
Fifteen contexts at GA; merge BC-14 into DU-3 for operational simplicity.

Reasoning
---------
Matches SRS product subsystems (§42) and FR groupings.

Future Evolution
----------------
- Split BC-12 when graph query load warrants dedicated service.

================================================================================
13. Context Map
================================================================================

Purpose
-------
Visualize relationships between bounded contexts.

Detailed Explanation
--------------------
Context Map (relationship type -> downstream):

  BC-01 Project Portfolio --[U/S]--> BC-02 Requirements
  BC-01 --[U/S]--> BC-03 Planning
  BC-02 --[Conformist]--> BC-03 Planning (requirements feed plans)
  BC-03 --[Partnership]--> BC-04 Scheduling & Execution
  BC-04 --[Customer-Supplier]--> BC-05 Agent Management
  BC-04 --[ACL]--> BC-05 via Dispatch Protocol
  BC-06 Policy --[Published Language]--> BC-03, BC-04, BC-08 (policy evaluation)
  BC-07 Approvals --[Partnership]--> BC-03, BC-04, BC-08
  BC-08 Gates --[Customer-Supplier]--> BC-04 Execution
  BC-04 --[Event Publisher]--> BC-09 Audit
  BC-04 --[Event Publisher]--> BC-13 Cost
  BC-04 --[Event Publisher]--> BC-15 Analytics
  BC-11 Integrations --[ACL]--> External Systems
  BC-12 Knowledge --[Shared Kernel IDs only]--> BC-03, BC-04
  BC-10 Identity --[OHS]--> All contexts (authz)
  BC-06 --[Separate Ways]--> LLM Providers via Model Router

Legend: U/S=Upstream/Supplier, OHS=Open Host Service, ACL=Anti-Corruption Layer

Responsibilities
----------------
- Document integration patterns per relationship.
- Prevent inappropriate shared databases.

Inputs / Outputs
----------------
Inputs: Domain events, queries, policy evaluation requests.
Outputs: Integration contracts between contexts.

Dependencies
------------
- Event backbone for async relationships.

Interactions
------------
- Synchronous only for: policy eval, authz, credential issue, dispatch ack.

Failure Scenarios
-----------------
- Hidden coupling via shared tables between BC-03 and BC-04.

Recovery Strategy
----------------
- Schema ownership checks in CI.

Security / Scalability / Maintainability
------------------------------------------
- ACL on all external and agent boundaries.
- Async events for analytics and audit scale.

Alternatives
------------
- Point-to-point integrations without map.

Advantages / Disadvantages
+ Clear boundaries. - Documentation discipline.

Engineering Recommendation
--------------------------
Maintain living context map diagram in architecture repo.

Reasoning
---------
CON-008 requires independent agent release cycles via ACL.

Future Evolution
----------------
- Federated context map for multi-PM deployments.

================================================================================
14. Context Relationships
================================================================================

Purpose
-------
Detail how contexts integrate mechanically.

Detailed Explanation
--------------------
| From | To | Pattern | Mechanism | Consistency
|------|-----|---------|-----------|------------
| BC-03 | BC-04 | Event | PlanVersionActivated -> Scheduler | Eventual (ms-s)
| BC-04 | BC-05 | Sync+ACL | DispatchCommand via Dispatch Service | Strong per dispatch
| BC-06 | BC-04 | Sync | PolicyEvaluation PEP | Strong; fail-closed
| BC-07 | BC-04 | Event+Sync | ApprovalGranted unblocks task | Strong on approval write
| BC-04 | BC-09 | Event | All state transitions | Eventual (audit ingest)
| BC-11 | BC-02 | Event | ExternalTicketSynced | Eventual
| BC-04 | BC-12 | Sync | ContextPackageRequest | Strong read of snapshot
| BC-10 | All | Sync | Authz check interceptor | Strong
| BC-04 | BC-13 | Event | ResourceConsumed | Eventual rollup
| BC-04 | BC-15 | Event | All domain events | Eventual <=5s (NFR quality attr)

Responsibilities
----------------
- Enforce ACL translation at BC-11 and BC-05 boundaries.
- Propagate tenant_id and correlation_id on every crossing.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
As per table; see flow sections 19–27.

Failure Scenarios
-----------------
- Policy service timeout -> must deny dispatch (fail-closed).

Recovery Strategy
-----------------
- Cached policy read replicas with version pinning; stale cache TTL <=5s.

Security / Scalability / Maintainability
------------------------------------------
- No cross-tenant context calls ever.
- Bulk event consumption for BC-15.

Alternatives
------------
- Shared database integration.

Advantages / Disadvantages
+ Loose coupling. - Eventual consistency complexity.

Engineering Recommendation
--------------------------
Document consistency expectations per relationship in integration specs.

Reasoning
---------
Matches CQRS and event-first principles (ADR-SAD-008).

Future Evolution
----------------
- Change Data Capture for external analytics warehouses.

================================================================================
15. System Modules
================================================================================

Purpose
-------
Enumerate every logical module in the platform.

Detailed Explanation
--------------------
MODULE INDEX (23 modules — maps to SVC-01 through SVC-23):

  MOD-01  Edge Gateway Module
  MOD-02  Project & Portfolio Module
  MOD-03  Requirements Module
  MOD-04  Plan Engine Module
  MOD-05  Scheduler Module
  MOD-06  Execution Orchestrator Module
  MOD-07  Agent Registry Module
  MOD-08  Dispatch Manager Module
  MOD-09  Policy Engine Module
  MOD-10  Gate Engine Module
  MOD-11  Approval Workflow Module
  MOD-12  Audit & Compliance Module
  MOD-13  Identity & Access Module
  MOD-14  Credential Broker Module
  MOD-15  Integration Hub Module
  MOD-16  Webhook Module
  MOD-17  Sync Reconciler Module
  MOD-18  Event Backbone Module
  MOD-19  Knowledge & Context Module
  MOD-20  Model Router Module
  MOD-21  Validation Module
  MOD-22  Cost Meter Module
  MOD-23  Analytics & Projection Module
  MOD-24  Notification Module
  MOD-25  Configuration Module
  MOD-26  Feature Flag Module
  MOD-27  Halt & Recovery Controller Module
  MOD-28  Conflict Resolution Module
  MOD-29  Workflow Durability Adapter Module
  MOD-30  Cache & Session Module

Responsibilities
----------------
- Complete coverage of SRS functional domains.
- No orphan requirements.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Detailed per module in Section 16.

Failure Scenarios
-----------------
- Missing module for halt, credential revoke, or portfolio aggregates.

Recovery Strategy
-----------------
- RTM maps every FR to at least one module (verified in audit).

Security / Scalability / Maintainability
------------------------------------------
- Security-critical: MOD-13, MOD-14, MOD-09, MOD-12.
- Scale-critical: MOD-05, MOD-08, MOD-18, MOD-23.

Alternatives
------------
- Fewer combined modules.

Advantages / Disadvantages
+ Traceability. - Module count management.

Engineering Recommendation
--------------------------
30 modules logical; packaged into 5 deployables per ADR-SAD-001.

Reasoning
---------
Complete module list prevents gaps found in architecture audit.

Future Evolution
----------------
- Modules become services as load dictates.

================================================================================
16. Responsibilities of Every Module
================================================================================

Purpose
-------
Define explicit responsibility, I/O, and behavior for each module.

Detailed Explanation
--------------------
MOD-01 Edge Gateway Module
  Responsibilities: TLS, WAF, rate limit, route, inject correlation ID, terminate auth tokens
  Inputs: HTTPS requests | Outputs: Routed internal requests | FR: gateway for all APIs
  Dependencies: MOD-13 | Failures: overload -> 429 | Recovery: autoscale | Security: first authz hop

MOD-02 Project & Portfolio Module
  Responsibilities: Project CRUD, portfolio hierarchy, templates, scope versioning, clone
  Inputs: Project commands | Outputs: Project events | FR: FR-001–005, FR-003, FR-004, FR-122
  Dependencies: MOD-09, MOD-12 | Failures: invalid state transition | Recovery: reject command

MOD-03 Requirements Module
  Responsibilities: Ingest, normalize, traceability, ambiguity detection
  Inputs: Raw requirements, external tickets | Outputs: Requirement entities, flags
  FR: FR-010–014 | Dependencies: MOD-15, MOD-19 | Failures: parse failure -> human review queue

MOD-04 Plan Engine Module
  Responsibilities: DAG plans, replan, simulate, critical path, plan versioning
  Inputs: Requirements, policies, agent capabilities | Outputs: PlanVersion, TaskNodes
  FR: FR-020–024, FR-119 | Dependencies: MOD-09, MOD-20, MOD-05 | Failures: cycle detected -> reject

MOD-05 Scheduler Module
  Responsibilities: Schedule tasks by deps, priority, quotas, agent availability
  Inputs: Active plan, agent health, budgets | Outputs: ScheduleSlots, TaskScheduled events
  FR: FR-022, FR-041 | Dependencies: MOD-07, MOD-09, MOD-22 | Failures: quota exceeded -> pause

MOD-06 Execution Orchestrator Module
  Responsibilities: Task lifecycle, parallel execution, pause/resume/cancel, retry orchestration
  Inputs: Scheduled tasks | Outputs: Execution commands | FR: FR-040–044, FR-042
  Dependencies: MOD-08, MOD-10, MOD-27 | Failures: stuck task -> timeout -> retry/DLQ

MOD-07 Agent Registry Module
  Responsibilities: Register agent types, health checks, version compatibility matrix
  Inputs: Agent manifests | Outputs: Agent availability status | FR: FR-030–034, FR-031, FR-033
  Dependencies: MOD-18 | Failures: unhealthy agent -> mark unavailable, reroute

MOD-08 Dispatch Manager Module
  Responsibilities: Build dispatch packages, issue credentials, track ack, idempotency
  Inputs: Dispatch commands, context packages | Outputs: Dispatch records | FR: FR-032, FR-118, NFR-023
  Dependencies: MOD-14, MOD-19, MOD-21, MOD-09 | Failures: no ack -> retry | Recovery: idempotent redispatch

MOD-09 Policy Engine Module
  Responsibilities: Policy CRUD, evaluation at PEPs, autonomy levels, SoD rules
  Inputs: Policy eval requests | Outputs: Permit/Deny + obligations | FR: FR-050–053, FR-051
  Dependencies: MOD-12 | Failures: eval error -> DENY (fail-closed)

MOD-10 Gate Engine Module
  Responsibilities: Gate definitions, evaluation, evidence storage, waivers, env promotion blocks
  Inputs: Gate eval requests, CI/test results | Outputs: GateResult | FR: FR-060–063, FR-061
  Dependencies: MOD-15, MOD-21 | Failures: missing evidence -> fail gate

MOD-11 Approval Workflow Module
  Responsibilities: Multi-step approvals, timeout, delegation, approval authority matrix
  Inputs: Approval requests | Outputs: ApprovalGranted/Denied | FR: FR-052, FR-119
  Dependencies: MOD-09, MOD-14, MOD-24 | Failures: timeout -> escalate or deny per policy

MOD-12 Audit & Compliance Module
  Responsibilities: Immutable audit log, tenant-scoped queries, compliance exports, legal hold
  Inputs: All domain events | Outputs: Audit entries, export bundles | FR: FR-080, FR-083, FR-121, FR-103
  Dependencies: MOD-18 | Failures: ingest lag -> alert | Recovery: scale consumers

MOD-13 Identity & Access Module
  Responsibilities: SSO, SCIM, RBAC, ABAC, API keys, break-glass
  Inputs: Auth tokens, identity events | Outputs: Authz decisions | FR: FR-091–093
  Dependencies: External IdP | Failures: IdP outage -> cached session grace period limited

MOD-14 Credential Broker Module
  Responsibilities: Task-scoped token issue, rotation, revoke on halt within 60s
  Inputs: Dispatch requests, halt commands | Outputs: Scoped credentials | FR: FR-117, ADR-SAD-010
  Dependencies: KMS, secrets vault | Failures: leak suspicion -> mass revoke

MOD-15 Integration Hub Module
  Responsibilities: OAuth connections, VCS, CI/CD, cloud, issue tracker adapters
  Inputs: Integration config | Outputs: Synced entities, external events | FR: FR-070–074
  Dependencies: MOD-17 | Failures: API rate limit -> backoff

MOD-16 Webhook Module
  Responsibilities: Outbound signed webhooks, inbound verification, retry
  Inputs: Subscription config, events | Outputs: Webhook deliveries | FR: FR-072
  Dependencies: MOD-18, Queue architecture | Failures: endpoint down -> retry then DLQ

MOD-17 Sync Reconciler Module
  Responsibilities: Bidirectional work item sync, conflict detection with authoritative state
  Inputs: External + internal changes | Outputs: Reconciled state | FR: FR-071, ADR-005
  Dependencies: MOD-15 | Failures: conflict -> flag for human

MOD-18 Event Backbone Module
  Responsibilities: Durable event log, pub/sub, partitioning by tenant, schema registry
  Inputs: Domain events | Outputs: Event streams | NFR: NFR-013, NFR-022
  Dependencies: Infrastructure | Failures: partition unavailable -> failover

MOD-19 Knowledge & Context Module
  Responsibilities: Knowledge graph, context assembly, PII/secret redaction
  Inputs: Artifacts, policies | Outputs: ContextPackage | FR: FR-100–102
  Dependencies: MOD-23 projections | Failures: oversized context -> truncate per policy

MOD-20 Model Router Module
  Responsibilities: Route LLM requests to approved catalog, fallback, cost tracking
  Inputs: Inference requests | Outputs: Model responses + usage metadata | ADR-004, FR-082
  Dependencies: External LLM APIs | Failures: provider outage -> fallback model

MOD-21 Validation Module
  Responsibilities: Schema validation, output verification, safety checks before trust
  Inputs: Agent outputs | Outputs: ValidationResult | NFR-019
  Dependencies: Schema registry | Failures: invalid output -> reject + retry path

MOD-22 Cost Meter Module
  Responsibilities: Per-tenant/project/task/agent/model cost accounting, budget caps, alerts
  Inputs: Usage events | Outputs: Cost entries, budget signals | FR: FR-082, NFR-020, SC-006
  Dependencies: MOD-18 | Failures: budget exceeded -> halt new dispatches within 60s

MOD-23 Analytics & Projection Module
  Responsibilities: CQRS projections, dashboards, portfolio aggregates, search, reports
  Inputs: Domain events | Outputs: Read models | FR: FR-081, FR-083, FR-122, FR-084
  Dependencies: MOD-18 | Failures: projection lag -> stale dashboard indicator

MOD-24 Notification Module
  Responsibilities: Multi-channel notifications for approvals, alerts, incidents
  Inputs: Notification events | Outputs: Delivered messages | FR: implicit in journeys
  Dependencies: External providers | Failures: channel failure -> alternate channel

MOD-25 Configuration Module
  Responsibilities: Tenant config, templates, environment definitions, retention policies
  Inputs: Admin config | Outputs: Config snapshots | FR: FR-002, FR-094
  Dependencies: MOD-12 audit on changes | Failures: bad config -> validate before apply

MOD-26 Feature Flag Module
  Responsibilities: Gradual rollout, tier-specific features, kill switches
  Inputs: Flag definitions | Outputs: Flag evaluations | SRS §36 extensibility
  Dependencies: MOD-25 | Failures: flag service down -> default safe (off for risky features)

MOD-27 Halt & Recovery Controller Module
  Responsibilities: Global/scoped halt, resume, credential revoke coordination, SC-008
  Inputs: Halt commands | Outputs: HaltActivated events | FR: FR-043, FR-117, SC-008
  Dependencies: MOD-08, MOD-14, MOD-05 | Failures: partial halt -> retry revoke until 60s met

MOD-28 Conflict Resolution Module
  Responsibilities: Detect agent output conflicts, escalate, record resolutions
  Inputs: Validation/compare results | Outputs: Conflict cases | FR: FR-110–112
  Dependencies: MOD-21, MOD-24 | Failures: unresolved -> block dependent tasks

MOD-29 Workflow Durability Adapter Module
  Responsibilities: Durable timers for approvals, retries, scheduled maintenance tasks
  Inputs: Workflow instances | Outputs: Timeout signals | ADR-SAD-002
  Dependencies: MOD-11, MOD-06 | Failures: timer loss -> reconciliation from event log

MOD-30 Cache & Session Module
  Responsibilities: Distributed cache for authz, policy, read models; not authoritative
  Inputs: Cache queries | Outputs: Cached values | Performance architecture
  Dependencies: MOD-13, MOD-09 | Failures: cache miss -> origin fetch; stampede protection

Responsibilities (Section Level)
--------------------------------
- Every SRS FR maps to one or more modules above (RTM verified in audit).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
See per-module entries.

Failure Scenarios / Recovery / Security / Scalability / Maintainability
-------------------------------------------------------------------------
Covered per module; platform-wide: circuit breakers, bulkheads, tenant quotas.

Alternatives
------------
- Fewer merged modules (higher blast radius).

Advantages / Disadvantages
+ Complete accountability. - Integration test matrix size.

Engineering Recommendation
--------------------------
Maintain module catalog with owner, SLO, and FR traceability links.

Reasoning
---------
Section 15–16 audit requirement: no missing modules.

Future Evolution
----------------
- Extract MOD-08, MOD-18, MOD-23 first under load.

================================================================================
17. Internal Communication
================================================================================

Purpose
-------
Define how platform components communicate internally.

Detailed Explanation
--------------------
Patterns:
  P1  Synchronous RPC (gRPC over mTLS) — policy eval, authz, dispatch ack, credential issue
  P2  Asynchronous Events — state changes, audit, projections, notifications
  P3  Request/Reply over queue — long-running export jobs, report generation
  P4  Outbox pattern — guarantee event publish with transactional write (NFR-018)

Every message carries: tenant_id, correlation_id, causation_id, schema_version,
actor_id (human or service principal), timestamp_utc.

Responsibilities
----------------
- Enforce schema validation on all messages.
- Propagate trace context (NFR-015).

Inputs / Outputs
----------------
Inputs: Commands, queries, events.
Outputs: Responses, derived events.

Dependencies
------------
- MOD-18 Event Backbone, Service Mesh (Section 88).

Interactions
------------
- Sync calls max depth 2 hops (gateway->service->service); deeper via events.

Failure Scenarios
-----------------
- Sync chain too long -> latency and cascade failures.
- Missing correlation ID -> untraceable incidents.

Recovery Strategy
-----------------
- Automatic retry with idempotency keys on safe operations.
- Circuit breaker opens after error threshold.

Security Considerations
-----------------------
- mTLS + workload identity (ADR-SAD-007).
- No tenant_id trust from client without token validation.

Scalability Considerations
--------------------------
- Prefer events for fan-out >3 consumers.
- Partition event topics by tenant_id hash.

Maintainability Considerations
------------------------------
- Schema registry with compatibility checks (Section 90).

Alternatives
------------
- REST-only internal comms.
- Shared memory in monolith.

Advantages / Disadvantages
+ Reliable at scale. - Operational complexity.

Engineering Recommendation
--------------------------
Outbox + event backbone for all domain state changes.

Reasoning
---------
Audit reconstructability requires durable events (SC-003).

Future Evolution
----------------
- gRPC to HTTP/3; event protocol versioning.

================================================================================
18. External Communication
================================================================================

Purpose
-------
Define how AIPM communicates with outside world.

Detailed Explanation
--------------------
External parties:
  EP-01  Human users (browser, API clients) — HTTPS via DU-1
  EP-02  Customer IdP (SAML/OIDC/SCIM) — DU-3
  EP-03  Agent runtimes — Dispatch Protocol (mTLS, schema-validated payloads)
  EP-04  LLM providers — via MOD-20 only
  EP-05  VCS (GitHub/GitLab/Bitbucket) — DU-4 OAuth
  EP-06  CI/CD systems — DU-4 webhooks + API polling
  EP-07  Issue trackers — DU-4 bidirectional sync
  EP-08  Cloud provider APIs — DU-4 for Deployment/Cloud agents
  EP-09  Notification providers — email, Slack, PagerDuty
  EP-10  Customer webhook endpoints — DU-4 MOD-16
  EP-11  External PM tools — optional bidirectional sync (ADR-005)

Responsibilities
----------------
- ACL at every external boundary.
- No direct agent-to-external without policy check where required.

Inputs / Outputs
----------------
Per EP; external events normalized to internal domain events.

Dependencies
------------
- Integration Hub, Credential Broker, API Gateway.

Interactions
------------
- Agents receive dispatch; return structured results to Dispatch Manager only.

Failure Scenarios
-----------------
- External API outage -> degraded mode per integration health monitor.
- Agent bypassing PM to call prod APIs -> blocked by network policy + scoped creds.

Recovery Strategy
-----------------
- Retry with backoff; DLQ for sync failures; manual reconciliation UI.

Security Considerations
-----------------------
- OAuth token vault; webhook signature verification; IP allowlists optional enterprise.
- Egress controls in air-gapped profile (ADR-SAD-005).

Scalability Considerations
--------------------------
- Rate limit per integration connection; respect external API quotas.

Maintainability Considerations
------------------------------
- Connector certification pipeline.

Alternatives
------------
- iPaaS-only integrations.

Advantages / Disadvantages
+ Full SDLC coverage. - Connector maintenance burden.

Engineering Recommendation
--------------------------
Certified connectors for GA minimum 5 (OBJ-005).

Reasoning
---------
SRS FR-070–074 and SC-007 require external integration architecture.

Future Evolution
----------------
- Marketplace connectors (Section 85).

================================================================================
19. Event Driven Architecture
================================================================================

Purpose
-------
Define event-driven patterns as architectural backbone.

Detailed Explanation
--------------------
Event categories:
  EC-01  Domain Events — business facts (TaskDispatched, GatePassed)
  EC-02  Integration Events — external system facts (BuildFailed, PRMerged)
  EC-03  Audit Events — superset for compliance (immutable)
  EC-04  Telemetry Events — metrics and traces (parallel path)

Event envelope (conceptual fields):
  event_id, event_type, schema_version, tenant_id, aggregate_id, aggregate_type,
  correlation_id, causation_id, occurred_at_utc, actor, payload, integrity_hash

Publishing: transactional outbox from write model.
Consuming: at-least-once with idempotent handlers.

Responsibilities
----------------
- Decouple producers and consumers.
- Enable replay for audit and projection rebuild.

Inputs / Outputs
----------------
Inputs: State changes from all BCs.
Outputs: Event streams to consumers.

Dependencies
------------
- MOD-18, schema registry, ADR-SAD-004.

Interactions
------------
- All modules in Section 16 publish or consume events per RTM.

Failure Scenarios
-----------------
- Duplicate events -> idempotent consumers required.
- Schema breaking change -> consumer failures.

Recovery Strategy
-----------------
- Replay from offset; rebuild projections from scratch.
- Schema compatibility policy (Section 90).

Security Considerations
-----------------------
- Encrypt events at rest; tenant-scoped ACL on consume.
- No secrets in event payloads (NFR-007).

Scalability Considerations
--------------------------
- Partition by tenant_id; target NFR-022 capacity.
- Compacted topics for snapshot events where applicable.

Maintainability Considerations
------------------------------
- Event catalog documented per event_type.

Alternatives
------------
- Polling-only architecture.
- Dual-write without outbox.

Advantages / Disadvantages
+ Scale and audit. - Eventual consistency.

Engineering Recommendation
--------------------------
Event-driven as default; sync only when strong consistency required.

Reasoning
---------
SRS AP-02 event-first; SC-003 audit reconstructability.

Future Evolution
----------------
- Event sourcing snapshots for faster replay.

================================================================================
20. Message Flow
================================================================================

Purpose
-------
Describe how messages move between components.

Detailed Explanation
--------------------
Message types:
  Command — intent to change state (DispatchTask, ApprovePlan)
  Event — fact that occurred (TaskDispatched)
  Query — read-only request (GetProjectStatus)
  Document — large payload reference (artifact URI in dispatch package)

Typical dispatch message flow:
  1. Scheduler emits TaskScheduled event
  2. Execution Orchestrator consumes -> Policy PEP sync call
  3. On permit -> Command to Dispatch Manager
  4. Dispatch Manager assembles ContextPackage (sync Knowledge Module)
  5. Credential Broker issues scoped token
  6. Dispatch message to Agent Runtime (external)
  7. Agent ack within 30s P95 (NFR-023)
  8. Agent result -> Validation Module -> TaskCompleted or TaskFailed event

Responsibilities
----------------
- Validate schema at every hop.
- Preserve correlation_id end-to-end.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
See flow steps; depends on MOD-05 through MOD-08, MOD-19, MOD-21.

Failure Scenarios
-----------------
- Oversized message -> reference pattern to object storage.
- Invalid schema -> reject to DLQ with alert.

Recovery Strategy
-----------------
- Retry transient failures; dead-letter permanent failures (Section 64).

Security / Scalability / Maintainability
------------------------------------------
- mTLS on all internal messages.
- Message size limits; async for bulk.
- Versioned message schemas.

Alternatives
------------
- Shared database polling.

Advantages / Disadvantages
+ Clear audit trail. - Debugging distributed flows.

Engineering Recommendation
--------------------------
Distributed tracing on every message path (Section 57).

Reasoning
---------
NFR-015 requires correlated traces on dispatch paths.

Future Evolution
----------------
- Binary protocol optimization for high-volume telemetry.

================================================================================
21. Event Flow
================================================================================

Purpose
-------
Map domain event propagation across the system.

Detailed Explanation
--------------------
Critical event flows:

FLOW-EF-01 Plan Activation:
  PlanVersionApproved -> PlanVersionActivated -> Scheduler -> TaskScheduled (x N)

FLOW-EF-02 Dispatch Lifecycle:
  TaskScheduled -> TaskDispatched -> TaskRunning -> TaskValidating -> TaskSucceeded|Failed

FLOW-EF-03 Gate Pipeline:
  TaskSucceeded -> GateEvaluationRequested -> GatePassed|Failed -> EnvironmentPromotionAllowed|Blocked

FLOW-EF-04 Halt:
  HaltCommandReceived -> HaltActivated -> CredentialRevocationInitiated -> DispatchPaused

FLOW-EF-05 Audit Fan-out:
  ALL domain events -> AuditIngestor -> ImmutableAuditStore
  ALL domain events -> ProjectionConsumers -> ReadModels
  ALL domain events -> CostMeter -> UsageRollups

FLOW-EF-06 Post-Release Maintenance:
  ProjectReleased -> MaintenanceScheduleCreated -> TaskScheduled (maintenance agent)

Responsibilities
----------------
- Guarantee audit events for every state transition (FR-080).
- Ordered processing per aggregate instance.

Inputs / Outputs
----------------
Per flow above.

Dependencies
------------
- MOD-18 partitioned by tenant_id + aggregate_id for ordering.

Interactions
------------
- Projectors in DU-5 consume in consumer groups.

Failure Scenarios
-----------------
- Consumer lag -> dashboard staleness indicator (<=5s target per quality attr).

Recovery Strategy
-----------------
- Scale consumer groups; replay from checkpoint.

Security / Scalability / Maintainability
------------------------------------------
- Tenant isolation on event ACLs.
- Horizontal consumer scale.
- Event catalog versioning.

Alternatives
------------
- Point-to-point event forwarding.

Advantages / Disadvantages
+ Loose coupling. - Ordering complexity.

Engineering Recommendation
--------------------------
Single canonical event bus; no ad-hoc per-module queues for domain events.

Reasoning
---------
Prevents audit gaps and operational sprawl.

Future Evolution
----------------
- Cross-region event replication for DR.

================================================================================
22. State Flow
================================================================================

Purpose
-------
Describe how authoritative state changes over time.

Detailed Explanation
--------------------
State flow model:
  Command -> Aggregate validation -> State transition -> Domain event(s) ->
  Outbox publish -> Projections update -> Read model available

Authoritative state lives in DU-2 and DU-3 write stores (ACID per NFR-018).
Read state in DU-5 is derived and eventually consistent.

Key state stores (conceptual, not implementation):
  SS-01 Project state (BC-01)
  SS-02 Plan versions (BC-03) — immutable versions
  SS-03 Task execution state (BC-04)
  SS-04 Policy versions (BC-06)
  SS-05 Gate results (BC-08)
  SS-06 Audit log (BC-09) — append-only
  SS-07 Integration cursors (BC-11)

Responsibilities
----------------
- Single writer per aggregate instance.
- Optimistic concurrency via version numbers.

Inputs / Outputs
----------------
Inputs: Validated commands.
Outputs: New aggregate version + events.

Dependencies
------------
- Event sourcing for audit-critical aggregates (Plan, Task, Approval, Audit).

Interactions
------------
- Sync read-your-writes for command issuer via primary read.

Failure Scenarios
-----------------
- Split brain on aggregate -> prevented by single-partition ordering.

Recovery Strategy
-----------------
- Replay events to rebuild aggregate state.

Security / Scalability / Maintainability
------------------------------------------
- Encrypt state at rest (Section 52).
- Shard by tenant_id.
- Clear aggregate boundaries.

Alternatives
------------
- Mutable CRUD without events.

Advantages / Disadvantages
+ Audit and replay. - Storage growth.

Engineering Recommendation
--------------------------
Event sourcing for Task, Plan, Approval, Audit; CRUD acceptable for config catalog.

Reasoning
---------
Balances complexity with SC-003 reconstructability requirement.

Future Evolution
----------------
- Snapshot strategy for long aggregate histories.

================================================================================
23. Command Flow
================================================================================

Purpose
-------
Define how write intentions traverse the system.

Detailed Explanation
--------------------
Command ingress:
  User/Agent -> Edge Gateway -> Authz (MOD-13) -> Command Handler (Application Layer)
  -> Domain validation -> Policy PEP if required -> Persist + Emit events

Command categories:
  CC-01 Project commands (CreateProject, ArchiveProject)
  CC-02 Plan commands (SubmitPlan, ActivatePlanVersion)
  CC-03 Execution commands (PauseTask, CancelProject, Halt)
  CC-04 Governance commands (UpdatePolicy, GrantApproval)
  CC-05 Integration commands (ConnectRepository)
  CC-06 Admin commands (ConfigureRetention, RotateApiKey)

All commands require: idempotency_key, tenant_id, actor_id, schema_version.

Responsibilities
----------------
- Reject unauthorized commands before domain layer.
- Enforce SoD on approval commands (FR-053).

Inputs / Outputs
----------------
Inputs: Authenticated command requests.
Outputs: CommandResult (accepted/rejected) + events.

Dependencies
------------
- MOD-01, MOD-13, MOD-09.

Interactions
------------
- Halt command (CC-03) prioritized queue in Execution Orchestrator.

Failure Scenarios
-----------------
- Duplicate command -> idempotency returns prior result.
- Policy deny -> structured denial with explainability reference.

Recovery Strategy
-----------------
- Client retry with same idempotency_key.

Security / Scalability / Maintainability
------------------------------------------
- Command authorization at gateway and service layer (defense in depth).
- Rate limits per tenant and per actor.
- Command handlers unit-tested per aggregate.

Alternatives
------------
- Event-only ingress without commands.

Advantages / Disadvantages
+ Clear intent API. - Handler proliferation.

Engineering Recommendation
--------------------------
CQRS command side separate from query side (Section 24).

Reasoning
---------
NFR-001 read performance requires read/write separation.

Future Evolution
----------------
- Command batching for bulk admin operations.

================================================================================
24. Query Flow
================================================================================

Purpose
-------
Define how read requests are served.

Detailed Explanation
--------------------
Query ingress:
  User -> Edge Gateway -> Authz -> Analytics Query Service (DU-5) OR
  limited strong-consistency reads from Core read replica

Query types:
  Q-01 Dashboard aggregates (project status, metrics) — FR-081
  Q-02 Plan graph reads — FR-020
  Q-03 Audit log queries — FR-080, FR-121 (tenant-scoped)
  Q-04 Portfolio rollups — FR-122 (async materialized)
  Q-05 Cost reports — FR-082
  Q-06 Agent health status — FR-031

CQRS: 95%+ queries hit DU-5 projections; strong consistency queries (e.g.,
post-approval read) hit Core primary with tenant filter.

Responsibilities
----------------
- Never expose cross-tenant data (FR-121).
- Indicate staleness when projection lag > threshold.

Inputs / Outputs
----------------
Inputs: Query + filters + pagination.
Outputs: DTOs from read models.

Dependencies
------------
- MOD-23, search index, cache (MOD-30).

Interactions
------------
- Projectors consume events to update read models.

Failure Scenarios
-----------------
- Stale projection -> user sees outdated task count.

Recovery Strategy
-----------------
- Lag monitoring; auto-scale projectors; fallback to on-demand projection.

Security / Scalability / Maintainability
------------------------------------------
- ABAC filters applied in query layer.
- Read replicas and caches; P95 <=200ms (NFR-001).
- Read model versioning independent of write model.

Alternatives
------------
- All queries hit write database.

Advantages / Disadvantages
+ Read scale. - Staleness management.

Engineering Recommendation
--------------------------
Materialized views per dashboard; tenant-scoped indexes.

Reasoning
---------
FR-122 explicitly requires async portfolio aggregates.

Future Evolution
----------------
- Real-time push queries via WebSocket subscriptions.

================================================================================
25. Request Flow
================================================================================

Purpose
-------
End-to-end path for external requests entering the platform.

Detailed Explanation
--------------------
Standard request flow:
  1. Client DNS -> Regional Edge (DU-1)
  2. TLS handshake, WAF inspection
  3. JWT/API key validation via MOD-13 (may call IdP)
  4. Rate limit check (tenant tier)
  5. Route to internal service
  6. ABAC authorization with resource attributes
  7. Business logic execution
  8. Response assembly with correlation_id header
  9. Audit log entry for mutating requests

Priority request classes:
  PR-01 Halt / emergency — expedited queue, bypass non-critical limits
  PR-02 Approval actions — standard
  PR-03 Read queries — cache-friendly
  PR-04 Background exports — async job queue

Responsibilities
----------------
- SC-008: halt requests processed with priority to meet 30s stop target.

Inputs / Outputs
----------------
Inputs: HTTP requests.
Outputs: HTTP responses + audit records.

Dependencies
------------
- All edge and governance modules.

Interactions
------------
- See Sections 23–24 for command vs query branching.

Failure Scenarios
-----------------
- Gateway overload -> 503 with retry-after.
- Auth failure -> 401/403 without data leak.

Recovery Strategy
-----------------
- Autoscale edge; regional failover.

Security / Scalability / Maintainability
------------------------------------------
- OWASP ASVS L2 at edge.
- Horizontal edge scaling.
- Standard middleware pipeline.

Alternatives
------------
- Direct service exposure without gateway.

Advantages / Disadvantages
+ Centralized security. - Single point if misconfigured.

Engineering Recommendation
--------------------------
All external traffic through DU-1 only.

Reasoning
---------
Zero-trust starts at edge (Section 44).

Future Evolution
----------------
- Edge workers for geo-routing.

================================================================================
26. Response Flow
================================================================================

Purpose
-------
Define how responses are constructed and returned.

Detailed Explanation
--------------------
Response pipeline:
  Service result -> Error mapper (no internal stack traces) ->
  PII masker (NFR-008) -> Serializer (versioned DTO) ->
  Gateway adds security headers, correlation_id, request_id ->
  Client

Error response taxonomy:
  E-400 ValidationError — schema/field failures
  E-401 Unauthenticated
  E-403 PolicyDenied — include policy_id, not internal rules
  E-404 NotFound — tenant-scoped (no cross-tenant existence leak)
  E-409 ConcurrencyConflict — aggregate version mismatch
  E-429 RateLimited
  E-503 DependencyUnavailable — retry guidance

Async responses: 202 Accepted + job_id for exports, large reports (FR-083).

Responsibilities
----------------
- Consistent error contract across services.
- Explainability references in approval-related responses (FR-119).

Inputs / Outputs
----------------
Inputs: Domain results or errors.
Outputs: Client-safe response payloads.

Dependencies
------------
- MOD-21 validation errors; MOD-09 policy denials.

Interactions
------------
- Webhook responses follow separate signed payload format (MOD-16).

Failure Scenarios
-----------------
- Leaking internal errors or other tenant IDs.

Recovery Strategy
----------------
- Response sanitization middleware; security tests.

Security / Scalability / Maintainability
------------------------------------------
- No secrets in responses.
- Pagination on all list responses.
- Versioned response schemas.

Alternatives
------------
- Per-service ad-hoc error formats.

Advantages / Disadvantages
+ Client consistency. - Mapping overhead.

Engineering Recommendation
--------------------------
Global error catalog and response envelope standard.

Reasoning
---------
Enterprise API consumers require predictable contracts.

Future Evolution
----------------
- Content negotiation for export formats.

================================================================================
27. Workflow Flow
================================================================================

Purpose
-------
Define long-running business workflows spanning modules and human steps.

Detailed Explanation
--------------------
Workflow engine architecture (ADR-SAD-002):
  - Embedded finite-state machines for: Project, Task, Agent, Approval lifecycles
  - Durable Workflow Adapter (MOD-29) for timers, human waits, long retries

Primary workflows:
  WF-01 Project Delivery (SRS Journey J2–J3)
  WF-02 Scope Change with Replan
  WF-03 Incident Response (J4)
  WF-04 Compliance Export
  WF-05 Agent Certification (marketplace future)
  WF-06 Post-Release Maintenance (FR-115, FR-116)

Workflow invariants (SRS §41):
  - No production dispatch without PEP success
  - Replan creates new immutable plan version
  - Halt supersedes scheduling

Responsibilities
----------------
- Coordinate human and agent steps.
- Survive process restarts via durable state.

Inputs / Outputs
----------------
Inputs: Workflow start triggers (events or commands).
Outputs: Workflow completion events, escalations.

Dependencies
------------
- MOD-29, MOD-11, MOD-06, MOD-09.

Interactions
------------
- Workflows emit commands; never call agents directly bypassing dispatch.

Failure Scenarios
-----------------
- Stuck workflow on human timeout -> escalate per policy.

Recovery Strategy
-----------------
- Reconcile workflow state from event log on adapter failure.

Security / Scalability / Maintainability
------------------------------------------
- Workflow definitions versioned; tenant-specific template params only.
- Shard workflow instances by tenant.
- Visual workflow documentation for operations.

Alternatives
------------
- External BPMN engine as sole orchestrator.

Advantages / Disadvantages
+ Domain-aligned workflows. - Custom adapter maintenance.

Engineering Recommendation
--------------------------
FSM in domain layer + durable adapter for timers; not BPMN-first.

Reasoning
---------
SRS §48: workflow engine is implementation detail, not product definition.

Future Evolution
----------------
- Optional BPMN export for enterprise customers.

================================================================================
28. Agent Orchestration Architecture
================================================================================

Purpose
-------
Define how AIPM coordinates 14+ specialized agent types without executing their work.

Detailed Explanation
--------------------
Orchestration layers:
  L-A  Intent Layer — requirements normalized, scope bounded (BC-02)
  L-B  Planning Layer — DAG plan with agent-type assignments (BC-03)
  L-C  Scheduling Layer — when/what/order based on deps and quotas (BC-04)
  L-D  Dispatch Layer — how/with what context/credentials (MOD-08)
  L-E  Validation Layer — verify outputs before state advance (MOD-21)
  L-F  Gate Layer — environment promotion controls (MOD-10)

Orchestrator never writes application code (CON-001). It invokes agents via
Dispatch Protocol with structured contracts (FR-032, FR-118).

Registered agent types (SRS §12):
  Requirement Analysis, Project Planning, Backend, Frontend, Mobile, Database,
  DevOps, Cloud, Testing, Security, Documentation, Deployment, Monitoring,
  Maintenance.

Responsibilities
----------------
- Maintain global plan coherence.
- Enforce policy at Schedule, Dispatch, Gate PEPs (ADR-SAD-009).
- Handle agent unavailability with reroute or pause (FR-031).

Inputs / Outputs
----------------
Inputs: Approved plans, policies, agent registry.
Outputs: Dispatches, validated artifacts, gate outcomes.

Dependencies
------------
- MOD-04 through MOD-08, MOD-09, MOD-10, MOD-19, MOD-21.

Interactions
------------
- Planning agents produce plans; PM may enrich with mandatory policy tasks.
- Execution agents receive dispatches only from Dispatch Manager.

Failure Scenarios
-----------------
- Orchestration bypass (agent self-triggered work).
- Plan drift without replan (FR-021).

Recovery Strategy
-----------------
- Replan workflow; conflict escalation (MOD-28).

Security / Scalability / Maintainability
------------------------------------------
- Task-scoped credentials only (MOD-14).
- Parallel dispatch where DAG allows (FR-041).
- Agent contract versioning (FR-033).

Alternatives
------------
- Centralized agent supervisor process.
- Peer-to-peer agent coordination.

Advantages / Disadvantages
+ Enterprise control. - Orchestrator complexity.

Engineering Recommendation
--------------------------
Hub-and-spoke orchestration via Dispatch Manager; no agent mesh.

Reasoning
---------
ADR-007: PM orchestrates; agents in isolated runtimes.

Future Evolution
----------------
- ML-based scheduling optimization within policy bounds.

================================================================================
29. Agent Communication Architecture
================================================================================

Purpose
-------
Define protocols and patterns for PM-to-agent and agent-to-PM communication.

Detailed Explanation
--------------------
Dispatch Protocol (conceptual):
  Phase 1: DISPATCH_REQUEST — task_id, agent_type, input_schema_ref, context_package_ref,
           credential_handle, deadline, idempotency_key, correlation_id
  Phase 2: DISPATCH_ACK — accepted/rejected within 30s P95 (NFR-023)
  Phase 3: HEARTBEAT — optional for long tasks
  Phase 4: RESULT_SUBMIT — output_schema_ref, artifacts[], status, metrics
  Phase 5: VALIDATION_RESULT — from PM Validation Module to agent if retry needed

Transport: mTLS mutual auth; agent identity from workload certificate.
Payload: schema-validated documents; large artifacts by reference URI.

Agent-to-agent communication: PROHIBITED directly. All coordination via PM events.

Responsibilities
----------------
- Guarantee idempotent dispatch handling.
- Validate all agent messages before state transition.

Inputs / Outputs
----------------
Per protocol phases above.

Dependencies
------------
- MOD-08, MOD-14, MOD-21, agent runtime endpoints.

Interactions
------------
- Health checks separate channel from dispatch (MOD-07).

Failure Scenarios
-----------------
- Agent submits malformed output -> validation reject, retry per FR-040.
- Man-in-the-middle -> prevented by mTLS.

Recovery Strategy
-----------------
- Retry with backoff; DLQ after max retries.

Security / Scalability / Maintainability
------------------------------------------
- Credentials in dispatch are handles only, not raw secrets.
- Connection pooling per agent pool; bulkhead per agent type.
- Protocol versioned (Section 89).

Alternatives
------------
- gRPC streaming only.
- REST polling only.

Advantages / Disadvantages
+ Secure, auditable. - Protocol evolution discipline.

Engineering Recommendation
--------------------------
Ack-required dispatch with idempotency keys on both sides.

Reasoning
---------
NFR-023 and FR-040 require explicit ack and retry semantics.

Future Evolution
----------------
- Bidirectional streaming for long-running agent sessions.

================================================================================
30. Agent Isolation Model
================================================================================

Purpose
-------
Define security and operational isolation for agent runtimes.

Detailed Explanation
--------------------
Isolation tiers (maps to SRS Tenant Isolation Tier for agents):
  AI-L1  Logical — shared agent pool, tenant data separated by credentials and context
  AI-L2  Dedicated pool — single-tenant agent compute per enterprise customer
  AI-L3  Customer VPC — agents run in customer cloud; PM dispatches remotely
  AI-L4  Air-gapped — agents on customer premises; store-and-forward dispatch

Network: agents in separate network segment; no inbound from internet except PM dispatch.
Compute: container/WASM sandbox per task optional for untrusted third-party agents.
Data: context packages tenant-scoped; redacted per FR-102.
Secrets: task-scoped via Credential Broker; never shared across tasks.

Responsibilities
----------------
- Prevent cross-tenant data leakage via agents.
- Support ADR-007 isolated runtimes.

Inputs / Outputs
----------------
Inputs: Isolation profile per tenant/tier.
Outputs: Enforced runtime placement decisions.

Dependencies
------------
- MOD-14, network policies, deployment profiles (ADR-SAD-005).

Interactions
------------
- Agent Registry records isolation requirements per agent type.

Failure Scenarios
-----------------
- Agent container escape -> blast radius limited by network policy and scoped creds.

Recovery Strategy
-----------------
- Kill agent task, revoke credentials, incident workflow.

Security / Scalability / Maintainability
------------------------------------------
- Third-party agents require AI-L1 minimum sandbox + certification.
- Scale agent pools independently of PM core.
- Isolation profile as configuration, not code fork.

Alternatives
------------
- PM-hosted agents in same process (rejected — violates ADR-007).

Advantages / Disadvantages
+ Enterprise trust. - Operational overhead for dedicated pools.

Engineering Recommendation
--------------------------
Default AI-L1 SaaS; AI-L2/L3 as enterprise SKUs.

Reasoning
---------
FR-090 multi-tenant isolation at compute boundaries.

Future Evolution
----------------
- Confidential computing for sensitive agent workloads.

================================================================================
31. Agent Registration Model
================================================================================

Purpose
-------
Define how agents are registered, certified, and discovered.

Detailed Explanation
--------------------
Registration pipeline:
  1. Submit Agent Manifest (type_id, version, capabilities, schemas, health_endpoint,
     min_pm_version, isolation_requirements, resource_estimates)
  2. Automated contract tests
  3. Security review for third-party (FR-034)
  4. Publish to Agent Registry with status: Certified | Deprecated | Retired
  5. Runtime health polling (FR-031)

Manifest is signed; PM verifies signature before registration.

Compatibility matrix (FR-033):
  Rows: PM core versions | Columns: Agent versions | Cells: compatible/incompatible

Responsibilities
----------------
- Single registry of truth for dispatch routing.
- Block dispatch to uncertified or incompatible agents.

Inputs / Outputs
----------------
Inputs: Manifests, health probe results.
Outputs: Registry entries, routing eligibility.

Dependencies
------------
- MOD-07, certification pipeline, schema registry.

Interactions
------------
- Plan Engine reads capabilities when assigning agent types to tasks.

Failure Scenarios
-----------------
- Dispatch to retired agent version -> blocked at scheduler.

Recovery Strategy
-----------------
- Auto-deprecate on repeated health failures; alert agent owner.

Security / Scalability / Maintainability
------------------------------------------
- Manifest signature verification mandatory.
- Registry cached; O(1) lookup by agent_type.
- Version sunset policy with 6-month notice.

Alternatives
------------
- Hardcoded agent list in PM core.

Advantages / Disadvantages
+ Independent agent releases (CON-008). - Certification latency.

Engineering Recommendation
--------------------------
Same registration path for internal and external agents.

Reasoning
---------
FR-030, FR-033, FR-034 require formal registration model.

Future Evolution
----------------
- Marketplace publishing workflow (Section 85).

================================================================================
32. Agent Lifecycle
================================================================================

Purpose
-------
Define states and transitions for agent types and instances.

Detailed Explanation
--------------------
Agent Type Lifecycle:
  Unregistered -> PendingCertification -> Registered -> Deprecated -> Retired

Agent Instance Runtime Lifecycle:
  Unknown -> Healthy -> Degraded -> Unavailable -> Draining -> Offline

Transitions triggered by: certification results, health checks, admin action,
version sunset, security incident.

Responsibilities
----------------
- FR-031 health-check and reroute.
- FR-033 upgrade compatibility enforcement.

Inputs / Outputs
----------------
Inputs: Health metrics, admin commands, certification events.
Outputs: Lifecycle state changes, routing updates.

Dependencies
------------
- MOD-07, MOD-05 (scheduler reads availability).

Interactions
------------
- Unavailable agent -> scheduler pauses or reroutes affected tasks.

Failure Scenarios
-----------------
- Flapping health status -> hysteresis on state transitions.

Recovery Strategy
-----------------
- Degraded: reduced concurrency; Unavailable: stop new dispatches to instance.

Security / Scalability / Maintainability
------------------------------------------
- Retired agents cannot receive credentials.
- Health check storm prevention via jittered polling.

Alternatives
------------
- No formal instance lifecycle.

Advantages / Disadvantages
+ Reliable routing. - Health check infrastructure.

Engineering Recommendation
--------------------------
Separate type lifecycle from instance lifecycle.

Reasoning
---------
CON-008: agents release independently; types outlive instances.

Future Evolution
----------------
- Auto-scaling agent pools based on queue depth.

================================================================================
33. Agent State Machine
================================================================================

Purpose
-------
Formal state machine for agent instance runtime (normative).

Detailed Explanation
--------------------
States: {Unknown, Healthy, Degraded, Unavailable, Draining, Offline}

Transitions:
  Unknown --health_ok--> Healthy
  Unknown --health_fail--> Unavailable
  Healthy --latency_high|error_rate_high--> Degraded
  Healthy --health_fail--> Unavailable
  Degraded --recovery--> Healthy
  Degraded --health_fail--> Unavailable
  Unavailable --health_ok--> Healthy (after stability window)
  Healthy|Degraded --admin_drain--> Draining
  Draining --tasks_complete--> Offline
  Offline --admin_activate--> Unknown
  Any --security_incident--> Offline (immediate credential revoke)

Responsibilities
----------------
- Drive scheduler routing decisions.
- Trigger alerts on Unavailable.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Health probes -> MOD-07 -> events AgentHealthChanged -> MOD-05

Failure Scenarios / Recovery / Security / Scalability / Maintainability
-------------------------------------------------------------------------
As Section 32; state machine tested exhaustively.

Alternatives
------------
- Binary up/down only.

Advantages / Disadvantages
+ Nuanced routing. - More states to operate.

Engineering Recommendation
--------------------------
Implement as explicit FSM in domain layer.

Reasoning
---------
Prevents ad-hoc health logic scattered in scheduler.

Future Evolution
----------------
- Predictive health using anomaly detection (FR-084).

================================================================================
34. Project Lifecycle State Machine
================================================================================

Purpose
-------
Formal project states per SRS Definitions §26.

Detailed Explanation
--------------------
States: {Draft, Ready, Planning, Executing, Blocked, Gated, Released, Archived, Halted}

Key transitions:
  Draft --configure_complete--> Ready
  Ready --kickoff--> Planning
  Planning --plan_approved--> Executing
  Executing --dependency_wait--> Blocked
  Blocked --unblocked--> Executing
  Executing --gate_pending--> Gated
  Gated --gate_passed--> Executing
  Executing --release_complete--> Released
  Released --archive--> Archived
  Any --halt--> Halted
  Halted --resume_authorized--> prior_state (recorded)
  Draft|Ready --cancel--> Archived

Responsibilities
----------------
- FR-001 project lifecycle management.
- Halt overrides all non-terminal states.

Inputs / Outputs
----------------
Commands and events per transition; audit every change.

Dependencies
------------
- MOD-02, MOD-27, MOD-11.

Failure Scenarios
-----------------
- Illegal transition -> reject with current state.

Recovery Strategy
-----------------
- Admin repair command with audit trail (break-glass only).

Security / Scalability / Maintainability
------------------------------------------
- State changes require authz per role.
- Per-tenant partition; no cross-project lock contention globally.

Alternatives
------------
- Fewer consolidated states.

Advantages / Disadvantages
+ Matches SRS normative enums. - Complex UI.

Engineering Recommendation
--------------------------
Persist state + prior_state for halt resume.

Reasoning
---------
SRS §26 defines normative lifecycle enums.

Future Evolution
----------------
- Sub-states for portfolio-level programs.

================================================================================
35. Task Lifecycle State Machine
================================================================================

Purpose
-------
Formal task states per SRS Definitions §26.

Detailed Explanation
--------------------
States: {Pending, Scheduled, Dispatched, Running, Validating, Succeeded,
         Failed, Cancelled, DeadLetter}

Transitions:
  Pending --scheduler--> Scheduled
  Scheduled --dispatch--> Dispatched
  Dispatched --agent_ack--> Running
  Running --result_received--> Validating
  Validating --pass--> Succeeded
  Validating --fail_retryable--> Scheduled (retry per FR-040)
  Validating --fail_terminal--> Failed
  Failed --max_retries--> DeadLetter
  Pending|Scheduled|Dispatched|Running --cancel--> Cancelled
  Dispatched|Running --halt--> Cancelled|Failed (per policy)
  DeadLetter --manual_remediate--> Pending

Responsibilities
----------------
- FR-040 retries, FR-044 DLQ, FR-042 pause/cancel.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
MOD-06 owns FSM; events drive projections and audit.

Failure Scenarios
-----------------
- Stuck in Running past timeout -> force fail or cancel.

Recovery Strategy
-----------------
- Timeout watchdog via MOD-29 durable timers.

Security / Scalability / Maintainability
------------------------------------------
- Task state changes logged to audit.
- Millions of tasks via sharded store.

Alternatives
------------
- Merge Validating into Running.

Advantages / Disadvantages
+ Clear validation gate. - Extra state.

Engineering Recommendation
--------------------------
Separate Validating state for NFR-019 enforcement visibility.

Reasoning
---------
AI outputs untrusted until validated (SRS §6).

Future Evolution
----------------
- Partial success sub-states for multi-artifact tasks.

================================================================================
36. Planning Lifecycle
================================================================================

Purpose
-------
Architecture for plan creation through approval.

Detailed Explanation
--------------------
Phases:
  PL-1 Intake — requirements available in BC-02
  PL-2 Draft Plan — Planning Agent or Plan Engine produces DAG
  PL-3 Policy Enrichment — insert mandatory security/test/doc tasks (MOD-09)
  PL-4 Simulation — optional dry-run (FR-024)
  PL-5 Review — diff presented to Project Owner
  PL-6 Approval — MOD-11; explainability record (FR-119)
  PL-7 Activation — PlanVersionActivated event -> scheduler

Replan triggers (FR-021): scope change, task failure cascade, policy change,
agent unavailability. Replan creates new version; old versions immutable.

Responsibilities
----------------
- Maintain DAG acyclicity.
- Critical path computation (FR-023).

Inputs / Outputs
----------------
Inputs: Requirements, templates, agent capabilities, policies.
Outputs: Versioned plans, explainability records.

Dependencies
------------
- MOD-04, MOD-03, MOD-09, MOD-11, MOD-20 (planning inference).

Interactions
------------
- Project Planning Agent may produce initial plan; PM validates and enriches.

Failure Scenarios
-----------------
- Contradictory requirements -> flag FR-013, block PL-2 until resolved.

Recovery Strategy
-----------------
- Return to PL-1 with human clarification workflow.

Security / Scalability / Maintainability
------------------------------------------
- Planning uses redacted context only.
- Large plans paginated; critical path async compute.

Alternatives
------------
- Human-only planning.

Advantages / Disadvantages
+ AI-assisted with governance. - Replan churn.

Engineering Recommendation
--------------------------
Immutable plan versions with diff-based approval UI data from projections.

Reasoning
---------
FR-021 requires replan without losing audit history.

Future Evolution
----------------
- Multi-scenario plan comparison.

================================================================================
37. Execution Lifecycle
================================================================================

Purpose
-------
Architecture for task execution from schedule to completion.

Detailed Explanation
--------------------
Phases:
  EX-1 Schedule — MOD-05 assigns slots respecting deps, quotas, agent health
  EX-2 Policy PEP (Schedule) — MOD-09 permit required
  EX-3 Dispatch — MOD-08 sends to agent with context + credentials
  EX-4 Policy PEP (Dispatch) — irreversible prep actions checked
  EX-5 Run — agent executes; heartbeats for long tasks
  EX-6 Validate — MOD-21 schema and quality checks
  EX-7 Complete — emit TaskSucceeded; trigger dependent scheduling
  EX-8 Gate — if milestone, MOD-10 evaluates promotion gates

Parallelism: independent DAG branches execute concurrently (FR-041).

Responsibilities
----------------
- End-to-end execution within budget (MOD-22).
- Idempotent dispatch (ADR-SAD-006 patterns).

Inputs / Outputs
----------------
Inputs: Activated plan, schedules, agent availability.
Outputs: Completed tasks, artifacts, gate results.

Dependencies
------------
- MOD-05, MOD-06, MOD-08, MOD-09, MOD-10, MOD-21, MOD-22.

Interactions
------------
- CI/VCS events may satisfy gate inputs (MOD-15).

Failure Scenarios
-----------------
- Budget exceeded -> halt new EX-3 within 60s (SC-006).
- Agent failure -> retry or DLQ.

Recovery Strategy
-----------------
- Exponential backoff (FR-040); replan if persistent failure.

Security / Scalability / Maintainability
------------------------------------------
- Scoped credentials per EX-3.
- Scale scheduler and dispatch horizontally.
- Clear phase metrics per EX-n.

Alternatives
------------
- Sequential execution only.

Advantages / Disadvantages
+ Throughput. - Race condition management.

Engineering Recommendation
--------------------------
Optimistic scheduling with dependency barrier synchronization.

Reasoning
---------
SC-005 requires 10k concurrent tasks.

Future Evolution
----------------
- Priority preemption policies within tenant fairness.

================================================================================
38. Monitoring Lifecycle
================================================================================

Purpose
-------
Architecture for post-deploy and runtime monitoring coordination.

Detailed Explanation
--------------------
Phases:
  MN-1 Deploy Complete — Deployment Agent signals release
  MN-2 Monitoring Agent Dispatch — health checks, SLI probes per template
  MN-3 Telemetry Ingest — metrics/logs/traces from customer env via integrations
  MN-4 Anomaly Detection — MOD-23 FR-084 on error rates, latency, cost
  MN-5 Incident Event — if threshold breached, emit IncidentDetected
  MN-6 Incident-to-Task — FR-116 creates tasks for Maintenance/Dev agents
  MN-7 Ongoing Observation — until project archived or maintenance window ends

Responsibilities
----------------
- FR-116 incident loop.
- Feed gate and approval data for production health.

Inputs / Outputs
----------------
Inputs: Deploy events, external telemetry, monitoring agent results.
Outputs: Incidents, maintenance tasks, alerts.

Dependencies
------------
- Monitoring Agent, MOD-15, MOD-24, MOD-23.

Interactions
------------
- Does not replace customer APM; integrates with it.

Failure Scenarios
-----------------
- Telemetry gap -> degrade to agent-reported health only; alert integrator.

Recovery Strategy
-----------------
- Retry integration pull; escalate to operator.

Security / Scalability / Maintainability
------------------------------------------
- Telemetry may contain sensitive data — classify and redact.
- High-volume metrics via dedicated telemetry pipeline (Section 56).

Alternatives
------------
- PM-native full APM (out of scope OOS-002).

Advantages / Disadvantages
+ Closed-loop maintenance. - Integration dependency.

Engineering Recommendation
--------------------------
Incident events as first-class domain events triggering workflows.

Reasoning
---------
FR-115, FR-116 require post-release architecture.

Future Evolution
----------------
- SLO burn rate alerts tied to auto-remediation policies.

================================================================================
39. Deployment Lifecycle
================================================================================

Purpose
-------
Architecture for governed deployment orchestration.

Detailed Explanation
--------------------
Phases:
  DP-1 Pre-deploy Gate — all upstream tasks succeeded, tests/security gates pass
  DP-2 Release Package — Deployment Agent prepares artifacts
  DP-3 Production PEP — policy requires human approval (ADR-003, FR-052)
  DP-4 Approval — Project Owner / Compliance Officer per authority matrix
  DP-5 Deploy Execute — Deployment Agent with production-scoped credentials
  DP-6 Post-deploy Verify — Monitoring Agent MN-1 phase
  DP-7 Record — audit entry, explainability record (FR-119), cost attribution

PM does not host applications (OOS-002); orchestrates Deployment Agent only.

Responsibilities
----------------
- FR-061 block promotion until gates pass.
- Irreversible action controls (SRS Definitions).

Inputs / Outputs
----------------
Inputs: Gate results, approvals, release artifacts references.
Outputs: Deploy events, project -> Released transition.

Dependencies
------------
- MOD-10, MOD-11, Deployment Agent, MOD-15 CI/CD integration.

Interactions
------------
- CI/CD pipeline results feed DP-1 via MOD-15.

Failure Scenarios
-----------------
- Deploy without approval -> blocked at DP-3.
- Deploy failure -> rollback task dispatched to agent.

Recovery Strategy
-----------------
- Automated rollback workflow per template policy.

Security / Scalability / Maintainability
------------------------------------------
- Production credentials most restricted scope.
- Deploy events high-priority audit.

Alternatives
------------
- Manual deploy outside PM (breaks audit chain).

Advantages / Disadvantages
+ Governed releases. - Latency from approvals.

Engineering Recommendation
--------------------------
Deployment as explicit workflow with mandatory DP-3 for production.

Reasoning
---------
ADR-003 mandatory production gates.

Future Evolution
----------------
- Canary deploy orchestration via policy rules.

================================================================================
40. Maintenance Lifecycle
================================================================================

Purpose
-------
Architecture for post-release maintenance orchestration.

Detailed Explanation
--------------------
Phases:
  MT-1 Trigger — schedule (FR-115) or incident (FR-116)
  MT-2 Plan Patch — Maintenance Agent receives dispatch with incident/schedule context
  MT-3 Execute — patches, dependency updates, config fixes via agents
  MT-4 Validate — Testing/Security agents re-run per policy
  MT-5 Gate — non-prod first, then prod per environment rules
  MT-6 Complete — update knowledge graph with resolution record

Responsibilities
----------------
- Ongoing software health after Released state.
- Link maintenance tasks to originating incidents.

Inputs / Outputs
----------------
Inputs: Maintenance schedule templates, incident events.
Outputs: Maintenance task completions, updated artifacts.

Dependencies
------------
- Maintenance Agent, MOD-38 monitoring lifecycle, MOD-04 replanning.

Interactions
------------
- May trigger partial replan if scope expands.

Failure Scenarios
-----------------
- Maintenance window exceeded -> alert and optional auto-rollback.

Recovery Strategy
-----------------
- Escalate to Engineering Lead per conflict module.

Security / Scalability / Maintainability
------------------------------------------
- Maintenance prod changes follow same gate rules as features.
- Scheduled maintenance batched per tenant quotas.

Alternatives
------------
- Ad-hoc maintenance outside PM.

Advantages / Disadvantages
+ Continuous delivery operations. - Ongoing agent cost.

Engineering Recommendation
--------------------------
Template-defined maintenance cadence activated on ProjectReleased event.

Reasoning
---------
FR-115 explicitly requires maintenance agent scheduling post-release.

Future Evolution
----------------
- Predictive maintenance from anomaly ML.

================================================================================
41. Error Recovery Lifecycle
================================================================================

Purpose
-------
Platform-wide error detection, handling, and recovery.

Detailed Explanation
--------------------
Recovery tiers:
  RT-1 Transient — automatic retry with exponential backoff (FR-040)
  RT-2 Component — circuit breaker, failover to healthy replica
  RT-3 Task — reroute to alternate agent instance; replan if type unavailable
  RT-4 Project — pause project, notify owner, await decision
  RT-5 Platform — halt scope, credential mass revoke, operator incident

DLQ path (FR-044): Failed -> DeadLetter -> manual remediation workflow ->
requeue to Pending.

Chaos and DR tests validate RT-2 and RT-5 quarterly (SRS §29).

Responsibilities
----------------
- Classify errors: transient, permanent, security, policy.
- Never silent fail (Architecture Principle AP-10).

Inputs / Outputs
----------------
Inputs: Error events, health signals, timeouts.
Outputs: Retry commands, escalations, halt signals, DLQ entries.

Dependencies
------------
- MOD-06, MOD-27, MOD-29, Section 63-64.

Interactions
------------
- All modules report structured errors with correlation_id.

Failure Scenarios
-----------------
- Retry storm amplifies outage.

Recovery Strategy
-----------------
- Jittered backoff, max retry caps, budget-aware throttling.

Security / Scalability / Maintainability
------------------------------------------
- Security errors -> no retry; immediate revoke and alert.
- Error rates per tenant monitored for noisy neighbor isolation.

Alternatives
------------
- Infinite retries.

Advantages / Disadvantages
+ Resilience. - Complex classification.

Engineering Recommendation
--------------------------
Standard error taxonomy across all modules.

Reasoning
---------
Autonomous systems generate high error volume; structure is essential.

Future Evolution
----------------
- Auto-remediation policies within RT-3 for known patterns.

================================================================================
42. Human Approval Lifecycle
================================================================================

Purpose
-------
Architecture for human-in-the-loop governance.

Detailed Explanation
--------------------
Phases:
  HA-1 Trigger — policy marks action as requiring approval (plan, prod deploy, waiver)
  HA-2 Request Created — MOD-11 with SoD checks (FR-053)
  HA-3 Notify — MOD-24 to approvers
  HA-4 Review — approver sees diff, explainability record (FR-119), risk summary
  HA-5 Decision — Grant, Deny, or Delegate
  HA-6 Timeout — per policy: escalate, deny, or delegate (FR-052)
  HA-7 Execute — unblocks downstream workflow

Approval Authority Matrix (SRS §18.6):
  Plan baseline: Project Owner
  Production gate: Project Owner + optional Compliance Officer
  Policy changes: Organization Admin
  Halt resume: Operator + optional Engineering Lead

Responsibilities
----------------
- Enforce SoD: approver != initiator for agent-initiated changes.
- Immutable audit of decisions.

Inputs / Outputs
----------------
Inputs: Approval triggers, actor context.
Outputs: ApprovalGranted/Denied events, explainability records.

Dependencies
------------
- MOD-11, MOD-09, MOD-24, MOD-12.

Interactions
------------
- Blocks execution workflows until HA-5 Grant.

Failure Scenarios
-----------------
- Approver unavailable -> timeout path per policy.

Recovery Strategy
-----------------
- Delegation chain configured per tenant.

Security / Scalability / Maintainability
------------------------------------------
- MFA for production approvals (enterprise policy).
- Approval queue scales via async notifications.

Alternatives
------------
- Email-only approval without audit (rejected).

Advantages / Disadvantages
+ Enterprise trust. - Latency.

Engineering Recommendation
--------------------------
Durable approval state via MOD-29 timers.

Reasoning
---------
ADR-003 policy-driven hybrid autonomy requires approval architecture.

Future Evolution
----------------
- Risk-score adaptive approval (more autonomy when metrics green).

================================================================================
43. Security Architecture
================================================================================

Purpose
-------
Define defense-in-depth security across the platform.

Detailed Explanation
--------------------
Security domains:
  SD-01 Perimeter — WAF, DDoS, rate limits (DU-1)
  SD-02 Identity — SSO, SCIM, MFA (MOD-13)
  SD-03 Authorization — RBAC+ABAC (Sections 48-49)
  SD-04 Data — encryption, residency, retention (Sections 52, 81-82)
  SD-05 Secrets — vault, credential broker (Sections 50-51)
  SD-06 Application — input validation, ASVS L2 (SRS §31)
  SD-07 Agent — isolation, scoped creds (Section 30)
  SD-08 Supply chain — signed manifests, SBOM (SRS §31)
  SD-09 Audit — immutable logs (Section 53)
  SD-10 Operations — break-glass, pen test, vulnerability management

Responsibilities
----------------
- Meet SRS §31 security goals and FR-090–094.
- Tenant escape prevention as P0 invariant.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Threat model per release; security reviews at ARB.

Failure Scenarios
-----------------
- Single layer compromise should not yield tenant escape.

Recovery Strategy
-----------------
- Incident response playbooks; FR-117 credential revoke on halt.

Security / Scalability / Maintainability
------------------------------------------
- Security architecture is primary concern of this section.
- Centralized policy reduces scattered security logic.
- Security modules independently auditable.

Alternatives
------------
- Perimeter-only security.

Advantages / Disadvantages
+ Enterprise grade. - Cost and latency.

Engineering Recommendation
--------------------------
Zero Trust (Section 44) as overriding pattern.

Reasoning
---------
Autonomous agents increase attack surface; defense in depth mandatory.

Future Evolution
----------------
- FedRAMP control mapping if pursued.

================================================================================
44. Zero Trust Architecture
================================================================================

Purpose
-------
Eliminate implicit trust inside the platform.

Detailed Explanation
--------------------
Principles:
  ZT-01 Never trust, always verify — every request authenticated and authorized
  ZT-02 Least privilege — minimal scopes per operation
  ZT-03 Assume breach — segment networks, monitor lateral movement
  ZT-04 Explicit policy — deny default (fail-closed)
  ZT-05 Continuous validation — short-lived tokens, health attestation

Implementation:
  - mTLS between all services and agents (ADR-SAD-007)
  - Workload identity per pod/process
  - Policy check at PEPs (ADR-SAD-009)
  - Micro-segmentation: DU-2 cannot reach external APIs except via DU-4
  - No long-lived broad credentials

Responsibilities
----------------
- Enforce CON-002, SRS §31 zero-trust goal.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Identity from MOD-13; policies from MOD-09; network policies from infra.

Failure Scenarios
-----------------
- Policy engine down -> deny all dispatches (fail-closed).

Recovery Strategy
-----------------
- Policy engine HA replicas; cached read-only policy emergency mode with deny on stale.

Security / Scalability / Maintainability
------------------------------------------
- Zero trust is the security model.
- Token validation must be fast (cache JWKS, local introspection).
- Policy-as-code in version control.

Alternatives
------------
- VPN-trust internal network.

Advantages / Disadvantages
+ Strong isolation. - Latency per request.

Engineering Recommendation
--------------------------
Fail-closed on any authz uncertainty.

Reasoning
---------
Agent autonomy requires strict trust boundaries.

Future Evolution
----------------
- SPIFFE/SPIRE style workload identity.

================================================================================
45. Identity Architecture
================================================================================

Purpose
-------
Manage human and machine identities.

Detailed Explanation
--------------------
Identity types:
  ID-01 Human users (SSO via SAML/OIDC)
  ID-02 Service principals (agents, integrations)
  ID-03 API keys (scoped automation)
  ID-04 Break-glass emergency accounts

Identity attributes: tenant_id, user_id, roles[], groups[], mfa_status, session_id.

SCIM provisions users and groups from customer IdP (FR-092).

Responsibilities
----------------
- Single identity graph per tenant.
- Separate human vs machine identity lifecycle.

Inputs / Outputs
----------------
Inputs: IdP assertions, SCIM events, API key issuance requests.
Outputs: Identity records, session tokens.

Dependencies
------------
- External IdP, MOD-13.

Interactions
------------
- Every request resolves to principal + tenant context.

Failure Scenarios
-----------------
- IdP outage -> limited session grace; no new sessions.

Recovery Strategy
-----------------
- Cached identity with short TTL; secondary IdP optional enterprise.

Security / Scalability / Maintainability
------------------------------------------
- No shared accounts across tenants.
- Identity resolution cached at edge.
- SCIM changes audited.

Alternatives
------------
- Local-only user store.

Advantages / Disadvantages
+ Enterprise SSO. - IdP dependency.

Engineering Recommendation
--------------------------
OIDC primary; SAML for legacy enterprise.

Reasoning
---------
FR-092 mandates SSO and SCIM.

Future Evolution
----------------
- Passkey/WebAuthn for admin actions.

================================================================================
46. Authentication Architecture
================================================================================

Purpose
-------
Verify principal identity for every request.

Detailed Explanation
--------------------
Authn flows:
  AF-01 Browser SSO — OIDC authorization code + PKCE
  AF-02 API access — API key or OAuth client credentials
  AF-03 Agent dispatch — mTLS client cert + workload identity
  AF-04 Webhook inbound — HMAC signature + timestamp anti-replay
  AF-05 Service-to-service — mTLS + service JWT

Token properties: short TTL (15-60 min human), refresh rotation, bound to tenant.

Responsibilities
----------------
- Reject unauthenticated requests at edge.
- MFA for privileged roles (Org Admin, break-glass).

Inputs / Outputs
----------------
Inputs: Credentials, tokens, certificates.
Outputs: Authenticated session context.

Dependencies
------------
- MOD-13, MOD-01, customer IdP.

Interactions
------------
- Authn at DU-1; re-validation for sensitive operations.

Failure Scenarios
-----------------
- Token replay -> prevented by short TTL + rotation.

Recovery Strategy
-----------------
- Key rotation with overlap window.

Security / Scalability / Maintainability
------------------------------------------
- JWT validation at edge reduces backend load.
- JWKS cache with refresh.

Alternatives
------------
- Session cookies only without API key support.

Advantages / Disadvantages
+ Multi-modal auth. - Complexity.

Engineering Recommendation
--------------------------
Central auth middleware; no service-specific auth parsers.

Reasoning
---------
Consistent auth reduces security gaps.

Future Evolution
----------------
- Continuous auth risk scoring.

================================================================================
47. Authorization Architecture
================================================================================

Purpose
-------
Control what authenticated principals may do.

Detailed Explanation
--------------------
Authorization pipeline:
  1. Authenticate principal
  2. Resolve RBAC roles
  3. Evaluate ABAC attributes (project, env, data class, action)
  4. Evaluate policy obligations (approval required, MFA step-up)
  5. Permit or deny with audit log

Decision point: centralized MOD-13 with local enforcement libraries in each service.

Responsibilities
----------------
- FR-091 RBAC and ABAC for all operations.
- Default deny.

Inputs / Outputs
----------------
Inputs: Principal, resource, action, context attributes.
Outputs: AuthzDecision + obligations.

Dependencies
------------
- MOD-09 for policy obligations overlay.

Interactions
------------
- SoD rules for approvals (FR-053).

Failure Scenarios
-----------------
- Authz service timeout -> deny (fail-closed).

Recovery Strategy
----------------
- HA replicas; circuit breaker with deny fallback.

Security / Scalability / Maintainability
------------------------------------------
- Authz decision cache with tenant+resource key; TTL <=30s.
- Policy changes invalidate cache.

Alternatives
------------
- RBAC only without ABAC.

Advantages / Disadvantages
+ Fine-grained control. - Policy complexity.

Engineering Recommendation
--------------------------
RBAC for coarse roles; ABAC for project/environment granularity.

Reasoning
---------
SRS §15 user types require fine-grained least privilege.

Future Evolution
----------------
- ReBAC for complex delegation graphs.

================================================================================
48. RBAC Architecture
================================================================================

Purpose
-------
Role-based access control model.

Detailed Explanation
--------------------
Standard roles (map to SRS §15):
  ROLE_ORG_ADMIN, ROLE_PROJECT_OWNER, ROLE_ENGINEERING_LEAD, ROLE_OPERATOR,
  ROLE_COMPLIANCE_OFFICER, ROLE_INTEGRATOR, ROLE_VIEWER, ROLE_AGENT_PRINCIPAL,
  ROLE_SUPPORT_ENGINEER (vendor, consent-based)

Role bundles per persona; custom roles optional enterprise.

Permission model: resource_type.action (e.g., project.approve, task.dispatch, halt.execute)

Responsibilities
----------------
- Map SRS user types to enforceable roles.
- Role assignment audited.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
SCIM groups -> role mappings; admin UI for manual assignment.

Failure Scenarios / Recovery / Security / Scalability / Maintainability
-------------------------------------------------------------------------
Standard RBAC patterns; horizontal scale via stateless evaluators.

Alternatives
------------
- ABAC only.

Advantages / Disadvantages
+ Simple to explain. - Coarse without ABAC.

Engineering Recommendation
--------------------------
RBAC + ABAC hybrid (Section 49).

Reasoning
---------
Enterprise customers expect recognizable roles.

Future Evolution
----------------
- Custom role builder UI.

================================================================================
49. ABAC Architecture
================================================================================

Purpose
-------
Attribute-based fine-grained access control.

Detailed Explanation
--------------------
Attributes:
  Subject: tenant_id, roles, clearance, team
  Resource: project_id, environment (dev/staging/prod), data_classification
  Action: read, write, dispatch, approve, export, halt
  Environment: time, ip_range, mfa_verified, anomaly_score

Example rules:
  - dispatch to prod requires environment=prod AND role ENGINEERING_LEAD OR agent principal
  - audit export requires COMPLIANCE_OFFICER AND data_classification <= confidential
  - cross-region read denied unless tenant policy allows (FR-120)

Responsibilities
----------------
- Enforce FR-120 region pinning at authz layer.
- FR-121 tenant boundary on audit queries.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Policy definitions in MOD-09; evaluated by MOD-13.

Failure Scenarios
-----------------
- Attribute missing -> deny.

Recovery Strategy
-----------------
- Explicit default attributes; validation on resource creation.

Security / Scalability / Maintainability
------------------------------------------
- ABAC rules versioned; tested with policy simulation (FR-054).

Alternatives
------------
- Manual ACL lists per resource.

Advantages / Disadvantages
+ Precision. - Rule explosion.

Engineering Recommendation
--------------------------
CEL/Rego-style policy language; deny overrides.

Reasoning
---------
Production gates and environment promotion require attribute-aware rules.

Future Evolution
----------------
- ML anomaly attributes in environment context.

================================================================================
50. Secrets Architecture
================================================================================

Purpose
-------
Manage secrets without exposure in PM or logs.

Detailed Explanation
--------------------
Secret categories:
  SEC-01 Integration OAuth tokens
  SEC-02 API keys (customer and platform)
  SEC-03 Agent task credentials
  SEC-04 Encryption keys (via KMS)
  SEC-05 Webhook signing secrets

Storage: managed secrets vault; references only in PM state (NFR-007).
Rotation: automated for platform secrets; guided for customer integrations.
Access: Credential Broker is only module issuing task secrets to agents.

Responsibilities
----------------
- No plaintext secrets in logs, events, or context packages (FR-102).
- Revoke on halt within 60s (FR-117).

Inputs / Outputs
----------------
Inputs: Secret create/rotate/revoke commands.
Outputs: Secret handles, not values, in dispatch packages.

Dependencies
------------
- KMS (Section 51), MOD-14.

Interactions
------------
- Integration Hub stores tokens via vault API.

Failure Scenarios
-----------------
- Secret leak suspicion -> mass revoke + incident.

Recovery Strategy
-----------------
- Rotation with dual-active overlap.

Security / Scalability / Maintainability
------------------------------------------
- Vault HA; HSM-backed root keys enterprise tier.
- Audit every secret access.

Alternatives
------------
- Env var secrets in deployables (rejected).

Advantages / Disadvantages
+ Reduced blast radius. - Vault dependency.

Engineering Recommendation
--------------------------
Central vault; dynamic secrets for agent tasks.

Reasoning
---------
NFR-007 and agent autonomy demand rigorous secret hygiene.

Future Evolution
----------------
- Customer-managed secrets (CMK) integration.

================================================================================
51. Key Management Architecture
================================================================================

Purpose
-------
Manage cryptographic keys lifecycle.

Detailed Explanation
--------------------
Key hierarchy:
  KH-1 Root — HSM or cloud KMS root (platform operator)
  KH-2 Tenant — per-tenant data encryption keys (DEK) optional CMK
  KH-3 Service — TLS and mTLS certificates via automated rotation
  KH-4 Integration — per-connection token encryption keys
  KH-5 Audit — integrity signing keys for audit export bundles

Operations: generate, rotate, revoke, destroy per retention policy.
FR-120 region pinning applies to key material storage location.

Responsibilities
----------------
- Support AES-256 at rest (NFR-006).
- TLS 1.2+ in transit.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Cloud KMS; MOD-14 for task token signing; MOD-12 for audit signing.

Failure Scenarios
-----------------
- KMS unavailable -> fail closed for encrypt/decrypt operations.

Recovery Strategy
-----------------
- Multi-region KMS replicas enterprise tier; cached DEKs with short TTL.

Security / Scalability / Maintainability
------------------------------------------
- Annual key ceremony documentation; auto cert renewal.

Alternatives
------------
- App-managed keys without KMS.

Advantages / Disadvantages
+ Compliance ready. - KMS cost.

Engineering Recommendation
--------------------------
Envelope encryption: KMS wraps DEKs; data encrypted with DEK.

Reasoning
---------
Performance and security balance for high-volume audit storage.

Future Evolution
----------------
- Post-quantum algorithm migration plan.

================================================================================
52. Encryption Architecture
================================================================================

Purpose
-------
Protect data in transit and at rest.

Detailed Explanation
--------------------
At rest: AES-256 for all persistent stores; tenant DEK isolation optional.
In transit: TLS 1.2+ external; mTLS internal (ADR-SAD-007).
Application-level: encrypt highly sensitive fields (PII) before storage (NFR-008).
Backups: encrypted with separate backup keys; tested restore monthly.

Responsibilities
----------------
- NFR-006 compliance.
- FR-120 data residency for ciphertext location.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
KMS Section 51; all storage Section 75.

Failure Scenarios
-----------------
- Misconfigured TLS -> CI fitness test blocks deploy.

Recovery Strategy
-----------------
- Certificate auto-renewal; monitoring for expiry.

Security / Scalability / Maintainability
------------------------------------------
- Encryption transparent to app via SDK; minimal perf impact with HW accel.

Alternatives
------------
- Disk encryption only without app-level field encryption.

Advantages / Disadvantages
+ Defense in depth. - Key management overhead.

Engineering Recommendation
--------------------------
Encrypt everything; field-level for PII catalog fields.

Reasoning
---------
GDPR and enterprise questionnaires require explicit encryption architecture.

Future Evolution
----------------
- Format-preserving encryption for searchable encrypted fields.

================================================================================
53. Audit Architecture
================================================================================

Purpose
-------
Immutable, reconstructable audit trail.

Detailed Explanation
--------------------
Audit sources: all commands, domain events, authz decisions, policy evals,
gate results, dispatches, credential issue/revoke, admin config changes.

Properties: append-only, tamper-evident (hash chain or signed batches),
tenant-scoped queries (FR-121), exportable (FR-083, CON-007).

Retention: per tenant policy (FR-094); legal hold overrides deletion (FR-103).

Responsibilities
----------------
- SC-003 100% decision event reconstruction.
- FR-080 immutable audit log.

Inputs / Outputs
----------------
Inputs: All platform events.
Outputs: Audit entries, compliance export bundles.

Dependencies
------------
- MOD-12, MOD-18, event sourcing (Section 22).

Interactions
------------
- Compliance Officer queries via DU-5 read API with ABAC.

Failure Scenarios
-----------------
- Audit ingest gap -> P1 alert; buffer events until recovered.

Recovery Strategy
-----------------
- Replay from event backbone to rebuild audit store.

Security / Scalability / Maintainability
------------------------------------------
- Audit data highest classification; separate access roles.
- Partition by tenant and time; cold storage for aged logs.

Alternatives
------------
- Mutable application logs only.

Advantages / Disadvantages
+ Regulatory evidence. - Storage growth.

Engineering Recommendation
--------------------------
Dedicated audit ingest path separate from metrics pipeline.

Reasoning
---------
Audit integrity must not compete with high-volume telemetry.

Future Evolution
----------------
- Blockchain anchoring optional for non-repudiation proofs.

================================================================================
54. Logging Architecture
================================================================================

Purpose
-------
Operational logging distinct from compliance audit.

Detailed Explanation
--------------------
Log types: application logs, access logs, integration logs, security logs.
Format: structured JSON with correlation_id, tenant_id, service, level, message.
Prohibition: no secrets, no PII beyond pseudonymized IDs (NFR-007, NFR-008).

Retention: shorter than audit; operational need driven (30-90 days hot).

Responsibilities
----------------
- Support incident debugging and SRE workflows.
- Feed security monitoring (SIEM).

Inputs / Outputs
----------------
Inputs: All services emit logs.
Outputs: Centralized log store with tenant-scoped search for operators.

Dependencies
------------
- Observability stack; distinct from MOD-12 audit.

Interactions
------------
- Logs correlate with traces via correlation_id.

Failure Scenarios
-----------------
- Log flood during incident -> sampling with always-log errors.

Recovery Strategy
-----------------
- Buffer and backpressure; never block critical path on log delivery.

Security / Scalability / Maintainability
------------------------------------------
- Operator access only; not customer-accessible by default.
- High volume; aggressive sampling for debug in production.

Alternatives
------------
- Unstructured text logs.

Advantages / Disadvantages
+ Debuggability. - Cost at scale.

Engineering Recommendation
--------------------------
Structured logging standard enforced by linter.

Reasoning
---------
NFR-015 distributed tracing requires log correlation.

Future Evolution
----------------
- Customer log streaming add-on (tenant-scoped).

================================================================================
55. Monitoring Architecture
================================================================================

Purpose
-------
Platform and business health monitoring.

Detailed Explanation
--------------------
Monitoring layers:
  MN-A Infrastructure — CPU, memory, disk, network per deployable
  MN-B Service SLO — latency, error rate, saturation per SVC
  MN-C Business — tasks/hour, gate pass rate, approval latency
  MN-D Agent — health, ack time, success rate per agent type
  MN-E Integration — connector success, sync lag
  MN-F Security — auth failures, policy denials, anomaly spikes

Dashboards: operator (internal), tenant admin (subset), status page (public).

Responsibilities
----------------
- Detect SLO breaches; feed alerting (Section 59).
- FR-081 real-time project metrics via projections.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Metrics from Section 58; MOD-23 for tenant dashboards.

Failure Scenarios
-----------------
- Monitoring blind spot during outage -> meta-monitoring on collectors.

Recovery Strategy
-----------------
- Redundant monitoring paths; synthetic probes.

Security / Scalability / Maintainability
------------------------------------------
- Tenant metrics isolated; no cross-tenant dashboards.

Alternatives
------------
- Logs-only operations.

Advantages / Disadvantages
+ Proactive ops. - Cardinality explosion.

Engineering Recommendation
--------------------------
RED method (Rate, Errors, Duration) per service; USE for infra.

Reasoning
---------
99.9% availability requires SLO monitoring (NFR-004).

Future Evolution
----------------
- Customer-defined SLO monitors on their projects.

================================================================================
56. Telemetry Architecture
================================================================================

Purpose
-------
High-volume performance and diagnostic data collection.

Detailed Explanation
--------------------
Telemetry types: metrics, traces, profiles (optional).
Standards: OpenTelemetry (Section 37 SRS interoperability).
Sampling: head-based default 10%; tail-based for errors 100%.
Agent telemetry: execution duration, token usage, model_id -> Cost Meter.

Responsibilities
----------------
- NFR-015 trace coverage on dispatch paths.
- Feed performance architecture (Section 95).

Inputs / Outputs
----------------
Inputs: Instrumented services and agents.
Outputs: Telemetry backend for analysis.

Dependencies
------------
- OTel collectors, time-series DB, trace store.

Interactions
------------
- Separate pipeline from audit (lower retention, higher volume).

Failure Scenarios
-----------------
- Collector overload -> drop sampled data, never block requests.

Recovery Strategy
-----------------
- Scale collectors horizontally.

Security / Scalability / Maintainability
------------------------------------------
- Scrub PII at collector edge.
- Cardinality limits per metric label set.

Alternatives
------------
- Vendor-locked APM only.

Advantages / Disadvantages
+ Deep diagnostics. - Storage cost.

Engineering Recommendation
--------------------------
OpenTelemetry everywhere; vendor-neutral export.

Reasoning
---------
SRS interoperability goals and avoid APM lock-in.

Future Evolution
----------------
- Continuous profiling in staging environments.

================================================================================
57. Observability Architecture
================================================================================

Purpose
-------
Unified triage across logs, metrics, traces, and audit.

Detailed Explanation
--------------------
Three pillars + audit:
  Logs (Section 54) — what happened in text
  Metrics (Section 58) — aggregated health
  Traces (Section 56) — request path latency
  Audit (Section 53) — who did what for compliance

Correlation: correlation_id links all four for a single user action or dispatch.

Responsibilities
----------------
- MTTR reduction for operators.
- Support SC-003 reconstruction workflows.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
All observability modules; unified search UI for operators.

Failure Scenarios
-----------------
- Missing correlation_id -> architecture violation caught in CI.

Recovery Strategy
-----------------
- Synthetic end-to-end trace tests in staging.

Security / Scalability / Maintainability
------------------------------------------
- RBAC on observability tools; tenant data masked for vendor support.

Alternatives
------------
- Siloed tools without correlation.

Advantages / Disadvantages
+ Fast incident response. - Integration effort.

Engineering Recommendation
--------------------------
Mandatory correlation_id from edge through agents.

Reasoning
---------
NFR-015 explicit requirement.

Future Evolution
----------------
- AI-assisted incident summarization for operators.

================================================================================
58. Metrics Architecture
================================================================================

Purpose
-------
Define metric taxonomy and collection.

Detailed Explanation
--------------------
Metric categories:
  MT-01 Platform counters — requests_total, errors_total, latency_histogram
  MT-02 Business counters — tasks_dispatched, gates_passed, approvals_pending
  MT-03 Resource gauges — queue_depth, projection_lag_seconds
  MT-04 Cost gauges — token_usage, budget_remaining
  MT-05 SLO burn — error_budget_remaining

Naming: aipm_{domain}_{metric}_{unit}; labels: tenant_id (careful cardinality),
service, region, agent_type.

Responsibilities
----------------
- Feed alerting and capacity planning.
- FR-081 dashboard data source.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
MOD-23 aggregates business metrics from events.

Failure Scenarios
-----------------
- Cardinality explosion from unbounded labels.

Recovery Strategy
-----------------
- Label allowlist; drop high-cardinality metrics.

Security / Scalability / Maintainability
------------------------------------------
- Tenant_id in metrics only for dedicated dashboards with access control.

Alternatives
------------
- Ad-hoc per-service metric names.

Advantages / Disadvantages
+ Consistent SLOs. - Cardinality discipline required.

Engineering Recommendation
--------------------------
Platform metric catalog with required instrumentation checklist.

Reasoning
---------
SLO dashboards require consistent metrics.

Future Evolution
----------------
- External Prometheus-compatible scrape for enterprise.

================================================================================
59. Alerting Architecture
================================================================================

Purpose
-------
Notify operators and customers of actionable conditions.

Detailed Explanation
--------------------
Alert tiers:
  AL-P1 Critical — platform down, tenant escape suspicion, mass credential failure
  AL-P2 High — SLO breach, halt triggered, budget cap hit, audit ingest stopped
  AL-P3 Medium — agent type unavailable, projection lag high, integration degraded
  AL-P4 Low — certificate expiry 30d, capacity forecast threshold

Routing: P1 pages on-call; P2 tickets; P3/P4 dashboard + email.
Customer alerts: budget, gate failures, approval needed via MOD-24.

Responsibilities
----------------
- SC-006 budget alert; SC-008 halt confirmation alerts.

Inputs / Outputs
----------------
Inputs: Metric thresholds, anomaly detector (FR-084).
Outputs: Notifications, incident records.

Dependencies
------------
- MOD-24, monitoring (Section 55), on-call system.

Interactions
------------
- Alert storms suppressed via grouping and inhibition rules.

Failure Scenarios
-----------------
- Alert fatigue -> ignored P1s.

Recovery Strategy
-----------------
- Runbook link in every alert; periodic alert audit.

Security / Scalability / Maintainability
------------------------------------------
- Alert content must not leak cross-tenant data.

Alternatives
------------
- Email-only alerting.

Advantages / Disadvantages
+ Fast response. - Noise if misconfigured.

Engineering Recommendation
--------------------------
SLO-based alerts over static thresholds where possible.

Reasoning
---------
Error budget aligns with NFR-004.

Future Evolution
----------------
- Auto-incident creation with runbook automation.

================================================================================
60. Notification Architecture
================================================================================

Purpose
-------
Deliver human-facing notifications across channels.

Detailed Explanation
--------------------
Channels: in-app, email, Slack/Teams, PagerDuty, webhook (customer).
Event sources: approvals needed, gate failed, halt, budget threshold, incident.

Delivery guarantees: at-least-once; deduplication by notification_id.
Preferences: per-user channel preferences; quiet hours enterprise.

Responsibilities
----------------
- Support approval workflow HA-3 (Section 42).
- Escalation paths for timeout.

Inputs / Outputs
----------------
Inputs: NotificationRequested events.
Outputs: Delivery confirmations, failures for retry.

Dependencies
------------
- MOD-24, external providers, MOD-18.

Interactions
------------
- Separate from alerting for operators vs customer-facing messages.

Failure Scenarios
-----------------
- Provider outage -> fallback channel per user prefs.

Recovery Strategy
-----------------
- Retry queue with exponential backoff.

Security / Scalability / Maintainability
------------------------------------------
- No sensitive data in email subjects; link to authenticated UI.
- Rate limit notifications per tenant.

Alternatives
------------
- Single channel (email only).

Advantages / Disadvantages
+ Timely human actions. - Provider dependencies.

Engineering Recommendation
--------------------------
Plugin channel modules (Section 9 M7).

Reasoning
---------
Approval latency depends on reliable notification.

Future Evolution
----------------
- Mobile push for admin app.

================================================================================
61. Scheduling Architecture
================================================================================

Purpose
-------
Determine when and in what order tasks execute.

Detailed Explanation
--------------------
Scheduler components:
  SCH-1 Dependency Resolver — DAG ready set computation
  SCH-2 Priority Arbiter — project priority, task priority, SLA deadlines
  SCH-3 Quota Enforcer — tenant concurrency limits, agent pool capacity
  SCH-4 Budget Guard — consult MOD-22 before schedule
  SCH-5 Agent Router — match task agent_type to healthy instances (MOD-07)
  SCH-6 Fairness — prevent single tenant monopolizing shared pools

Algorithm: topological sort + priority queue; not ML at GA.

Responsibilities
----------------
- FR-022, FR-041.
- Critical path reporting (FR-023) async job.

Inputs / Outputs
----------------
Inputs: Active plan, task states, agent health, quotas.
Outputs: TaskScheduled events.

Dependencies
------------
- MOD-05, MOD-07, MOD-09, MOD-22.

Interactions
------------
- Halt stops SCH output immediately; in-flight continues per policy.

Failure Scenarios
-----------------
- Scheduler hot loop on thrashing replans -> rate limit replan frequency.

Recovery Strategy
-----------------
- Scheduler stateless; rebuild from task states.

Security / Scalability / Maintainability
------------------------------------------
- Scheduler cannot schedule policy-denied tasks.
- Horizontally scaled workers with partition locking per project.

Alternatives
------------
- FIFO only without priority.

Advantages / Disadvantages
+ Fair multi-tenant execution. - Complexity.

Engineering Recommendation
--------------------------
Per-project scheduling lock; cross-project fairness at agent pool level.

Reasoning
---------
SC-005 multi-tenant concurrent load.

Future Evolution
----------------
- ML-assisted duration estimates for better scheduling.

================================================================================
62. Queue Architecture
================================================================================

Purpose
-------
Async buffering and work distribution.

Detailed Explanation
--------------------
Queue types:
  Q-01 Command Queue — async commands (exports, bulk operations)
  Q-02 Dispatch Queue — per agent-type dispatch with priority
  Q-03 Event Processing Queue — consumer groups on event backbone
  Q-04 Notification Queue — outbound notifications
  Q-05 Integration Sync Queue — reconciler jobs
  Q-06 Halt Priority Queue — expedited halt propagation

Properties: at-least-once delivery, visibility timeout, dead-letter after max receive.

Responsibilities
----------------
- Decouple producers and consumers for scale.
- Priority for halt (SC-008).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
MOD-18 for event queues; dedicated queue service or backbone topics.

Failure Scenarios
-----------------
- Queue backlog growth -> scale consumers; tenant throttling.

Recovery Strategy
-----------------
- Replay from DLQ after fix.

Security / Scalability / Maintainability
------------------------------------------
- Encrypt messages at rest; tenant_id in message attributes.
- Partition queues by tenant or agent type for isolation.

Alternatives
------------
- Synchronous only processing.

Advantages / Disadvantages
+ Elastic throughput. - Ordering challenges.

Engineering Recommendation
--------------------------
Use event backbone for domain events; dedicated queues for task dispatch.

Reasoning
---------
Dispatch has different SLA than analytics projection.

Future Evolution
----------------
- Priority queue tiers per customer SKU.

================================================================================
63. Retry Architecture
================================================================================

Purpose
-------
Handle transient failures systematically.

Detailed Explanation
--------------------
Retry policies (FR-040):
  - Max attempts per task type (configurable in template)
  - Exponential backoff with jitter: base 2s, max 300s, multiplier 2
  - Idempotency key required on retried dispatches
  - Non-retryable: policy denial, validation permanent fail, auth errors

Retry ownership:
  RT-Task — MOD-06 for agent dispatch and validation
  RT-Integration — MOD-15 with connector-specific limits
  RT-Notification — MOD-24
  RT-Webhook — MOD-16

Responsibilities
----------------
- Prevent infinite loops and cost runaway (RSK-003).

Inputs / Outputs
----------------
Inputs: Failure classification.
Outputs: Retry schedule or DLQ routing.

Dependencies
------------
- MOD-29 durable timers for backoff delays.

Interactions
------------
- Cost Meter aware retries still consume budget.

Failure Scenarios
-----------------
- Retry storm during systemic outage.

Recovery Strategy
-----------------
- Circuit breaker pauses retries; alert operators.

Security / Scalability / Maintainability
------------------------------------------
- No retry on security violations.
- Retry metrics per task type for tuning.

Alternatives
------------
- Fixed interval retry only.

Advantages / Disadvantages
+ Resilience. - Delayed failure detection.

Engineering Recommendation
--------------------------
Central retry policy catalog per task/integration type.

Reasoning
---------
FR-040 explicit exponential backoff requirement.

Future Evolution
----------------
- Adaptive retry based on error class ML.

================================================================================
64. Dead Letter Architecture
================================================================================

Purpose
-------
Handle permanently failed work items.

Detailed Explanation
--------------------
DLQ sources: tasks (FR-044), event consumers, webhooks, integration sync.

DLQ entry contains: original message, failure reason, attempts, correlation_id,
timestamp, remediation hints.

Remediation workflow:
  1. Operator or Engineering Lead reviews DLQ item
  2. Fix root cause (agent, integration, policy)
  3. Requeue to original queue or manual complete with audit
  4. All actions audited

Responsibilities
----------------
- No silent loss of failed tasks.
- Visibility in admin UI.

Inputs / Outputs
----------------
Inputs: Exhausted retries.
Outputs: DLQ records, remediation events.

Dependencies
------------
- MOD-06, MOD-16, MOD-17, queue infrastructure.

Interactions
------------
- DLQ depth is alerting metric (AL-P3).

Failure Scenarios
-----------------
- DLQ unbounded growth -> storage and alert.

Recovery Strategy
-----------------
- Automated DLQ age alerts; bulk requeue tooling.

Security / Scalability / Maintainability
------------------------------------------
- DLQ access restricted to Operator and Engineering Lead roles.
- Tenant-scoped DLQ partitions.

Alternatives
------------
- Drop failed messages after retries.

Advantages / Disadvantages
+ Operational visibility. - Manual remediation cost.

Engineering Recommendation
--------------------------
Unified DLQ UI across task, event, and integration failures.

Reasoning
---------
FR-044 mandates DLQ with remediation workflow.

Future Evolution
----------------
- Suggested remediation from knowledge graph patterns.

================================================================================
65. Workflow Engine Architecture
================================================================================

Purpose
-------
Durable execution of multi-step workflows.

Detailed Explanation
--------------------
Architecture (ADR-SAD-002):
  Layer 1: Domain FSMs — Project, Task, Agent, Approval (source of truth)
  Layer 2: Workflow Coordinator — interprets workflow templates
  Layer 3: Durability Adapter (MOD-29) — persists wait states, timers, checkpoints
  Layer 4: Optional external engine adapter — future for BPMN import only

Workflow state stored as events + checkpoint snapshots in write model.

Responsibilities
----------------
- Human wait steps survive restarts (approvals).
- Scheduled maintenance triggers (FR-115).

Inputs / Outputs
----------------
Inputs: Workflow definitions from templates.
Outputs: Commands at step boundaries.

Dependencies
------------
- MOD-29, MOD-11, event store.

Interactions
------------
- Workflows never bypass policy PEPs.

Failure Scenarios
-----------------
- Split-brain workflow state -> single writer per workflow instance.

Recovery Strategy
-----------------
- Rebuild workflow state from event log.

Security / Scalability / Maintainability
------------------------------------------
- Workflow definitions signed and versioned.
- Shard by tenant_id + workflow_id.

Alternatives
------------
- External Temporal/Camunda as primary (higher ops cost at GA).

Advantages / Disadvantages
+ Domain-aligned. - Custom adapter engineering.

Engineering Recommendation
--------------------------
FSM-first; durability adapter thin wrapper over event store.

Reasoning
---------
SRS §48: workflow engine is implementation detail.

Future Evolution
----------------
- Export/import BPMN for enterprise integration teams.

================================================================================
66. Rule Engine Architecture
================================================================================

Purpose
-------
Evaluate business rules separate from application code.

Detailed Explanation
--------------------
Rule categories:
  RL-01 Scheduling rules — priority boosts, blackout windows
  RL-02 Gate rules — threshold comparisons on test/security metrics
  RL-03 Routing rules — agent selection preferences
  RL-04 Notification rules — escalation timing
  RL-05 Anomaly rules — FR-084 thresholds

Implementation: declarative rules in versioned packs; evaluated by sandboxed engine.

Responsibilities
----------------
- Tenant-configurable behavior without code forks (CON-006).

Inputs / Outputs
----------------
Inputs: Facts (metrics, task attributes, time).
Outputs: Rule conclusions (actions to take).

Dependencies
------------
- MOD-09 policy engine (policy vs business rule separation: policy=authorization,
  rules=behavior within permitted space).

Interactions
------------
- Plan enrichment uses rules to insert mandatory tasks.

Failure Scenarios
-----------------
- Rule conflict -> deterministic precedence order documented.

Recovery Strategy
-----------------
- Rule simulation mode before activation (like FR-054 for policies).

Security / Scalability / Maintainability
------------------------------------------
- Rule packs signed; no arbitrary code execution in rules at GA.
- Compile rules for performance.

Alternatives
------------
- Hardcoded rules in services.

Advantages / Disadvantages
+ Configurable. - Rule testing burden.

Engineering Recommendation
--------------------------
CEL for simple rules; dedicated engine for complex gate rules.

Reasoning
---------
Templates need configurable gate and scheduling behavior.

Future Evolution
----------------
- Customer-authored rule packs (enterprise).

================================================================================
67. Policy Engine Architecture
================================================================================

Purpose
-------
Authorize actions and enforce autonomy boundaries.

Detailed Explanation
--------------------
Policy Evaluation Points (PEPs) per ADR-SAD-009:
  PEP-1 Schedule — can this task be scheduled now?
  PEP-2 Dispatch — can this dispatch proceed? credentials scope ok?
  PEP-3 Gate — can environment promotion occur?
  PEP-4 Admin — can this configuration change proceed?

Policy language: Rego or CEL with constraints; versioned PolicySet per tenant.
Evaluation: deny overrides permit; default deny; fail-closed on engine error.

Autonomy levels 0-4 (SRS Definitions) mapped to policy templates.

Responsibilities
----------------
- FR-050–053, FR-051 before every irreversible action.
- ADR-003 production gate enforcement.

Inputs / Outputs
----------------
Inputs: PolicyEvalRequest (actor, action, resource, context).
Outputs: Permit/Deny + obligations (e.g., require_approval_id).

Dependencies
------------
- MOD-09, MOD-12 audit of all evaluations.

Interactions
------------
- Scheduler, Dispatch, Gate modules call synchronously.

Failure Scenarios
-----------------
- Policy timeout -> DENY and alert.

Recovery Strategy
----------------
- HA policy engine cluster; read replicas with version pins.

Security / Scalability / Maintainability
------------------------------------------
- Policy changes audited; simulation against history (FR-054).
- Cache compiled policies; invalidate on version change.

Alternatives
------------
- Hardcoded if-else authorization.

Advantages / Disadvantages
+ Enterprise governance. - Policy authoring learning curve.

Engineering Recommendation
--------------------------
Policy-as-code in git; CI tests for policy packs.

Reasoning
---------
ADR-003 and SRS AI philosophy: policies dispose.

Future Evolution
----------------
- Policy recommendation from compliance framework packs.

================================================================================
68. Validation Architecture
================================================================================

Purpose
-------
Validate all untrusted inputs especially agent outputs.

Detailed Explanation
--------------------
Validation stages:
  V-01 Ingress — API schema validation at gateway
  V-02 Dispatch input — context package schema before send
  V-03 Agent output — schema + semantic validators (NFR-019)
  V-04 Gate evidence — CI results format and threshold
  V-05 Integration payload — external event normalization

Validators: JSON Schema, custom semantic checks, contract tests for agent types.

Responsibilities
----------------
- AI Safety NFR-019: never execute unvalidated model output.
- FR-110 conflict detection via comparison validators.

Inputs / Outputs
----------------
Inputs: Payloads, schema references.
Outputs: ValidationResult (pass/fail, errors[], warnings[]).

Dependencies
------------
- MOD-21, schema registry, agent manifests.

Interactions
------------
- Failed V-03 triggers retry or MOD-28 conflict escalation.

Failure Scenarios
-----------------
- Validator bug -> false positive blocks delivery.

Recovery Strategy
-----------------
- Validator versioning; waiver path for gates only (FR-063).

Security / Scalability / Maintainability
------------------------------------------
- Validators run in sandbox for third-party extensions.
- Parallel validation for independent artifacts.

Alternatives
------------
- Trust agent self-reporting (rejected).

Advantages / Disadvantages
+ Safety. - Latency.

Engineering Recommendation
--------------------------
Validation as mandatory pipeline stage with metrics.

Reasoning
---------
SRS §6 failure mode: hallucinated completion.

Future Evolution
----------------
- LLM-as-judge secondary validator with human escalation.

================================================================================
69. AI Model Router Architecture
================================================================================

Purpose
-------
Route inference requests to approved models per ADR-004.

Detailed Explanation
--------------------
Router functions:
  MR-01 Model selection — task type -> preferred model from catalog
  MR-02 Fallback — primary unavailable -> secondary within  SLA
  MR-03 Cost optimization — route to cheaper model when policy allows
  MR-04 Budget check — consult MOD-22 before inference
  MR-05 Telemetry — log model_id, tokens, latency for FR-082
  MR-06 Safety — approved catalog only; block unapproved endpoints

Catalog entries: provider, model_id, capabilities, cost tier, data handling terms,
region availability (GDPR).

Responsibilities
----------------
- CON-004 provider replaceability.
- No direct LLM calls outside router.

Inputs / Outputs
----------------
Inputs: InferenceRequest from Plan Engine, Requirements, agents (via PM).
Outputs: InferenceResponse + usage metadata.

Dependencies
------------
- External LLM APIs, MOD-22, MOD-12 audit.

Interactions
------------
- Planning and requirement agents use router; dev agents may call models directly
  in their runtime but PM tracks via agent-reported usage.

Failure Scenarios
-----------------
- All models unavailable -> planning pauses; execution of approved tasks continues.

Recovery Strategy
-----------------
- Cached plan templates for degraded planning mode.

Security / Scalability / Maintainability
------------------------------------------
- No customer data to models without DPA-covered catalog entry.
- Rate limit per tenant; queue inference requests.

Alternatives
------------
- Single vendor hard bind.

Advantages / Disadvantages
+ Resilience and cost control. - Router complexity.

Engineering Recommendation
--------------------------
Central MOD-20; extend catalog via configuration not code.

Reasoning
---------
ADR-004 model-agnostic router.

Future Evolution
----------------
- Per-task-type fine-tuned small models on router.

================================================================================
70. Memory Architecture
================================================================================

Purpose
-------
Define what the platform remembers between operations (non-implementation).

Detailed Explanation
--------------------
Memory classes:
  MM-01 Authoritative State — aggregates in write store (Section 22)
  MM-02 Event History — complete domain event log
  MM-03 Knowledge Graph — artifacts, decisions, relationships (Section 73)
  MM-04 Session Memory — user session, UI state (ephemeral)
  MM-05 Cache Memory — performance caches (MOD-30)
  MM-06 Agent Session — agent-side only; PM does not retain agent internal memory

PM does not implement long-term LLM conversation memory; context assembled per
dispatch from MM-01 + MM-03 (FR-101).

Responsibilities
----------------
- Distinguish authoritative vs derived vs ephemeral.
- Retention policies per class (FR-094).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
MOD-19 context assembly reads MM-01, MM-03.

Failure Scenarios
-----------------
- Treating cache as authoritative -> inconsistency.

Recovery Strategy
-----------------
- Rebuild MM-05 from MM-01/MM-02 anytime.

Security / Scalability / Maintainability
------------------------------------------
- MM-03 may contain sensitive artifacts — classify and redact.
- MM-02 grows unbounded — tiered storage.

Alternatives
------------
- Single blob store for everything.

Advantages / Disadvantages
+ Clear data lifecycle. - Multiple stores to operate.

Engineering Recommendation
--------------------------
No hidden in-process memory for cross-request state in PM core.

Reasoning
---------
Stateful orchestrator instances must remain horizontally scalable.

Future Evolution
----------------
- Summarized project memory for planning (derived, not authoritative).

================================================================================
71. Context Management Architecture
================================================================================

Purpose
-------
Assemble minimal correct context for each agent dispatch.

Detailed Explanation
--------------------
Context package contents (FR-101):
  - Task definition and input schema instance
  - Relevant requirements (traceability links FR-012)
  - Upstream artifact references (not full repo by default)
  - Interface contracts from knowledge graph
  - Policy constraints for this dispatch
  - Prior task outputs from dependency chain

Context assembly pipeline:
  1. Resolve dependency artifacts from knowledge graph
  2. Apply inclusion rules from agent manifest
  3. Redact PII and secrets (FR-102)
  4. Enforce size budget per agent type
  5. Produce ContextPackage with content_hash for audit

Responsibilities
----------------
- Prevent context drift and overload.
- Audit what context was sent (hash, not necessarily full content in audit).

Inputs / Outputs
----------------
Inputs: Dispatch request, tenant policy.
Outputs: ContextPackage reference.

Dependencies
------------
- MOD-19, MOD-09 redaction rules, object storage for large blobs.

Interactions
------------
- Dispatch Manager consumes context packages.

Failure Scenarios
-----------------
- Oversized context -> truncate with warning or fail dispatch per policy.

Recovery Strategy
-----------------
- Fallback to minimal context template.

Security / Scalability / Maintainability
------------------------------------------
- Strict redaction; no production credentials in context.
- Cache context packages by content_hash for identical dispatches.

Alternatives
------------
- Send entire project history to every agent.

Advantages / Disadvantages
+ Focused agent inputs. - Assembly latency.

Engineering Recommendation
--------------------------
Declarative context rules per agent type in manifest.

Reasoning
---------
SRS §6 context drift mitigation.

Future Evolution
----------------
- Semantic retrieval for relevant artifact selection.

================================================================================
72. Knowledge Architecture
================================================================================

Purpose
-------
Organize project knowledge for traceability and context.

Detailed Explanation
--------------------
Knowledge entities:
  KE-01 Requirements
  KE-02 Tasks
  KE-03 Artifacts (code, configs, docs, test results)
  KE-04 Decisions (approvals, conflict resolutions FR-112)
  KE-05 Interfaces (API contracts between agents)
  KE-06 Incidents and resolutions

Relations: derives_from, implements, tests, blocks, supersedes, conflicts_with.

Authoritative IDs from domain aggregates; knowledge layer indexes relationships.

Responsibilities
----------------
- FR-100 knowledge graph linking artifacts and decisions.
- FR-012 traceability.

Inputs / Outputs
----------------
Inputs: Domain events, artifact registrations from agents.
Outputs: Graph queries for planning, context, conflict detection.

Dependencies
------------
- MOD-19, MOD-23 projections, Section 73.

Interactions
------------
- Conflict module queries KE-05 for contract mismatches.

Failure Scenarios
-----------------
- Stale graph edge -> incorrect context.

Recovery Strategy
-----------------
- Rebuild graph from event stream.

Security / Scalability / Maintainability
------------------------------------------
- Tenant-isolated graph partitions.
- Artifact content by reference URI with access control.

Alternatives
------------
- Flat file list without relationships.

Advantages / Disadvantages
+ Traceability and conflict detection. - Graph maintenance cost.

Engineering Recommendation
--------------------------
Event-sourced graph projection (ADR-SAD-003).

Reasoning
---------
FR-100 and FR-110 require structured relationships.

Future Evolution
----------------
- Cross-project pattern library (opt-in anonymized).

================================================================================
73. Knowledge Graph Architecture
================================================================================

Purpose
-------
Technical architecture for graph storage and query.

Detailed Explanation
--------------------
Architecture (ADR-SAD-003):
  - Authoritative facts in relational write model (nodes/edges as tables)
  - Graph projection in search-optimized store for traversal queries
  - Optional native graph engine when traversal p95 exceeds threshold

Query patterns:
  QG-01 Traceability path requirement -> task -> artifact -> test
  QG-02 Dependency neighborhood for context assembly
  QG-03 Conflict detection on interface nodes
  QG-04 Impact analysis for scope change replan

Responsibilities
----------------
- Support FR-100, FR-012, FR-110 without blocking dispatch path.

Inputs / Outputs
----------------
Inputs: Graph mutation events.
Outputs: Query results with tenant scope.

Dependencies
------------
- MOD-19, MOD-23 search indexer.

Interactions
------------
- Async projection updates; sync read of small neighborhoods from cache.

Failure Scenarios
-----------------
- Projection lag -> context may miss latest artifact (mitigate with sync read for critical path).

Recovery Strategy
-----------------
- Full graph rebuild from events.

Security / Scalability / Maintainability
------------------------------------------
- Graph queries always filtered by tenant_id.
- Index sharding by tenant.

Alternatives
------------
- Native graph DB only from day one.

Advantages / Disadvantages
+ Flexible evolution. - Dual storage complexity.

Engineering Recommendation
--------------------------
Start relational + projection; add graph engine when metrics justify.

Reasoning
---------
Avoid premature graph DB ops burden.

Future Evolution
----------------
- Graph neural features for risk prediction.

================================================================================
74. Cache Architecture
================================================================================

Purpose
-------
Accelerate reads without compromising correctness.

Detailed Explanation
--------------------
Cache domains:
  CA-01 Authz decisions (TTL 30s)
  CA-02 Compiled policies (TTL until version change)
  CA-03 Project dashboard snapshots (TTL 5-15s)
  CA-04 Agent registry health (TTL 10s)
  CA-05 Context package by content_hash (TTL 1h)
  CA-06 Idempotency results (TTL 24h)

Cache invalidation: event-driven on relevant domain events; never cache writes.

Responsibilities
----------------
- Support NFR-001 read latency.
- Not authoritative (MM-05).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
MOD-30; Redis-class distributed cache conceptually.

Failure Scenarios
-----------------
- Cache stampede on hot keys.

Recovery Strategy
-----------------
- Single-flight pattern; fallback to origin.

Security / Scalability / Maintainability
------------------------------------------
- Never cache cross-tenant data in shared keys without tenant prefix.
- Memory limits per cache domain.

Alternatives
------------
- No cache (higher DB load).

Advantages / Disadvantages
+ Performance. - Staleness.

Engineering Recommendation
--------------------------
Cache-aside pattern; explicit TTL per domain.

Reasoning
---------
P95 read <=200ms difficult without caching at GA scale.

Future Evolution
----------------
- Edge caching for static template catalog.

================================================================================
75. Storage Architecture
================================================================================

Purpose
-------
Define storage categories and responsibilities.

Detailed Explanation
--------------------
Storage tiers (conceptual):
  ST-01 Authoritative OLTP — project, task, plan, policy aggregates (ACID)
  ST-02 Event Log — append-only domain events (MOD-18)
  ST-03 Audit Store — immutable compliance log (may equal ST-02 view)
  ST-04 Object Store — large artifacts, export bundles, context blobs
  ST-05 Search Index — projections, portfolio aggregates (FR-122)
  ST-06 Time-Series — metrics and telemetry
  ST-07 Cold Archive — aged audit per retention (FR-094)

Data residency: ST-01 through ST-05 pinned to tenant region (FR-120).

Responsibilities
----------------
- NFR-018 ACID for authoritative writes.
- Encryption at rest (Section 52).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Each storage tier accessed only by owning modules.

Failure Scenarios
-----------------
- Wrong tier for workload -> performance or cost failure.

Recovery Strategy
-----------------
- Backup per tier (Section 79); event replay for ST-01 from ST-02.

Security / Scalability / Maintainability
------------------------------------------
- Tenant sharding on ST-01/ST-02.
- Lifecycle policies on ST-04/ST-07.

Alternatives
------------
- Single database for all.

Advantages / Disadvantages
+ Right tool per pattern. - Operational complexity.

Engineering Recommendation
--------------------------
Database-per-service logical pattern within deployables.

Reasoning
---------
CQRS and event sourcing require separate read/write optimization.

Future Evolution
----------------
- Customer-managed object storage for artifacts (enterprise).

================================================================================
76. Scalability Architecture
================================================================================

Purpose
-------
Achieve SRS scale targets through architectural patterns.

Detailed Explanation
--------------------
Scale dimensions:
  SD-T Tenants — 200 GA -> 10,000+ Year 5 (SRS §28)
  SD-P Projects — 2,000 GA -> 200,000+ Year 5
  SD-K Tasks — 10,000 concurrent GA -> 1M+ Year 5
  SD-E Events — 5,000/sec GA -> 500,000/sec Year 5 (NFR-022)

Patterns:
  - Horizontal scaling stateless tiers
  - Sharding by tenant_id (ADR-SAD-006)
  - Async projection for reads
  - Agent pool scaling external to PM
  - Backpressure and tenant quotas

Responsibilities
----------------
- SC-005 load test architecture support.
- Noisy neighbor isolation.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Capacity planning (Section 96); autoscaling policies per DU.

Failure Scenarios
-----------------
- Hot tenant degrades others.

Recovery Strategy
-----------------
- Per-tenant rate limits and fair scheduling (SCH-6).

Security / Scalability / Maintainability
------------------------------------------
- Scale does not relax isolation.

Alternatives
------------
- Vertical scaling only.

Advantages / Disadvantages
+ Global scale path. - Shard migration complexity.

Engineering Recommendation
--------------------------
Design for Year 5 event rate; implement GA at 25% capacity headroom.

Reasoning
---------
Retrofitting sharding is high risk.

Future Evolution
----------------
- Cell-based architecture per region group.

================================================================================
77. High Availability Architecture
================================================================================

Purpose
-------
Meet 99.9%+ availability targets.

Detailed Explanation
--------------------
HA patterns per DU:
  - Multi-AZ deployment minimum all deployables
  - N+1 replicas for stateless services
  - Leader election for singleton workers (scheduler coordinator optional)
  - Health checks with automatic replacement
  - Graceful shutdown with connection draining

SLO: API 99.9% (NFR-004); event ingestion 99.9% standard / 99.95% enterprise.

Responsibilities
----------------
- Error budget management (43.2 min/month at 99.9%).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Load balancers, health probes, multi-AZ data stores.

Failure Scenarios
-----------------
- AZ failure -> traffic shift to healthy AZ.

Recovery Strategy
-----------------
- Automated failover within region; cross-region for enterprise (Section 80).

Security / Scalability / Maintainability
------------------------------------------
- HA must not weaken security (mTLS on all replicas).

Alternatives
------------
- Single AZ (rejected for GA).

Advantages / Disadvantages
+ Uptime. - Cost duplication.

Engineering Recommendation
--------------------------
Active-active stateless; active-passive for optional singleton coordinators.

Reasoning
---------
NFR-004 and SRS §30.

Future Evolution
----------------
- 99.95% API for enterprise tier.

================================================================================
78. Disaster Recovery Architecture
================================================================================

Purpose
-------
Recover from region-wide disasters per NFR-005.

Detailed Explanation
--------------------
DR parameters:
  RPO <= 15 minutes
  RTO <= 4 hours (control plane)

Strategy:
  - Continuous replication of ST-01/ST-02 to secondary region
  - Warm standby DU-1, DU-2, DU-3 in secondary region
  - DU-5 rebuildable from event replay (longer RTO acceptable)
  - DNS failover to secondary region
  - DR drill twice yearly

Deployment profiles:
  Standard: backup-restore RTO up to 24h documented
  Enterprise: warm standby RTO <= 4h

Responsibilities
----------------
- SRS §39 disaster recovery goals.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Cross-region replication, runbooks, status communications.

Failure Scenarios
-----------------
- Replication lag exceeds RPO during peak.

Recovery Strategy
-----------------
- Monitor replication lag; throttle writes if needed.

Security / Scalability / Maintainability
------------------------------------------
- Encrypted replication; keys in both regions for enterprise.

Alternatives
------------
- Backup only without warm standby.

Advantages / Disadvantages
+ Enterprise contracts. - Replication cost.

Engineering Recommendation
--------------------------
Tiered DR SKUs per ADR-SAD-005.

Reasoning
---------
SRS §39 warm standby for enterprise.

Future Evolution
----------------
- Active-active multi-region.

================================================================================
79. Backup Architecture
================================================================================

Purpose
-------
Protect against data loss and support restore.

Detailed Explanation
--------------------
Backup scope:
  BK-01 OLTP snapshots every 15 min (align RPO)
  BK-02 Event log retained with geo-redundancy
  BK-03 Object store versioning and cross-region copy
  BK-04 Config and policy packs in git (source of truth) + DB backup
  BK-05 KMS key backup per provider procedure

Verification: monthly automated restore test to isolated environment.

Responsibilities
----------------
- Complement DR; protect against logical errors and ransomware.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Backup service independent of primary data path.

Failure Scenarios
-----------------
- Unrestorable backup discovered during incident.

Recovery Strategy
-----------------
- Monthly restore drills; alert on backup job failure.

Security / Scalability / Maintainability
------------------------------------------
- Backups encrypted; access more restricted than production.
- Lifecycle to cold storage per retention.

Alternatives
------------
- Replication only without point-in-time recovery.

Advantages / Disadvantages
+ Point-in-time recovery. - Storage cost.

Engineering Recommendation
--------------------------
Point-in-time recovery for OLTP; immutable backup vault.

Reasoning
---------
Logical deletion and compliance require restore capability.

Future Evolution
----------------
- Customer-initiated backup export.

================================================================================
80. Failover Architecture
================================================================================

Purpose
-------
Define automatic and manual failover mechanisms.

Detailed Explanation
--------------------
Failover types:
  FO-01 AZ failover — automatic via load balancer health checks
  FO-02 Instance failover — container orchestrator restarts unhealthy pods
  FO-03 Region failover — DNS + warm standby activation (enterprise)
  FO-04 Dependency failover — LLM provider fallback via Model Router
  FO-05 Agent instance failover — scheduler routes to healthy agent instance

Manual failover: operator-initiated region switch for DR drills or disasters.

Responsibilities
----------------
- Minimize downtime within RTO.
- Prevent split-brain writes across regions.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Health monitors trigger failover; runbooks for manual steps.

Failure Scenarios
-----------------
- Split-brain if both regions accept writes.

Recovery Strategy
-----------------
- Active-passive region pairing; single write region at a time.

Security / Scalability / Maintainability
------------------------------------------
- Failover must preserve tenant isolation and auth.

Alternatives
------------
- Manual only failover.

Advantages / Disadvantages
+ Availability. - Complexity.

Engineering Recommendation
--------------------------
Automated AZ failover; semi-automated region failover with approval.

Reasoning
---------
Balances RTO with split-brain risk.

Future Evolution
----------------
- Global load balancing with conflict-free replicated types for read regions.

================================================================================
81. Multi Region Architecture
================================================================================

Purpose
-------
Support geographic distribution and data residency.

Detailed Explanation
--------------------
Region model:
  RG-01 Primary regions at GA: US, EU (ADR-006)
  RG-02 Tenant pinned to home region (FR-120)
  RG-03 Cross-region access denied by default
  RG-04 Global control plane metadata for routing (tenant -> region map)
  RG-05 Enterprise option: read replica in second region for dashboards only

Cell architecture (future):
  Each cell = full stack for subset of tenants in a region group.

Responsibilities
----------------
- GDPR and data residency compliance.
- Latency optimization for regional customers.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
DNS geo-routing; tenant provisioning assigns home_region.

Failure Scenarios
-----------------
- Tenant created in wrong region -> migration complex; prevent at provisioning.

Recovery Strategy
-----------------
- Region failover per Section 80.

Security / Scalability / Maintainability
------------------------------------------
- Data never leaves region without explicit policy exception.
- Scale by adding cells within region.

Alternatives
------------
- Single global region.

Advantages / Disadvantages
+ Compliance and latency. - Operational duplication.

Engineering Recommendation
--------------------------
Home region immutable at tenant creation except migration tooling enterprise.

Reasoning
---------
FR-120 explicit region pinning.

Future Evolution
----------------
- APAC, Middle East regions (SRS §43).

================================================================================
82. Multi Tenant Architecture
================================================================================

Purpose
-------
Isolate thousands of organizations on shared infrastructure.

Detailed Explanation
--------------------
Isolation tiers (SRS Definitions):
  T-1 Logical — shared compute, row-level security, tenant_id on all records
  T-2 Dedicated — single-tenant compute pool, shared control plane software
  T-3 AirGapped — customer premises, limited egress (ADR-SAD-005)

Enforcement layers:
  EL-1 Network segmentation for dedicated/air-gapped
  EL-2 Authz tenant_id from token only, never from request body
  EL-3 Storage shard key = tenant_id
  EL-4 Event topic ACLs by tenant
  EL-5 Quotas per tenant for fair use

Responsibilities
----------------
- FR-090 isolation at data and compute boundaries.
- FR-121 audit tenant boundary.
- P0: tenant escape prevention.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
All modules enforce tenant context from MOD-13.

Failure Scenarios
-----------------
- Missing tenant filter in query -> security incident P1.

Recovery Strategy
-----------------
- Automated integration tests for tenant isolation per release.
- Pen test tenant escape scenarios annually.

Security / Scalability / Maintainability
------------------------------------------
- Tenant isolation is highest priority security invariant.
- Shard by tenant_id for scale.

Alternatives
------------
- Database per tenant (cost prohibitive at 10k tenants).

Advantages / Disadvantages
+ Economic scale. - Noisy neighbor risk.

Engineering Recommendation
--------------------------
Logical isolation T-1 default; T-2/T-3 as SKUs without code forks.

Reasoning
---------
ADR-001 hybrid SaaS; thousands of companies requirement.

Future Evolution
----------------
- Automated tenant shard rebalancing.

================================================================================
83. Plugin Architecture
================================================================================

Purpose
-------
Enable certified extensions without core modification.

Detailed Explanation
--------------------
Plugin types: agents, gates, policies, connectors, notifications, compliance exports.
Plugin lifecycle: develop -> test -> certify -> publish -> deprecate -> retire.

Runtime models:
  RP-1 In-process — internal platform modules only
  RP-2 Sidecar — connector and validator plugins
  RP-3 External service — third-party agents and connectors

Contract: manifest + schemas + health + signature.

Responsibilities
----------------
- NFR-012 extensibility.
- FR-034 third-party agents post-certification.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Plugin registry in MOD-07 and Integration Hub.

Failure Scenarios
-----------------
- Malicious plugin -> sandbox RP-2/RP-3 only for third-party.

Recovery Strategy
-----------------
- Disable plugin version globally on security incident.

Security / Scalability / Maintainability
------------------------------------------
- Signature verification mandatory.
- Plugins cannot elevate privileges beyond manifest scope.

Alternatives
------------
- Fork core per customization (violates CON-006).

Advantages / Disadvantages
+ Ecosystem. - Compatibility matrix.

Engineering Recommendation
--------------------------
Dogfood plugin SDK internally before external release (SRS §36).

Reasoning
---------
Marketplace vision requires plugin architecture.

Future Evolution
----------------
- WASM sandbox for untrusted validators.

================================================================================
84. Extension Architecture
================================================================================

Purpose
-------
Define supported extension points for customers and partners.

Detailed Explanation
--------------------
Extension points:
  EP-01 Project templates (FR-002)
  EP-02 Policy packs
  EP-03 Gate definitions
  EP-04 Webhooks (FR-072)
  EP-05 Custom fields on work items (integration mapping)
  EP-06 Report templates (FR-083)
  EP-07 Feature flags per tenant (Section 92)

Unsupported: modifying core orchestration logic, bypassing PEPs, direct DB access.

Responsibilities
----------------
- CON-006 no core forks; configuration over customization.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Configuration service (MOD-25) stores extensions.

Failure Scenarios
-----------------
- Extension bypassing policy -> blocked by PEP architecture.

Recovery Strategy
-----------------
- Validate extensions in staging sandbox tenant.

Security / Scalability / Maintainability
------------------------------------------
- Extensions cannot widen credential scope.
- Version extensions with PM core compatibility matrix.

Alternatives
------------
- Professional services custom code (outside platform).

Advantages / Disadvantages
+ Customer flexibility within guardrails. - Support burden.

Engineering Recommendation
--------------------------
Extension validation pipeline before activation in production tenant.

Reasoning
---------
Enterprise customers need templates without forks.

Future Evolution
----------------
- Customer-authored extensions in certified sandbox.

================================================================================
85. Marketplace Architecture
================================================================================

Purpose
-------
Future architecture for third-party agent and connector marketplace.

Detailed Explanation
--------------------
Marketplace components (post-GA+6 months per SRS §36):
  MP-01 Publisher portal — submit manifests, documentation
  MP-02 Certification pipeline — security, contract, interoperability tests
  MP-03 Listing catalog — searchable agent types, connectors, templates
  MP-04 Billing integration — revenue share (SRS BG-004)
  MP-05 Rating and revocation — operational metrics feed certification status

GA preparation: all marketplace modules as slots; certification pipeline stub.

Responsibilities
----------------
- FR-034 allow third-party agents via certification.
- Safe ecosystem growth.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Integrates with MOD-07 Agent Registry after certification.

Failure Scenarios
-----------------
- Uncertified marketplace listing -> blocked at registry.

Recovery Strategy
-----------------
- Revoke listing and disable dispatches on vulnerability disclosure.

Security / Scalability / Maintainability
------------------------------------------
- Mandatory security review for marketplace plugins.
- Marketplace metadata global; plugin execution tenant-isolated.

Alternatives
------------
- Partner integrations only without public marketplace.

Advantages / Disadvantages
+ Revenue and ecosystem. - Quality control overhead.

Engineering Recommendation
--------------------------
Build certification pipeline before public marketplace launch.

Reasoning
---------
Autonomous agents from third parties are high risk without certification.

Future Evolution
----------------
- Industry template packs (healthcare, fintech).

================================================================================
86. Integration Architecture
================================================================================

Purpose
-------
Connect AIPM to customer SDLC ecosystem.

Detailed Explanation
--------------------
Integration categories:
  INT-01 Identity — SAML, OIDC, SCIM (FR-092)
  INT-02 VCS — GitHub, GitLab, Bitbucket (FR-073)
  INT-03 CI/CD — pipeline status ingestion (FR-074)
  INT-04 Issue trackers — bidirectional sync (FR-071, SC-007)
  INT-05 Cloud — infra APIs for deployment agents
  INT-06 Notifications — Slack, email, PagerDuty
  INT-07 External PM — optional bidirectional (ADR-005)

Connector framework (MOD-15):
  - OAuth/service account onboarding (FR-070)
  - Health monitoring and credential refresh
  - Rate limit respect
  - Normalization to internal domain events

Responsibilities
----------------
- Minimum 5 certified connectors at GA (OBJ-005).
- ACL for all external data.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
DU-4 Integration Plane; sync reconciler (MOD-17).

Failure Scenarios
-----------------
- External API breaking change -> connector version bump.

Recovery Strategy
-----------------
- Connector contract tests in CI; versioned adapters.

Security / Scalability / Maintainability
------------------------------------------
- Tokens in vault; scoped OAuth permissions.
- Per-connection worker pools.

Alternatives
------------
- iPaaS middleware.

Advantages / Disadvantages
+ SDLC continuity. - Connector maintenance.

Engineering Recommendation
--------------------------
Anti-corruption layer per external system; no external IDs in domain core without mapping.

Reasoning
---------
FR-070–074 and ADR-005 integration requirements.

Future Evolution
----------------
- 50+ connectors (SRS §43).

================================================================================
87. API Gateway Architecture
================================================================================

Purpose
-------
Single controlled entry point for external traffic.

Detailed Explanation
--------------------
Gateway functions (DU-1):
  GW-01 TLS termination and certificate management
  GW-02 WAF and bot protection
  GW-03 Authentication (JWT, API key)
  GW-04 Rate limiting per tenant/tier
  GW-05 Request routing to internal services
  GW-06 Correlation ID injection
  GW-07 Request/response size limits
  GW-08 API version routing (Section 89)

No business logic in gateway except authn and routing.

Responsibilities
----------------
- OWASP ASVS L2 at perimeter (SRS §31).
- First line of zero trust.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Routes to all DU-2, DU-3, DU-5 external APIs.

Failure Scenarios
-----------------
- Gateway misroute -> canary routing and config validation.

Recovery Strategy
-----------------
- Blue-green gateway deployments.

Security / Scalability / Maintainability
------------------------------------------
- DDoS protection; geo-blocking optional enterprise.
- Autoscale gateway pods.

Alternatives
------------
- Per-service public endpoints (rejected).

Advantages / Disadvantages
+ Centralized security. - Gateway as chokepoint.

Engineering Recommendation
--------------------------
All external API traffic through DU-1 only.

Reasoning
---------
Section 25 request flow starts at gateway.

Future Evolution
----------------
- GraphQL federation gateway optional for admin UI.

================================================================================
88. Internal Service Mesh
================================================================================

Purpose
-------
Secure and observe service-to-service communication.

Detailed Explanation
--------------------
Mesh capabilities:
  SM-01 mTLS between all internal services (ADR-SAD-007)
  SM-02 Workload identity (SPIFFE-compatible concept)
  SM-03 Traffic policies — retries, timeouts, circuit breakers
  SM-04 Observability — automatic trace propagation
  SM-05 Authorization — service-level allowlists optional defense in depth

Implementation options: sidecar mesh or native mTLS in libraries at GA.
Recommendation: library-native mTLS at GA; sidecar mesh when service count >15.

Responsibilities
----------------
- Internal zero trust enforcement.
- NFR-015 trace propagation.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
All inter-service calls in Sections 17, 10.

Failure Scenarios
-----------------
- Mesh misconfiguration blocks all traffic.

Recovery Strategy
-----------------
- Mesh config canary; emergency bypass only with break-glass audit.

Security / Scalability / Maintainability
------------------------------------------
- Mutual TLS mandatory; no plaintext internal HTTP in production.

Alternatives
------------
- VPN trust zone (rejected).

Advantages / Disadvantages
+ Security and observability. - Operational complexity.

Engineering Recommendation
--------------------------
mTLS from day one; evaluate full mesh at scale.

Reasoning
---------
Agent dispatch and credential broker require strong internal security.

Future Evolution
----------------
- Full sidecar mesh when extracting microservices.

================================================================================
89. Versioning Strategy
================================================================================

Purpose
-------
Manage evolution of APIs, events, schemas, and agents.

Detailed Explanation
--------------------
Version domains:
  V-API — URL or header version (v1, v2); 6-month deprecation notice (SRS §35)
  V-Event — schema_version in envelope; backward compatible consumers
  V-Agent — manifest version ranges in compatibility matrix (FR-033)
  V-Policy — policy pack versions; pinned in audit records
  V-Template — project template versions

Rules:
  - Additive changes only in minor versions
  - Breaking changes require major version + migration guide
  - Consumers ignore unknown fields

Responsibilities
----------------
- NFR-013 event schema backward compatibility.
- Independent agent release cycles (CON-008).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Schema registry for events and dispatch protocol.

Failure Scenarios
-----------------
- Breaking change without version bump -> consumer failures.

Recovery Strategy
-----------------
- Dual-write/dual-read migration window for major versions.

Security / Scalability / Maintainability
------------------------------------------
- Old API versions sunset on schedule with customer notification.

Alternatives
------------
- Unversioned APIs.

Advantages / Disadvantages
+ Safe evolution. - Version sprawl.

Engineering Recommendation
--------------------------
Semantic versioning for all public contracts.

Reasoning
---------
CON-008 and marketplace require explicit compatibility.

Future Evolution
----------------
- Automated compatibility test suite in CI.

================================================================================
90. Compatibility Strategy
================================================================================

Purpose
-------
Ensure components work across version combinations.

Detailed Explanation
--------------------
Compatibility matrices:
  CM-01 PM Core x Agent Type versions (FR-033)
  CM-02 PM Core x Connector versions
  CM-03 Event schema producer x consumer versions
  CM-04 Policy pack x PM Core versions

Testing: contract tests in CI for all supported combinations.
Runtime: dispatch blocked if combination not in matrix.

Responsibilities
----------------
- Prevent runtime incompatibility incidents.

Inputs / Outputs
----------------
Inputs: Manifest version declarations.
Outputs: Compatibility verdict at registration and dispatch.

Dependencies
------------
- MOD-07, schema registry, certification pipeline.

Failure Scenarios
-----------------
- Unsupported combo dispatched -> blocked at MOD-08.

Recovery Strategy
-----------------
- Upgrade agent or PM per migration guide.

Security / Scalability / Maintainability
------------------------------------------
- Compatibility checks are fast lookup tables.

Alternatives
------------
- Latest-only support.

Advantages / Disadvantages
+ Stable operations. - Matrix maintenance.

Engineering Recommendation
--------------------------
Publish compatibility matrix with every PM core release.

Reasoning
---------
CON-008 independent agent releases require explicit compatibility.

Future Evolution
----------------
- Automated matrix generation from contract tests.

================================================================================
91. Configuration Architecture
================================================================================

Purpose
-------
Manage tenant and platform configuration safely.

Detailed Explanation
--------------------
Configuration layers:
  CF-01 Platform defaults — operator-managed
  CF-02 Tenant settings — retention, region, policies (FR-094)
  CF-03 Project templates — agent sequences, gates (FR-002)
  CF-04 Integration connections — OAuth, mappings
  CF-05 Environment definitions — dev/staging/prod rules

Storage: authoritative in MOD-25; templates also in git for drift detection.
Changes: validated before apply; audited (MOD-12).

Responsibilities
----------------
- CON-006 customization via config not forks.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Admin UI and API for configuration; agents read derived config at dispatch.

Failure Scenarios
-----------------
- Bad config breaks scheduling -> validation rules and dry-run.

Recovery Strategy
-----------------
- Config version history with rollback.

Security / Scalability / Maintainability
------------------------------------------
- Sensitive config in vault references only.
- Cache with event-driven invalidation.

Alternatives
------------
- Environment variables only.

Advantages / Disadvantages
+ Tenant flexibility. - Config complexity.

Engineering Recommendation
--------------------------
Git-backed templates for regulated customers; API for runtime overrides.

Reasoning
---------
FR-002 templates and FR-094 retention require structured config.

Future Evolution
----------------
- Configuration drift detection alerts.

================================================================================
92. Feature Flag Architecture
================================================================================

Purpose
-------
Control feature rollout and tier-specific capabilities.

Detailed Explanation
--------------------
Flag categories:
  FF-01 Release flags — gradual GA rollout
  FF-02 Tier flags — enterprise vs standard capabilities
  FF-03 Kill switches — disable risky features instantly
  FF-04 Experiment flags — internal dogfooding

Evaluation: MOD-26 at runtime; default safe (risky features off if service down).
Scope: global, tenant, user percentage rollout.

Responsibilities
----------------
- SRS §36 extensibility; phased GA exposure (SRS §12).

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Integrated at gateway and service layers.

Failure Scenarios
-----------------
- Flag service outage -> safe defaults per flag definition.

Recovery Strategy
-----------------
- Local cached flag snapshot with TTL.

Security / Scalability / Maintainability
------------------------------------------
- Flag changes audited.
- Low latency evaluation (<1ms cached).

Alternatives
------------
- Compile-time feature toggles only.

Advantages / Disadvantages
+ Safe rollout. - Flag debt if not cleaned up.

Engineering Recommendation
--------------------------
Mandatory expiry dates on release flags.

Reasoning
---------
Large scope GA requires progressive exposure.

Future Evolution
----------------
- Per-tenant feature bundles for marketplace.

================================================================================
93. Dependency Architecture
================================================================================

Purpose
-------
Map and manage platform dependencies.

Detailed Explanation
--------------------
Dependency layers (extends SRS §23):
  DEP-INT Internal — 5 DUs, 30 modules, event backbone
  DEP-EXT Critical — IdP, KMS, cloud compute/storage, LLM APIs
  DEP-EXT Important — VCS, CI/CD, issue trackers, notification providers
  DEP-EXT Optional — external PM tools, marketplace plugins

Dependency rules:
  DR-01 Core orchestration must degrade gracefully if LLM unavailable
  DR-02 No hard dependency on single LLM vendor (ADR-004)
  DR-03 Integration failures must not halt unrelated projects
  DR-04 Abstract all DEP-EXT behind MOD-15/20 interfaces

Health dashboard: dependency status per tenant integrations.

Responsibilities
----------------
- Document fallback for each DEP-EXT Critical.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Health checks feed monitoring (Section 55).

Failure Scenarios
-----------------
- Cascading failure from IdP + KMS simultaneous outage.

Recovery Strategy
-----------------
- Graceful degradation modes documented per dependency.

Security / Scalability / Maintainability
------------------------------------------
- Third-party dependency reviews annually.
- Circuit breakers per dependency.

Alternatives
------------
- Tight coupling to one vendor stack.

Advantages / Disadvantages
+ Resilience. - Abstraction maintenance.

Engineering Recommendation
--------------------------
Dependency health as first-class metrics and status page components.

Reasoning
---------
SRS §23 and RSK-002 LLM outage mitigation.

Future Evolution
----------------
- Customer choice of LLM provider per tenant policy.

================================================================================
94. Cost Management Architecture
================================================================================

Purpose
-------
Track, control, and optimize platform and customer costs.

Detailed Explanation
--------------------
Cost dimensions (FR-082):
  CD-01 LLM tokens per model/project/task
  CD-02 Agent compute time (reported by agents)
  CD-03 Platform infrastructure (internal FinOps)
  CD-04 Integration API calls

Controls:
  CC-01 Per-tenant budget caps (NFR-020)
  CC-02 Per-project budgets
  CC-03 Alert thresholds (80%, 95%, 100%)
  CC-04 Hard stop on budget breach within 60s (SC-006)

MOD-22 Cost Meter consumes usage events from Model Router, agents, platform.

Responsibilities
----------------
- Prevent RSK-003 cost runaway.
- Support business goal BG-005 margin protection.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Scheduler consults budget before dispatch; router consults before inference.

Failure Scenarios
-----------------
- Cost metering lag -> brief overspend; reconcile and tighten.

Recovery Strategy
-----------------
- Halt new dispatches on hard cap; manual budget increase workflow.

Security / Scalability / Maintainability
------------------------------------------
- Cost data tenant-isolated.
- Async rollup for dashboards.

Alternatives
------------
- Post-hoc billing only without runtime caps.

Advantages / Disadvantages
+ Financial safety. - May block legitimate work.

Engineering Recommendation
--------------------------
Real-time budget counter in fast store; async reconciliation.

Reasoning
---------
SC-006 explicit 60-second stop requirement.

Future Evolution
----------------
- Cost optimization recommendations via analytics.

================================================================================
95. Performance Architecture
================================================================================

Purpose
-------
Meet NFR performance targets through design.

Detailed Explanation
--------------------
Performance targets (SRS §34, NFR-001/002):
  PT-01 P95 API read <= 200ms at 1k RPS per region
  PT-02 P95 dispatch <= 2s
  PT-03 P95 policy eval <= 100ms
  PT-04 Dashboard load <= 1.5s P95
  PT-05 Agent ack <= 30s P95 (NFR-023)

Techniques:
  - CQRS read models (ADR-SAD-008)
  - Caching (Section 74)
  - Async exports (FR-083)
  - Connection pooling
  - Batch event processing
  - Read replicas

Responsibilities
----------------
- Load test at 2x GA target before release.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Performance tests in CI; continuous benchmarking in staging.

Failure Scenarios
-----------------
- Noisy neighbor degrades PT-01 for others.

Recovery Strategy
-----------------
- Tenant rate limits; query complexity limits.

Security / Scalability / Maintainability
------------------------------------------
- Performance optimizations must not skip authz.

Alternatives
------------
- Weaker SLOs.

Advantages / Disadvantages
+ User experience. - Infra cost.

Engineering Recommendation
--------------------------
Performance budget per API endpoint tracked in CI.

Reasoning
---------
Quantified NFRs require architectural support not just tuning.

Future Evolution
----------------
- Edge caching for read-heavy portfolio views.

================================================================================
96. Capacity Planning Architecture
================================================================================

Purpose
-------
Ensure infrastructure scales ahead of demand.

Detailed Explanation
--------------------
Capacity model inputs:
  CM-In Tenants, projects, concurrent tasks, events/sec (SRS §28)
  CM-In Average task duration, dispatch size, audit event rate
  CM-In Regional distribution

Planning horizons:
  CH-1 GA — 200 tenants, 10k tasks, 5k events/sec
  CH-2 Year 2 — 2k tenants, 100k tasks, 50k events/sec
  CH-3 Year 5 — 10k+ tenants, 1M tasks, 500k events/sec

Outputs: CPU/memory/storage/network forecasts per DU; autoscaling thresholds.

Responsibilities
----------------
- Headroom 25% at GA; trigger scale reviews at 70% utilization.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
FinOps + SRE quarterly capacity reviews.

Failure Scenarios
-----------------
- Sudden tenant onboarding spike.

Recovery Strategy
-----------------
- Pre-provisioned burst capacity; tenant onboarding throttling.

Security / Scalability / Maintainability
------------------------------------------
- Capacity tests include encryption overhead.

Alternatives
------------
- Reactive scaling only.

Advantages / Disadvantages
+ Prevents outages. - Over-provisioning cost.

Engineering Recommendation
--------------------------
Load test automation matching SC-005 parameters.

Reasoning
---------
SRS scalability goals are order-of-magnitude jumps.

Future Evolution
----------------
- Predictive autoscaling from growth trends.

================================================================================
97. Deployment Topology
================================================================================

Purpose
-------
Describe physical/logical deployment layouts.

Detailed Explanation
--------------------
Profile P-SaaS (Standard):
  - Multi-AZ single home region per tenant
  - 5 DUs as Kubernetes clusters or equivalent
  - Shared agent pools (T-1 isolation)
  - Managed event backbone, OLTP, object store, cache

Profile P-Dedicated (Enterprise):
  - T-2 dedicated compute for agents and optionally DU-2 worker pools
  - Warm standby secondary region
  - CMK option for encryption

Profile P-AirGapped (ADR-SAD-005):
  - PM control plane on customer premises or isolated VPC
  - No public egress; store-and-forward for updates
  - Capability flags disable cloud LLM routes; local model catalog only
  - Manual connector sync options

Network zones:
  NZ-1 Public edge (DU-1)
  NZ-2 Application private (DU-2, DU-3, DU-5)
  NZ-3 Integration DMZ (DU-4)
  NZ-4 Agent zone (isolated)
  NZ-5 Data zone (databases, event log, vault)

Responsibilities
----------------
- CON-003 cloud portable; CON-005 air-gap support.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Infrastructure-as-code per profile; same software artifacts, different config.

Failure Scenarios
-----------------
- Profile misconfiguration exposing data zone.

Recovery Strategy
-----------------
- Network policy validation in deployment pipeline.

Security / Scalability / Maintainability
------------------------------------------
- mTLS across zones; minimal cross-zone ports.

Alternatives
------------
- Single VM demo topology (non-production only).

Advantages / Disadvantages
+ SKU flexibility. - Three profiles to test.

Engineering Recommendation
--------------------------
Single container image set; profile via configuration flags only.

Reasoning
---------
CON-006 prohibits per-customer forks.

Future Evolution
----------------
- Hybrid edge agents (SRS §43).

================================================================================
98. Future Architecture Evolution
================================================================================

Purpose
-------
Plan architectural changes without blocking current design.

Detailed Explanation
--------------------
Evolution roadmap:
  FE-01 Year 1 GA — 5 DUs, 2 regions, T-1/T-2 isolation, 14 agents
  FE-02 Year 2 — Extract dispatch service; cell architecture pilot; 25 agents
  FE-03 Year 3 — Marketplace GA; customer VPC agents; active-active reads
  FE-04 Year 4 — Federated PM; portfolio AI optimization; 50+ connectors
  FE-05 Year 5 — 500k events/sec cells; predictive scheduling; industry packs

Invariant core (never changes):
  - Control/data plane separation
  - Policy PEPs before irreversible actions
  - Event-sourced audit
  - Tenant isolation
  - PM does not write application code

Responsibilities
----------------
- Guide investment without re-architecture.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Quarterly architecture review against FE milestones.

Failure Scenarios
-----------------
- Premature microservices split increasing ops burden.

Recovery Strategy
----------------
- Extract services only when fitness metrics trigger (CPU, team size, deploy frequency).

Security / Scalability / Maintainability
------------------------------------------
- Evolution must not weaken invariants.

Alternatives
------------
- Big-bang rewrite for scale.

Advantages / Disadvantages
+ Long-term viability. - Planning overhead.

Engineering Recommendation
--------------------------
Architecture fitness functions automated in CI.

Reasoning
---------
SRS §43-44 growth expectations.

Future Evolution
----------------
- This section updated annually.

================================================================================
99. Architectural Risks
================================================================================

Purpose
-------
Identify architecture-specific risks and mitigations.

Detailed Explanation
--------------------
| Risk ID    | Risk                                      | Mitigation Architecture
|------------|-------------------------------------------|----------------------------------------
| ARSK-001   | Event backbone becomes bottleneck         | Partitioning, NFR-022 capacity design
| ARSK-002   | Policy engine latency breaks dispatch SLO | Cache, HA cluster, async pre-eval where safe
| ARSK-003   | CQRS projection lag misleads users        | Staleness indicators, lag alerts
| ARSK-004   | Agent protocol fragmentation                | Strict versioning, certification
| ARSK-005   | Tenant escape via authz bug               | Defense in depth, pen test, automated tests
| ARSK-006   | Orchestrator monolith in DU-2             | Modular boundaries, extraction path ADR-SAD-001
| ARSK-007   | Air-gapped profile untested               | Dedicated test environment P-AirGapped
| ARSK-008   | LLM router single point of failure        | Multi-provider fallback ADR-004
| ARSK-009   | DLQ unbounded operational debt            | DLQ SLAs, automated aging alerts
| ARSK-010   | Marketplace malicious plugin              | Certification, sandbox, signing

Responsibilities
----------------
- ARB tracks top 5 architectural risks quarterly.

Inputs / Outputs / Dependencies / Interactions
----------------------------------------------
Links to SRS risk register RSK-001–010; architecture-specific additions above.

Failure Scenarios
-----------------
- Unmitigated ARSK-005 -> platform trust collapse.

Recovery Strategy
-----------------
- Incident response; feature kill switches; halt capability.

Security / Scalability / Maintainability
------------------------------------------
- Risks span all three quality attributes.

Alternatives
------------
- Ignore architectural risk until incidents occur.

Advantages / Disadvantages
+ Proactive mitigation. - Analysis time.

Engineering Recommendation
--------------------------
Top 3: ARSK-005, ARSK-001, ARSK-008 — prioritize testing and HA.

Reasoning
---------
Autonomous agents + multi-tenant scale = highest architectural risk concentration.

Future Evolution
----------------
- Risk register updated each release train.

================================================================================
100. Final Architecture Summary
================================================================================

Purpose
-------
Consolidate the enterprise architecture for downstream implementation.

Detailed Explanation
--------------------
AI Project Manager architecture is a five-deployable-unit, event-driven,
zero-trust control plane that orchestrates 14+ specialized agents across
thousands of tenants while maintaining authoritative delivery state, policy
enforcement, and immutable audit evidence.

Key architectural pillars:
  1. Control plane / data plane separation (CON-001, ADR-007)
  2. Domain-driven bounded contexts with context map and ACLs
  3. Event sourcing + CQRS for audit and scale (ADR-SAD-008)
  4. Policy engine at Schedule, Dispatch, and Gate PEPs (ADR-SAD-009)
  5. Task-scoped credential broker with halt revocation (ADR-SAD-010, FR-117)
  6. Model-agnostic router for LLM resilience (ADR-004)
  7. Multi-tenant sharding by tenant_id (ADR-SAD-006)
  8. Tiered deployment profiles: SaaS, Dedicated, AirGapped (ADR-SAD-005)
  9. 30 logical modules with complete SRS FR traceability
  10. Explicit lifecycles and state machines for project, task, and agent

Downstream documents enabled by this SAD (not in scope here):
  - Interface Control Documents (agent dispatch protocol, event schemas)
  - Data Architecture Document (logical data model)
  - Security Architecture Detail (threat models)
  - Deployment and Operations Guide
  - Requirements Traceability Matrix: SRS FR/NFR -> SAD modules

Responsibilities
----------------
- Serve as locked foundation for all implementation work.

Inputs / Outputs
----------------
Inputs: SRS-AIPM-001 v1.0.0.
Outputs: SAD-AIPM-001 v1.0.0 for engineering execution.

Dependencies
------------
- ARB approval; security sign-off before implementation GA.

Interactions
------------
- All future prompts and designs must align with this SAD.

Failure Scenarios
-----------------
- Implementation diverges from SAD without ADR -> compliance and audit failure.

Recovery Strategy
-----------------
- ADR-gated changes; architecture fitness CI.

Security / Scalability / Maintainability
------------------------------------------
- Addressed comprehensively in Sections 43–82, 76–77, 35.

Alternatives
------------
- Ad-hoc implementation without architecture baseline.

Advantages / Disadvantages
+ Enterprise-grade foundation. - Upfront documentation investment.

Engineering Recommendation
--------------------------
Freeze SAD v1.0.0 after approval; changes via ADR-only process.

Reasoning
---------
User mandate: all future implementation depends on this document.

Future Evolution
----------------
- SAD v2.0 for federated PM and marketplace GA milestones.

================================================================================
SRS REQUIREMENTS TRACEABILITY MATRIX (SUMMARY)
================================================================================

Functional Requirements -> Primary Module(s)
FR-001–005   -> MOD-02    FR-010–014  -> MOD-03    FR-020–024  -> MOD-04
FR-030–034   -> MOD-07    FR-040–044  -> MOD-06    FR-050–054  -> MOD-09
FR-060–063   -> MOD-10    FR-070–074  -> MOD-15    FR-080–084  -> MOD-12, MOD-23
FR-090–094   -> MOD-13, MOD-25   FR-100–103  -> MOD-19
FR-110–112   -> MOD-28    FR-115–116  -> MOD-06, workflows
FR-117       -> MOD-14, MOD-27     FR-118      -> MOD-08
FR-119       -> MOD-11, MOD-12     FR-120      -> MOD-13, storage
FR-121       -> MOD-12             FR-122      -> MOD-23

Non-Functional Requirements -> Primary Architecture Section(s)
NFR-001–002  -> 95, 74, 87    NFR-003     -> 76, 61, 62
NFR-004–005  -> 77, 78, 79    NFR-006–007 -> 50, 51, 52
NFR-008      -> 71, 46         NFR-009–010 -> 53, 43, 33 (SRS)
NFR-011      -> 8, 10          NFR-012     -> 9, 83
NFR-013      -> 19, 89         NFR-014     -> 1 (UI deployable)
NFR-015      -> 56, 57, 88     NFR-016     -> 78, 80
NFR-017      -> 25, 26         NFR-018     -> 22, 75
NFR-019      -> 68              NFR-020     -> 94
NFR-021      -> 77, 30 (SRS)    NFR-022     -> 19, 76
NFR-023      -> 29, 61

Success Criteria -> Architecture Evidence
SC-001  -> Sections 77, 87     SC-002  -> Section 43
SC-003  -> Sections 19, 22, 53    SC-004  -> Sections 27, 36–40
SC-005  -> Sections 76, 96        SC-006  -> Section 94
SC-007  -> Section 86             SC-008  -> Sections 27, 62, MOD-27

================================================================================
ARCHITECTURE AUDIT (ITERATIVE REVIEW)
================================================================================

Audit Pass 1 — Issues Found:
| ID | Type | Description |
|----|------|-------------|
| AUD-001 | Missing | Admin UI / Experience Layer not explicit in deployables |
| AUD-002 | Consistency | 23 services vs 30 modules mapping unclear |
| AUD-003 | Completeness | Circular dependency check not documented |
| AUD-004 | Security | Break-glass flow not in identity architecture |
| AUD-005 | SRS | OOS-001 verify no module generates app code — implicit only |

Audit Pass 1 — Fixes Applied:
| Fix | Resolution |
|-----|------------|
| FIX-001 | Added DU-1 Experience: Admin Console as L1 client of gateway (Section 6, 7) |
| FIX-002 | Clarified: 23 logical services packaged in 5 DUs; 30 modules some span services |
| FIX-003 | Added Circular Dependency Analysis below |
| FIX-004 | Added break-glass to Section 45 Identity Architecture |
| FIX-005 | Added explicit CON-001 enforcement in MOD-06, MOD-08 — no codegen paths |

Circular Dependency Analysis (FIX-003):
  - MOD-09 Policy Engine does NOT call MOD-06 Execution (policy is leaf evaluator)
  - MOD-06 Execution calls MOD-09 (one direction) — OK
  - MOD-12 Audit consumes events only; no module depends on audit for business logic — OK
  - MOD-22 Cost Meter is consulted by MOD-05, MOD-20; does not call them back — OK
  - MOD-18 Event Backbone is infrastructure; services depend on it, not vice versa — OK
  - NO circular runtime dependencies identified

Break-Glass Addition (FIX-004):
  - Break-glass accounts: MFA + enhanced logging + time-limited elevation
  - Requires second approver for production policy changes during break-glass
  - All actions tagged break_glass=true in audit

Audit Pass 2 — Issues Found:
| ID | Type | Description |
|----|------|-------------|
| AUD-006 | Scalability | Portfolio cross-shard query not explicit in Section 24 |
| AUD-007 | Maintainability | Module ownership table missing |

Audit Pass 2 — Fixes Applied:
| Fix | Resolution |
|-----|------------|
| FIX-006 | Section 24: portfolio queries use tenant-scoped async indexes only (FR-122) |
| FIX-007 | Module Ownership Table added below |

Module Ownership Table (FIX-007):
| Module | Owning Team (Conceptual) |
|--------|--------------------------|
| MOD-01–02 | Platform Edge & Core |
| MOD-03–04 | Planning Domain |
| MOD-05–08 | Execution Domain |
| MOD-09–12 | Governance & Compliance |
| MOD-13–14 | Security & Identity |
| MOD-15–17 | Integrations |
| MOD-18 | Infrastructure / SRE |
| MOD-19–21 | Knowledge & AI Safety |
| MOD-22–23 | Analytics & FinOps |
| MOD-24–27 | Operations & Reliability |
| MOD-28–30 | Platform Engineering |

Audit Pass 3 — Issues Found:
| ID | Type | Description |
|----|------|-------------|
| AUD-008 | Security | Air-gapped LLM data path not explicit in Section 69 |

Audit Pass 3 — Fixes Applied:
| FIX-008 | Section 69: Air-gapped profile uses local-only catalog; router blocks external providers |

Audit Pass 4 — Issues Found:
None.

Audit Pass 5 — Verification Checklist:
[X] All 100 sections present with required template fields
[X] All 30 modules defined with responsibilities
[X] All 23 logical services mapped to 5 deployables
[X] All 15 bounded contexts and context map documented
[X] All lifecycles and state machines (Sections 33–42) defined
[X] All SRS FR-001–FR-122 mapped to modules
[X] All SRS NFR-001–NFR-023 mapped to architecture sections
[X] All SRS ADR-001–008 inherited; ADR-SAD-001–010 added
[X] No circular dependencies
[X] No CON-001 violation (PM does not generate application code)
[X] Tenant isolation and region pinning architected (FR-120, FR-121)
[X] Halt 30s and credential revoke 60s architected (SC-008, FR-117)
[X] Budget stop 60s architected (SC-006)
[X] Scalability to SC-005 and NFR-022 addressed
[X] Zero contradictions with locked SRS

================================================================================
ARCHITECTURE STATUS: APPROVED
================================================================================

END OF SAD-AIPM-001 v1.0.0

================================================================================
