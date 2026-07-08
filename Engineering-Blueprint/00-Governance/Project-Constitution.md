# AI Project Manager — Project Constitution

**Document ID:** PC-AIPM-001  
**Product:** AI Project Manager (AIPM)  
**Version:** 1.0.0  
**Classification:** Supreme Engineering Authority  
**Date:** 2026-07-07  
**Location:** `Engineering-Blueprint/00-Governance/Project-Constitution.md`  
**Status:** APPROVED

---

## Authority and precedence

This Constitution is the **supreme engineering authority** for the AI Project Manager program. It binds all humans, all AI agents, all documentation, and all implementation.

When artifacts conflict, resolve in this order:

1. **This Constitution** (PC-AIPM-001) — operational law and non-negotiable principles  
2. **SRS-AIPM-001** — requirements source of truth (LOCKED)  
3. **SAD-AIPM-001** — architecture source of truth (APPROVED)  
4. **ADRs** (ADR-001–008, ADR-SAD-001–010, and successors)  
5. **Other Engineering Blueprint documents** (`01`–`16`, `Assets`)  
6. **Implementation code and runtime configuration**

This Constitution MUST NOT contradict SRS or SAD. It operationalizes them into enforceable law.

| Parent document | ID | Version | Status |
|-----------------|-----|---------|--------|
| Software Requirements Specification | SRS-AIPM-001 | 1.0.0 | APPROVED — LOCKED |
| Software Architecture Document | SAD-AIPM-001 | 1.0.0 | APPROVED |

---

## Section template

Sections 1–50 follow: **Purpose · Rule · Rationale · Examples · Exceptions · Compliance Requirements · Violation Consequences**

---

## 1. Purpose

**Purpose**  
Establish why this Constitution exists and what it protects.

**Rule**  
This Constitution MUST define immutable engineering law for AIPM so that requirements (SRS) and architecture (SAD) are interpreted consistently at scale. No downstream document, agent, or code path MAY weaken safety, tenancy, audit, or governance without a formal amendment.

**Rationale**  
Large autonomous systems fail when teams interpret requirements differently. A single constitutional layer prevents contradictions months later.

**Examples**  
- Domain model authors cite PC sections before defining aggregates.  
- Agent teams certify against PC Section 28 before registry publication.

**Exceptions**  
None. Purpose applies to all program work.

**Compliance Requirements**  
Every new blueprint document MUST list PC-AIPM-001 in its References section.

**Violation Consequences**  
Documents without constitutional alignment MUST NOT be approved. Implementation violating purpose MUST be blocked in architecture review.

---

## 2. Vision Protection

**Purpose**  
Protect the north star from scope creep and feature drift.

**Rule**  
All work MUST advance AIPM as the **trusted control plane** for autonomous software delivery. Work MUST NOT redefine AIPM as a code generator, chat assistant, demo, MVP, or generic workflow tool (SRS §2–3, OOS-001, OOS-009).

**Rationale**  
Vision drift causes architectural violations (e.g., PM writing application code).

**Examples**  
- Allowed: orchestration feature improving dispatch latency.  
- Forbidden: PM IDE plugin that writes customer application code.

**Exceptions**  
Customer-facing applications built *by agents under PM governance* are in scope for agents, not for PM core (CON-001).

**Compliance Requirements**  
ARB MUST reject proposals that violate vision protection in charter review.

**Violation Consequences**  
P1 architecture violation; feature MUST NOT ship; ADR required to proceed if dispute.

---

## 3. Architectural Principles

**Purpose**  
Encode non-negotiable structural rules from SAD.

**Rule**  
Implementations MUST preserve: (a) control plane / data plane separation; (b) five deployable units at GA (ADR-SAD-001); (c) event-sourced audit paths; (d) CQRS read/write split (ADR-SAD-008); (e) zero-trust internal communication (ADR-SAD-007); (f) `tenant_id` sharding (ADR-SAD-006).

**Rationale**  
SAD defines production-grade structure; ad-hoc structure blocks scale and audit.

**Examples**  
- Dispatch logic in Core DU; dashboards in Analytics DU.  
- Domain events emitted on every authoritative state change.

**Exceptions**  
Service extraction beyond five DUs MAY occur only with ADR and ARB approval when fitness metrics justify (SAD §98).

**Compliance Requirements**  
Architecture fitness checks in CI MUST validate DU boundaries and forbidden imports.

**Violation Consequences**  
Merge blocked; ARB remediation plan required within one sprint.

---

## 4. Engineering Principles

**Purpose**  
Bind daily engineering behavior to approved philosophy (SRS §46–48).

**Rule**  
Engineering MUST be: API-first, event-first, observable-first, testable, fail-closed on security/policy errors, and evolutionary (not big-bang microservices). Silent failures MUST NOT occur.

**Rationale**  
Distributed autonomous systems require discipline beyond feature delivery.

**Examples**  
- New capability exposes versioned API before UI.  
- Policy timeout returns DENY, not permit.

**Exceptions**  
Internal-only prototypes MAY omit APIs only in non-production sandboxes with explicit expiry.

**Compliance Requirements**  
Code review checklist MUST include engineering principles sign-off.

**Violation Consequences**  
Review rejection; repeat violations escalated to engineering lead.

---

## 5. Development Philosophy

**Purpose**  
Set culture for how software is built.

**Rule**  
Teams MUST prefer clarity over cleverness, configuration over forks, schema-first contracts over stringly-typed payloads, and smallest correct change over speculative abstraction (SRS §47, CON-006).

**Rationale**  
Finite team size and enterprise longevity demand boring, readable systems.

**Examples**  
- Tenant policy via template, not core fork.  
- Agent contract versioned in manifest, not embedded in PM code.

**Exceptions**  
Performance-critical paths MAY optimize after measured profiling with ADR.

**Compliance Requirements**  
Implementation guides in `13-Implementation/` MUST reflect this philosophy when authored.

**Violation Consequences**  
Refactor required before GA inclusion; technical debt ticket mandatory.

---

## 6. AI Governance Principles

**Purpose**  
Govern AI usage safely at enterprise scale (SRS §49).

**Rule**  
AI MUST propose; policies and humans MUST dispose. Model output MUST be treated as untrusted until validated (NFR-019). LLM access MUST route through approved catalog (ADR-004). Customer data MUST NOT train models without explicit contractual opt-in.

**Rationale**  
Autonomous AI without governance creates security and compliance liability.

**Examples**  
- Plan suggested by model; activation requires approval gate.  
- Agent output passes schema validator before TaskSucceeded.

**Exceptions**  
Non-production sandboxes MAY use experimental models with no production data.

**Compliance Requirements**  
AI features MUST document model IDs, data flows, and validation steps in security review.

**Violation Consequences**  
Feature freeze; security incident if production data leaked to unapproved model.

---

## 7. Documentation Principles

**Purpose**  
Ensure documentation keeps pace with system complexity.

**Rule**  
Every specification MUST be traceable to SRS FR/NFR or PC section. Documents MUST use approved folder order (`README.md` dependency chain). Locked SRS MUST NOT be edited without formal change request. Documentation MUST precede or accompany implementation, not follow months later.

**Rationale**  
Blueprint-first approach prevents the failures this program is designed to avoid.

**Examples**  
- API spec cites FR-070–074 before implementation.  
- Event catalog cites NFR-013 and SAD §19.

**Exceptions**  
Emergency production hotfix MAY ship with retroactive documentation within 5 business days.

**Compliance Requirements**  
Document approval MUST include traceability matrix update where applicable.

**Violation Consequences**  
Document status remains Draft; release blocked for affected scope.

---

## 8. Security Principles

**Purpose**  
Protect tenants, data, and operations (SRS §31, SAD §43–52).

**Rule**  
Security MUST be default-deny. Tenant isolation MUST be enforced at every layer. Secrets MUST NOT appear in logs, events, or context packages (NFR-007, FR-102). mTLS MUST protect service and agent channels (ADR-SAD-007). OWASP ASVS Level 2 MUST be minimum for external interfaces.

**Rationale**  
One tenant escape destroys platform trust permanently.

**Examples**  
- ABAC filter on every audit query (FR-121).  
- Task-scoped credentials via broker only (ADR-SAD-010).

**Exceptions**  
Break-glass access MAY bypass normal roles with MFA, time limit, enhanced audit, and second approver for production policy changes (SAD audit fix).

**Compliance Requirements**  
Annual pen test; continuous dependency scanning; threat model per major release.

**Violation Consequences**  
P0 incident; halt; executive notification; release embargo until remediated.

---

## 9. Reliability Principles

**Purpose**  
Ensure dependable operation (SRS §29, NFR-005).

**Rule**  
Systems MUST use at-least-once delivery with idempotent handlers. Critical paths MUST have N+1 redundancy. RPO MUST be ≤15 minutes; RTO ≤4 hours for control plane (enterprise tier). Chaos testing MUST run quarterly on scheduling and policy services.

**Rationale**  
Orchestration failures cascade across agents and tenants.

**Examples**  
- Dispatch idempotency keys prevent duplicate side effects.  
- Outbox pattern guarantees event publish with writes.

**Exceptions**  
Analytics projections MAY have longer RPO with documented stale-read indicators.

**Compliance Requirements**  
Reliability SLOs MUST be monitored; error budgets gate feature velocity.

**Violation Consequences**  
SLO breach triggers incident review; reliability work takes priority over features.

---

## 10. Performance Principles

**Purpose**  
Meet quantified latency targets (NFR-001, NFR-002, SRS §34).

**Rule**  
P95 API read latency MUST be ≤200ms at 1k RPS per region. P95 task dispatch MUST be ≤2s under nominal load. Policy evaluation SHOULD be ≤100ms P95. Performance optimizations MUST NOT skip authorization or validation.

**Rationale**  
Human approval workflows require responsive control plane.

**Examples**  
- CQRS projections serve dashboards; not primary OLTP.  
- Cache-aside for authz with short TTL.

**Exceptions**  
Bulk async exports (FR-083) MAY exceed read latency if job-based and documented.

**Compliance Requirements**  
Load tests at 2× GA target before release; performance budget per endpoint in CI.

**Violation Consequences**  
Release delay until SLO met or ADR accepts degraded tier with customer communication.

---

## 11. Scalability Principles

**Purpose**  
Support thousands of tenants and millions of tasks (SRS §28, SC-005, NFR-022).

**Rule**  
Architecture MUST shard by `tenant_id` (immutable once assigned). Stateless tiers MUST scale horizontally. Event backbone MUST be designed for ≥500k events/sec Year-5 capacity. No single tenant MUST monopolize shared pools without policy-defined priority.

**Rationale**  
Retrofitting sharding is higher risk than designing early.

**Examples**  
- Partitioned event topics by tenant hash.  
- Fair scheduling across tenants (SAD §61).

**Exceptions**  
Dedicated enterprise tenants MAY have isolated compute pools (ADR-SAD-005).

**Compliance Requirements**  
SC-005 load test evidence required before GA.

**Violation Consequences**  
Scale certification failed; GA blocked for affected capacity tier.

---

## 12. Maintainability Principles

**Purpose**  
Enable long-term evolution (NFR-011, SRS §35).

**Rule**  
Modules MUST have documented owners (SAD module table). Public contracts MUST use semantic versioning. Breaking API changes MUST provide ≥6 months deprecation notice. Orchestration core SHOULD target 80% unit test coverage on critical paths.

**Rationale**  
Team turnover and decade-long platform life require maintainable structure.

**Examples**  
- MOD-08 owned by Execution Domain team.  
- Deprecation warnings in API responses before removal.

**Exceptions**  
Security patches MAY break deprecated APIs with 30-day notice if actively exploited.

**Compliance Requirements**  
Service catalog MUST list owner and on-call per module.

**Violation Consequences**  
Undocumented modules MUST NOT accept production traffic.

---

## 13. Extensibility Principles

**Purpose**  
Grow ecosystem without core forks (NFR-012, SRS §36).

**Rule**  
Extensions MUST use plugin manifests: agents, gates, connectors, policies, notifications. Third-party extensions MUST pass certification before production (FR-034). Core orchestrator MUST NOT be forked per customer (CON-006).

**Rationale**  
Marketplace and enterprise customization depend on safe extension points.

**Examples**  
- Certified GitHub connector as MOD-15 plugin.  
- Project template encoding agent sequences (FR-002).

**Exceptions**  
Internal modules MUST dogfood same plugin contracts as external partners.

**Compliance Requirements**  
Compatibility matrix updated with every PM core release (FR-033).

**Violation Consequences**  
Uncertified plugins MUST NOT receive production dispatch.

---

## 14. Observability Principles

**Purpose**  
Enable triage and audit correlation (NFR-015, SAD §56–57).

**Rule**  
100% of dispatch paths MUST emit correlated trace IDs across edge, core, governance, and agents. Logs MUST be structured JSON with `tenant_id`, `correlation_id`, `service`. Metrics MUST follow platform naming catalog (SAD §58).

**Rationale**  
Autonomous incidents are undebuggable without end-to-end correlation.

**Examples**  
- Gateway injects correlation ID; dispatch preserves through agent ack.  
- Audit event shares correlation_id with trace span.

**Exceptions**  
High-volume debug logs MAY sample in production with errors always captured.

**Compliance Requirements**  
Synthetic dispatch trace test in staging CI MUST pass each release.

**Violation Consequences**  
Feature MUST NOT deploy to production without instrumentation.

---

## 15. Testing Principles

**Purpose**  
Verify requirements and constitutional compliance (SRS §11, SC-004).

**Rule**  
Tests MUST map to SRS FR/NFR IDs. Golden path end-to-end scenario MUST pass before GA (SC-004). Contract tests MUST cover all agent types and certified connectors. Tenant isolation MUST have automated integration tests each release.

**Rationale**  
Autonomous systems cannot be validated by manual testing alone.

**Examples**  
- Golden path: intent → plan → dispatch → gate → release.  
- Contract test: dispatch protocol schema round-trip.

**Exceptions**  
Experimental features behind feature flags MAY ship with limited test scope if not customer-accessible.

**Compliance Requirements**  
RTM MUST link tests to requirements for GA scope.

**Violation Consequences**  
Release certification failed; SC criteria not met.

---

## 16. Deployment Principles

**Purpose**  
Deploy safely across SaaS, Dedicated, and AirGapped profiles (ADR-SAD-005, CON-005).

**Rule**  
Deployments MUST use rolling zero-downtime for control plane. Profile selection MUST be configuration-driven, not code forks. Production deploys MUST pass all quality gates (FR-061). Air-gapped profiles MUST block external LLM routes (SAD §69 validation).

**Rationale**  
Enterprise SKUs require deployment diversity without engineering fragmentation.

**Examples**  
- Same container images; capability flags for air-gap.  
- Warm standby secondary region for enterprise DR.

**Exceptions**  
Emergency patches MAY use accelerated pipeline with enhanced monitoring.

**Compliance Requirements**  
Deployment runbooks in `12-Deployment/` and `15-Operations/` when authored.

**Violation Consequences**  
Rollback mandatory if gate evidence missing post-deploy.

---

## 17. Versioning Principles

**Purpose**  
Manage evolution without breaking tenants (SAD §89–90).

**Rule**  
APIs, events, agent manifests, and policies MUST use semantic versioning. Event schemas MUST maintain backward compatibility within major versions (NFR-013). Breaking changes MUST increment major version with migration guide and minimum 6-month deprecation (SRS §35).

**Rationale**  
Independent agent release cycles (CON-008) require explicit compatibility.

**Examples**  
- `event_type` v2 adds optional fields; v1 consumers ignore unknowns.  
- Agent manifest declares `min_pm_version`.

**Exceptions**  
Security vulnerabilities MAY force accelerated breaking changes with customer notice.

**Compliance Requirements**  
Schema registry MUST reject breaking event changes without major bump.

**Violation Consequences**  
Consumer outage → incident; compatibility test gap remediated.

---

## 18. Change Management Rules

**Purpose**  
Control how the program evolves.

**Rule**  
SRS changes MUST use formal change request with product + engineering approval. SAD structural changes MUST have ADR. Constitutional changes MUST follow Section 50 amendment rules. All changes MUST update Change-Log and Version-History.

**Rationale**  
Uncontrolled change destroys traceability and audit narrative.

**Examples**  
- CR-2026-014: new FR for portfolio export → SRS minor bump.  
- ADR-SAD-011: extract dispatch service → SAD reference update in new docs only.

**Exceptions**  
Typo fixes in non-locked docs MAY use patch version with single reviewer.

**Compliance Requirements**  
No merge to locked SRS without approved CR on record.

**Violation Consequences**  
Change reverted; process review with ARB.

---

## 19. ADR Governance

**Purpose**  
Record architectural decisions with provenance (SAD §5).

**Rule**  
Structural decisions MUST be captured in ADR files under `02-Architecture/ADR/`. ADR index MUST be updated in `00-Governance/ADR-Index.md`. ADRs MUST NOT contradict SRS. Superseded ADRs MUST be marked, not deleted.

**Rationale**  
Enterprise audits require decision history.

**Examples**  
- ADR-SAD-009 documents PEP placement.  
- Proposed ADR-GOV-001 if constitution location standardization needed.

**Exceptions**  
Reversible tactical choices MAY be documented in Decision-Log without full ADR if no cross-team impact.

**Compliance Requirements**  
ARB MUST review all new ADRs before Accepted status.

**Violation Consequences**  
Implementation without ADR for structural change MUST be rolled back or retroactive ADR within 10 days.

---

## 20. Requirement Traceability Rules

**Purpose**  
Link all work to SRS requirements (RTM-AIPM-001).

**Rule**  
Every blueprint document below SRS MUST cite FR/NFR/CON/SC IDs or justify N/A in RTM. Implementation MUST map features to requirements before GA. Untraceable requirements in specs are void and MUST NOT be implemented.

**Rationale**  
Regulated customers require demonstrable coverage.

**Examples**  
- Database doc cites FR-090, FR-120, NFR-018.  
- API doc maps endpoints to FR-001, FR-091.

**Exceptions**  
Internal tooling MAY be exempt if explicitly out of SRS scope with ARB note.

**Compliance Requirements**  
RTM updated with each approved specification.

**Violation Consequences**  
Document not approvable; feature excluded from release scope.

---

## 21. Naming Standards

**Purpose**  
Consistent identifiers across teams and systems.

**Rule**  
Use SRS glossary terms normatively (`00-Governance/Glossary.md`). IDs MUST follow patterns: `FR-`, `NFR-`, `CON-`, `SC-`, `MOD-`, `ADR-`, `PC` section. Metrics MUST use `aipm_{domain}_{metric}_{unit}` (SAD §58). UTC MUST be used for all stored timestamps (NFR-017).

**Rationale**  
Shared vocabulary prevents integration defects.

**Examples**  
- `TaskDispatched` event, not `task_sent`.  
- `tenant_id` everywhere, not `orgId` in one service.

**Exceptions**  
External system field mappings MAY use connector ACL aliases with documented mapping table.

**Compliance Requirements**  
Linting for metric and event names in CI when schemas exist.

**Violation Consequences**  
Schema rejection; rename before merge.

---

## 22. Folder Standards

**Purpose**  
Preserve blueprint order and discoverability.

**Rule**  
Documents MUST reside in numbered folders per `README.md` (bundle v1.2.0). **Business Capability Model (`04`) MUST be approved before Domain Model (`05`).** Constitution MUST reside in `00-Governance/Project-Constitution.md`. State machines MUST be documented in `07-State-Machines/`, not embedded only in domain model. Folder renumbering MUST have ARB approval and Change-Log entry.

**Rationale**  
Dependency order prevents downstream contradictions.

**Examples**  
- Events catalog in `06-Events/`, not `04-Database/`.  
- ADRs only in `02-Architecture/ADR/`.

**Exceptions**  
Shared assets MAY live in `Assets/` with cross-references.

**Compliance Requirements**  
New documents MUST NOT be placed in wrong tier folders.

**Violation Consequences**  
Document relocated; approval withheld until correct location.

---

## 23. Coding Standards Governance

**Purpose**  
Set high-level code quality law without language-specific rules here.

**Rule**  
Code MUST match domain language from glossary. Business logic MUST NOT live in integration adapters. Control plane MUST NOT contain customer application code paths (CON-001). Error types MUST be explicit with correlation IDs. Language choices MUST follow ADR-008 unless new ADR approved.

**Rationale**  
Implementation details belong in `13-Implementation/`; constitution sets boundaries.

**Examples**  
- Adapter translates GitHub webhook → domain event only.  
- No SQL migrations for customer app tables in PM repo.

**Exceptions**  
Language-specific style guides MAY be added in `13-Implementation/` without constitutional amendment.

**Compliance Requirements**  
Static analysis MUST block forbidden imports across DU boundaries.

**Violation Consequences**  
PR blocked; security review for boundary violations.

---

## 24. Documentation Standards

**Purpose**  
Uniform, professional blueprint documents.

**Rule**  
Documents MUST have: one H1, metadata block (ID, version, status, date), References section, and status keyword. Markdown MUST use relative links. Secrets and customer data MUST NOT appear in docs. WCAG 2.1 AA applies to admin UI docs where applicable (NFR-014).

**Rationale**  
Readable docs reduce onboarding cost and errors.

**Examples**  
- Status: `APPROVED` in header.  
- Link: `../01-SRS/SRS-AI-Project-Manager.md`.

**Exceptions**  
Draft watermarks MAY be used during review only.

**Compliance Requirements**  
Documentation review checklist before approval (Section 44).

**Violation Consequences**  
Return to author for revision; not approvable.

---

## 25. Diagram Standards

**Purpose**  
Consistent visual architecture communication.

**Rule**  
Diagrams SHOULD be stored in `02-Architecture/Architecture-Diagrams/` or `Assets/`. Diagrams MUST include version, date, and source document ID. Context maps MUST align with SAD §12–13. MUST NOT depict agent-to-agent direct coordination (PC architectural law).

**Rationale**  
Diagrams drift from truth without versioning.

**Examples**  
- `context-map-v1.svg` referenced from domain model doc.  
- C4 Level 2 per deployable unit.

**Exceptions**  
Informal whiteboard photos MAY be used in drafts, not as approved artifacts.

**Compliance Requirements**  
Approved diagrams MUST be linked from parent specification.

**Violation Consequences**  
Diagram not cited as authoritative; revision required.

---

## 26. API Design Principles

**Purpose**  
Guide `07-APIs/` without designing APIs here.

**Rule**  
APIs MUST be versioned, tenant-scoped, idempotent where mutating, and fail with consistent error taxonomy (SAD §26). External traffic MUST pass through Edge DU only (SAD §87). Authorization MUST be evaluated at gateway and service layers. Rate limits MUST apply per tenant tier.

**Rationale**  
APIs are long-lived contracts with enterprise customers.

**Examples**  
- `Idempotency-Key` header on dispatch commands.  
- 403 returns policy_id, not internal stack trace.

**Exceptions**  
Internal gRPC MAY use separate versioning with mTLS only.

**Compliance Requirements**  
API specs MUST trace to FR groups before implementation.

**Violation Consequences**  
Public API not releasable without spec approval and security review.

---

## 27. Database Design Principles

**Purpose**  
Guide `05-Database/` without designing schemas here.

**Rule**  
Authoritative writes MUST be ACID (NFR-018). Tenant data MUST be shardable by `tenant_id`. Region pinning MUST be enforceable (FR-120). Audit stores MUST be append-only. Knowledge graph MAY use relational authoritative + projection (ADR-SAD-003). Secrets MUST NOT be stored in application tables.

**Rationale**  
Data architecture errors are expensive to fix at scale.

**Examples**  
- Separate event log from OLTP projections.  
- Legal hold flag blocks deletion (FR-103).

**Exceptions**  
Analytics stores MAY be eventually consistent with documented lag.

**Compliance Requirements**  
Data classification MUST be documented per entity family.

**Violation Consequences**  
Schema review rejection; migration plan required.

---

## 28. Agent Design Principles

**Purpose**  
Bind all specialist agents (SRS §12, SAD §28–33, PC Article VII equivalent).

**Rule**  
Agents MUST register via manifest (FR-030). Agents MUST NOT coordinate directly with other agents. Agents MUST acknowledge dispatch within 30s P95 (NFR-023). Outputs MUST be schema-validated before success (NFR-019). Agents MUST report usage for cost accounting (FR-082). Only certified compatible versions MUST receive dispatch (FR-033, FR-118).

**Rationale**  
Agents are data-plane workers under PM law.

**Examples**  
- Backend agent returns artifacts by reference URI.  
- Uncertified agent version blocked at scheduler.

**Exceptions**  
Mock agents MAY be used in test harnesses without certification.

**Compliance Requirements**  
Certification pipeline MUST pass before registry `Registered` status.

**Violation Consequences**  
Agent decertified; dispatches halted until remediated.

---

## 29. Workflow Design Principles

**Purpose**  
Guide `10-Workflow/` and lifecycles (SAD §27, §65).

**Rule**  
Workflows MUST use explicit state machines for Project, Task, and Agent lifecycles (SRS §26). Replan MUST create immutable plan versions (FR-021). Halt MUST supersede scheduling (SC-008). Human wait steps MUST survive restarts via durable adapter (ADR-SAD-002). Workflows MUST NOT bypass policy PEPs.

**Rationale**  
Implicit workflow logic causes audit and safety failures.

**Examples**  
- `PlanVersionActivated` → scheduler consumes event.  
- Approval timeout escalates per policy (FR-052).

**Exceptions**  
BPMN export MAY be offered later; not source of truth at GA.

**Compliance Requirements**  
State transition tables MUST match SRS normative enums.

**Violation Consequences**  
Workflow change blocked if enum mismatch with SRS.

---

## 30. Memory Design Principles

**Purpose**  
Guide `09-Memory/` (SAD §70–73).

**Rule**  
PM MUST assemble context per dispatch from authoritative state and knowledge graph (FR-101). PM MUST NOT rely on open-ended LLM conversation memory for orchestration. PII and secrets MUST be redacted from context (FR-102). Cache MUST NOT be treated as authoritative.

**Rationale**  
Context drift is a primary agent failure mode (SRS §6).

**Examples**  
- ContextPackage hash recorded in audit.  
- Oversized context truncated per agent manifest budget.

**Exceptions**  
Derived summaries MAY be cached with content-hash deduplication.

**Compliance Requirements**  
Context assembly rules MUST be per agent type in manifest.

**Violation Consequences**  
Dispatch blocked if redaction rules fail.

---

## 31. Event Design Principles

**Purpose**  
Guide `06-Events/` (SAD §19–21, ADR-SAD-004).

**Rule**  
State changes MUST emit versioned domain events. Events MUST use outbox pattern with writes. Events MUST NOT contain secrets. Consumers MUST be idempotent. Event backbone MUST support partition by `tenant_id`. Audit events MUST be reconstructable (SC-003).

**Rationale**  
Events are the nervous system of orchestration and compliance.

**Examples**  
- Envelope: `event_id`, `tenant_id`, `correlation_id`, `schema_version`.  
- Projectors rebuild read models from stream.

**Exceptions**  
High-volume telemetry MAY use parallel pipeline with lower retention.

**Compliance Requirements**  
Event catalog MUST be published before consumer implementation.

**Violation Consequences**  
Consumer MUST NOT deploy without schema registration.

---

## 32. Error Handling Principles

**Purpose**  
Classify and recover from failures (SAD §41, §63–64).

**Rule**  
Errors MUST be classified: transient, permanent, security, policy. Transient errors MUST use exponential backoff with max retries (FR-040). Security errors MUST NOT retry. Permanent failures MUST route to DLQ with remediation workflow (FR-044). Policy errors MUST fail closed.

**Rationale**  
Retry storms and silent drops are operational risks for autonomous systems.

**Examples**  
- Agent timeout → retry then DLQ.  
- Policy deny → no retry, audit entry.

**Exceptions**  
Emergency operator override MAY requeue DLQ items with audit.

**Compliance Requirements**  
Error taxonomy MUST be consistent across services.

**Violation Consequences**  
Undefined error handling blocks production promotion.

---

## 33. Logging Principles

**Purpose**  
Operational logging distinct from audit (SAD §54).

**Rule**  
Logs MUST be structured JSON with correlation_id. Logs MUST NOT contain secrets or raw PII (NFR-007, NFR-008). Operational retention MAY be 30–90 days; audit retention follows tenant policy (FR-094). Security events MUST always be logged.

**Rationale**  
Logs support MTTR; audit supports compliance.

**Examples**  
- `level=error tenant_id=… correlation_id=…`.  
- Pseudonymized user IDs in access logs.

**Exceptions**  
Debug sampling MAY reduce volume except for error level.

**Compliance Requirements**  
Log scrubbing at collector edge for known PII patterns.

**Violation Consequences**  
Log pipeline blocked if secret detected; incident if leaked.

---

## 34. Monitoring Principles

**Purpose**  
Detect SLO breaches and anomalies (SAD §55, §59, FR-084).

**Rule**  
RED metrics MUST exist per service. Business metrics MUST include tasks/hour, gate pass rate, approval latency. Anomaly detection SHOULD monitor failure and cost rates (FR-084). Meta-monitoring MUST detect collector failures.

**Rationale**  
99.9% availability requires measurement (NFR-004).

**Examples**  
- Alert on projection lag > threshold.  
- Budget 95% alert before hard stop (SC-006).

**Exceptions**  
Sandbox tenants MAY have reduced alerting.

**Compliance Requirements**  
SLO dashboards MUST exist before GA.

**Violation Consequences**  
Production promotion blocked without monitoring coverage.

---

## 35. Privacy Principles

**Purpose**  
Protect personal data (SRS §32, NFR-008, NFR-010).

**Rule**  
Data minimization MUST apply to agent context and analytics. GDPR data subject requests MUST be supported within 30 days (NFR-010). CMK MAY be offered for enterprise tier. Deletion MUST respect legal hold (FR-103). Subprocessors MUST be documented for DPA.

**Rationale**  
EU and enterprise procurement require privacy by design.

**Examples**  
- Field-level masking in exports.  
- DSR workflow queues deletion across stores.

**Exceptions**  
Anonymized aggregate benchmarks MAY be opt-in cross-tenant (future roadmap).

**Compliance Requirements**  
DPIA for high-risk processing before feature GA.

**Violation Consequences**  
Feature not shippable in EU without remediation; regulatory escalation.

---

## 36. Human Oversight Principles

**Purpose**  
Preserve enterprise control (ADR-003, SRS §42).

**Rule**  
Humans MUST retain configurable authority over production and irreversible actions. Approvals MUST enforce segregation of duties (FR-053). Emergency halt MUST be available to authorized operators (FR-043, SC-008). Explainability records MUST accompany plan and production gate decisions (FR-119).

**Rationale**  
Fully unchecked autonomy is unacceptable for enterprise buyers (OOS-010).

**Examples**  
- Production deploy requires Project Owner approval.  
- Compliance Officer optional per policy template.

**Exceptions**  
Non-production environments MAY allow higher autonomy levels per tenant policy.

**Compliance Requirements**  
Approval authority matrix MUST be documented and tested.

**Violation Consequences**  
Autonomy policy MUST be tightened; feature disabled until gates verified.

---

## 37. AI Decision-Making Principles

**Purpose**  
Bound how AI influences outcomes.

**Rule**  
AI decisions MUST be recorded with inputs hash and policy version. Raw chain-of-thought MUST NOT be stored as official explainability (SRS Definitions). AI MUST NOT execute irreversible actions without PEP success. Anomalous outputs MUST trigger halt or human review.

**Rationale**  
Auditors and customers require accountable decisions, not opaque model internals.

**Examples**  
- ExplainabilityRecord lists rule outcomes and requirement IDs.  
- Planner suggests tasks; policy engine inserts mandatory security tasks.

**Exceptions**  
Research environments MAY log additional detail with access controls.

**Compliance Requirements**  
AI decision records MUST be exportable in compliance bundles (FR-083).

**Violation Consequences**  
Feature cannot be marketed as enterprise-ready until fixed.

---

## 38. Approval Gates

**Purpose**  
Define human authorization checkpoints.

**Rule**  
Plan baseline MUST be approved by Project Owner (default). Production promotion MUST pass human gate per ADR-003. Policy changes MUST require Organization Admin. Waivers MUST have approver and expiry (FR-063). Multi-step approvals MUST support timeout and delegation (FR-052).

**Rationale**  
Gates translate constitutional human oversight into process.

**Examples**  
- `ApprovalGranted` event unblocks dispatch.  
- Waiver expires → gate re-evaluates failed.

**Exceptions**  
Dev/staging promotion MAY auto-pass if tenant policy allows.

**Compliance Requirements**  
Gate configuration MUST be template-driven, not hardcoded per customer fork.

**Violation Consequences**  
Unauthorized promotion → incident P0; rollback required.

---

## 39. Quality Gates

**Purpose**  
Objective criteria before environment promotion (FR-060–062).

**Rule**  
Promotion MUST be blocked until configured gates pass: tests, security scans, documentation coverage. Gate evidence MUST be stored with validator versions (FR-062). Failed gates MUST NOT be bypassed without waiver workflow.

**Rationale**  
Autonomous agents otherwise ship defects to production.

**Examples**  
- CI test results ingested as gate input (FR-074).  
- Security scan threshold from tenant policy.

**Exceptions**  
Emergency waiver requires approver, reason, expiry, audit.

**Compliance Requirements**  
Gate definitions MUST be versioned and traceable to FR-060.

**Violation Consequences**  
Release invalid; compliance evidence rejected.

---

## 40. Definition of Ready

**Purpose**  
Criteria before work begins on a specification or feature.

**Rule**  
Work is Ready when: (a) parent documents approved; (b) SRS FR/NFR traced; (c) constitutional sections identified; (d) applicable ADRs listed; (e) security classification assigned; (f) owner and reviewers named; (g) out-of-scope explicitly stated.

**Rationale**  
Prevents premature implementation and rework.

**Examples**  
- Domain model work Ready after PC + BCM + SAD §11–16 review.  
- API spec Ready after FR-070–074 mapped.

**Exceptions**  
Spikes MAY proceed with time-box and documented assumptions if not production-bound.

**Compliance Requirements**  
Ready checklist signed in review ticket.

**Violation Consequences**  
Work not started in official sprint; no merge to main for production paths.

---

## 41. Definition of Done

**Purpose**  
Criteria before work is complete.

**Rule**  
Done when: (a) specification or code meets PC + SRS + SAD; (b) tests pass including contract/golden path where applicable; (c) observability instrumented; (d) documentation updated; (e) RTM updated; (f) security review complete if required; (g) no open P0/P1 defects; (h) Change-Log updated if blueprint changed.

**Rationale**  
Consistent Done prevents partial delivery into production.

**Examples**  
- Event catalog Done includes schema registry entry and consumer idempotency notes.  
- Feature Done includes dashboard metrics.

**Exceptions**  
Known P2 defects MAY ship with documented backlog if ARB accepts risk.

**Compliance Requirements**  
Done checklist in PR or document approval record.

**Violation Consequences**  
Not releasable; status reverts to In Progress.

---

## 42. Architecture Review Rules

**Purpose**  
Govern ARB decisions.

**Rule**  
ARB MUST review: structural changes, new ADRs, constitutional interpretations, cross-DU contracts, and autonomy policy changes. Security architect MUST have veto on tenancy and credential design. ARB MUST meet within 5 business days for constitutional disputes.

**Rationale**  
Centralized architecture prevents fragmentation.

**Examples**  
- Extract dispatch microservice → ARB + ADR required.  
- New PEP location → ARB mandatory.

**Exceptions**  
Bug fixes preserving architecture MAY skip ARB with architect ack.

**Compliance Requirements**  
ARB minutes recorded in Decision-Log.

**Violation Consequences**  
Unauthorized structural merge reverted.

---

## 43. Code Review Rules

**Purpose**  
High-level code review law.

**Rule**  
Every production PR MUST have: (a) one domain reviewer; (b) security reviewer for auth, crypto, tenancy; (c) traceability to FR/NFR or ticket; (d) tests; (e) no decrease in observability; (f) constitutional compliance for autonomy features.

**Rationale**  
Code review is last human gate before runtime.

**Examples**  
- Tenancy filter missing → review rejection.  
- New dispatch path without trace → rejection.

**Exceptions**  
Docs-only and test-only PRs MAY use lighter review per team policy.

**Compliance Requirements**  
Review checklist enforced in PR template.

**Violation Consequences**  
Merge blocked; repeat issues trigger team training.

---

## 44. Documentation Review Rules

**Purpose**  
Quality gate for blueprint documents.

**Rule**  
Document reviews MUST verify: metadata complete, references valid, traceability, constitutional compliance, no contradiction with SRS/SAD, glossary terms used correctly, status keyword set.

**Rationale**  
Bad docs cause bad implementations.

**Examples**  
- Missing PC reference → return to author.  
- Invented requirement without FR → reject.

**Exceptions**  
Draft reviews MAY be informal until In Review status.

**Compliance Requirements**  
Two-person rule for APPROVED status on tier-1 docs (SRS, SAD, PC, domain model).

**Violation Consequences**  
Document remains Draft; not listed in README status table.

---

## 45. Security Review Rules

**Purpose**  
Security gate for high-risk changes.

**Rule**  
Security MUST review: APIs, agent contracts, auth changes, credential flows, data residency, third-party plugins, and autonomy policies before approval. Pen test MUST pass before GA (SC-002). Threat model MUST be updated per major release.

**Rationale**  
Autonomous agents amplify security impact.

**Examples**  
- New OAuth scope in connector → security review.  
- Agent manifest requesting broad permissions → rejected.

**Exceptions**  
Purely internal refactors with no boundary change MAY use automated SAST only.

**Compliance Requirements**  
Security sign-off recorded in approval ticket.

**Violation Consequences**  
Release embargo; P0 if vulnerability in production.

---

## 46. Testing Review Rules

**Purpose**  
Validate test adequacy.

**Rule**  
Testing review MUST confirm: golden path coverage (SC-004), load test for scale claims (SC-005), tenant isolation tests, contract tests for agents/connectors, and regression for constitutional PEPs.

**Rationale**  
Untested autonomy is negligence at enterprise scale.

**Examples**  
- SC-005 evidence attached to GA checklist.  
- Halt test proves 30s stop (SC-008).

**Exceptions**  
Documentation-only changes MAY skip test review.

**Compliance Requirements**  
QA sign-off on release certification record.

**Violation Consequences**  
GA blocked; success criteria not met.

---

## 47. Release Review Rules

**Purpose**  
Final gate before production release.

**Rule**  
Release MUST verify: all SC criteria for scope, error budget healthy, migrations tested, rollback plan documented, status page updated, customer comms for breaking changes, compliance exports functional (CON-007).

**Rationale**  
Releases affect all tenants; discipline prevents incidents.

**Examples**  
- GA checklist SC-001 through SC-008 signed.  
- Rollback drill within last quarter for enterprise tier.

**Exceptions**  
Hotfix MAY compress review with incident commander approval.

**Compliance Requirements**  
Release record in Change-Log.

**Violation Consequences**  
Release rolled back; postmortem mandatory.

---

## 48. Backward Compatibility Rules

**Purpose**  
Protect existing tenants and integrations.

**Rule**  
Minor versions MUST be backward compatible. Agent compatibility matrix MUST be honored (FR-033). Event consumers MUST tolerate unknown fields. Removal requires deprecation period unless security emergency.

**Rationale**  
CON-008 requires independent agent releases without breaking tenants.

**Examples**  
- PM 1.1 works with Agent 2.3 per matrix; not 2.0 retired.  
- API v1 supported 6 months after v2 GA.

**Exceptions**  
Security CVE MAY shorten deprecation with documented notice.

**Compliance Requirements**  
Compatibility tests in CI for supported matrix cells.

**Violation Consequences**  
Customer incident; rollback and patch required.

---

## 49. Deprecation Policy

**Purpose**  
Sunset features and versions safely.

**Rule**  
Deprecations MUST provide minimum 6 months notice for external APIs and agent protocols (SRS §35). Deprecated items MUST log warnings. Removal MUST coincide with major version or security ADR. ADRs MUST be marked Superseded, not deleted.

**Rationale**  
Enterprise contracts assume predictable change windows.

**Examples**  
- `Sunset` header on deprecated API responses.  
- Agent type `Deprecated` in registry before `Retired`.

**Exceptions**  
Internal unused modules MAY deprecate faster with team-only impact.

**Compliance Requirements**  
Deprecation calendar published in `16-Roadmap/`.

**Violation Consequences**  
Forced rollback if customer broken without notice.

---

## 50. Long-Term Evolution Policy

**Purpose**  
Guide growth without violating invariants (SRS §43–44, SAD §98).

**Rule**  
Evolution MUST preserve constitutional invariants: control/data separation, PEP fail-closed, tenant isolation, no PM codegen, event auditability. New capabilities MUST use extension points. Cell-based multi-region and marketplace MAY proceed per roadmap with ADRs. Federated PM and active-active regions require ARB and major PC version if invariants affected.

**Rationale**  
Long-term vision must not erode foundational law.

**Examples**  
- Marketplace agents via certification, not core fork.  
- Year-5 event rate via sharding, not single-node scale-up.

**Exceptions**  
None for invariant preservation.

**Compliance Requirements**  
Annual constitution compliance audit against codebase and docs.

**Violation Consequences**  
Roadmap item blocked until ADR and constitutional review complete.

---

## Amendment process

| Article class | Approval required | Version bump |
|---------------|-------------------|--------------|
| Non-immutable PC sections | ARB supermajority (75%) | Minor or patch |
| Immutable core (Sections 2, 3, 8 tenant escape, 28 agent coordination ban) | Unanimous ARB + Security + Product | Major |
| SRS conflict | Constitution MUST be amended to align with SRS, not vice versa without SRS CR |

Amendments MUST update Change-Log, Version-History, and Decision-Log. Informal amendments are void.

---

## PROJECT CONSTITUTION CHECKLIST

Every future document MUST pass this checklist before **APPROVED** status:

| # | Check | Pass criteria |
|---|-------|---------------|
| C-01 | Parent documents approved | Dependency chain satisfied; **04 before 05** per README v1.2.0 |
| C-02 | PC-AIPM-001 referenced | Listed in References with version |
| C-03 | SRS traceability | All behaviors map to FR/NFR/CON/SC or N/A justified |
| C-04 | SAD alignment | Modules, DUs, flows consistent with SAD |
| C-05 | ADR compliance | Structural choices have Accepted ADR |
| C-06 | Vision protection | Not a code generator, chat bot, or scope violation |
| C-07 | Control/data plane | PM does not execute application work |
| C-08 | Agent rules | No direct agent-to-agent coordination |
| C-09 | Policy PEPs | Schedule, Dispatch, Gate fail-closed preserved |
| C-10 | Tenancy | `tenant_id` isolation explicit |
| C-11 | Region/residency | FR-120 respected if data location relevant |
| C-12 | Audit | State changes auditable; no secrets in logs/events |
| C-13 | Human oversight | Production paths have approval/gate story |
| C-14 | Halt and credentials | SC-008 / FR-117 respected if execution relevant |
| C-15 | Cost/budget | NFR-020 / SC-006 if resource consumption relevant |
| C-16 | Naming | Glossary terms and ID patterns used |
| C-17 | Folder location | Correct numbered blueprint folder |
| C-18 | Version metadata | Document ID, version, status, date complete |
| C-19 | No SRS contradiction | Verified against locked SRS |
| C-20 | No SAD contradiction | Verified against approved SAD |
| C-21 | Security classification | Assigned if handling auth, data, or agents |
| C-22 | Definition of Ready | Met before authoring began |
| C-23 | Definition of Done | Will be met before implementation GA |
| C-24 | Diagram standards | If diagrams included, versioned and sourced |
| C-25 | Change records | Change-Log updated if governance impact |

**Approval rule:** ALL applicable checks MUST pass. N/A requires explicit justification in review record.

---

## FINAL VALIDATION

### Validation method

Each PC section was checked against SRS-AIPM-001, SAD-AIPM-001, ADR-001–008, ADR-SAD-001–010, Vision.md, Project-Charter.md, and Glossary.md.

### Conflict scan results

| Check | Result |
|-------|--------|
| PC vs SRS CON-001 (no PM codegen) | **PASS** — Sections 2, 23, 28 |
| PC vs FR-117 / SC-008 (halt/credentials) | **PASS** — Sections 29, 38, checklist C-14 |
| PC vs FR-090 / FR-120 / FR-121 (tenancy/residency/audit) | **PASS** — Sections 8, 27, 35 |
| PC vs ADR-SAD-009 (PEP fail-closed) | **PASS** — Sections 6, 37, 39 |
| PC vs ADR-007 (no agent mesh) | **PASS** — Sections 25, 28 |
| PC vs ADR-SAD-001 (five DUs) | **PASS** — Section 3 |
| PC vs ADR-004 (model router) | **PASS** — Section 6 |
| PC vs CON-006 (no core forks) | **PASS** — Sections 5, 13, 22 |
| PC vs NFR-019 (validation before trust) | **PASS** — Sections 6, 28 |
| PC internal consistency | **PASS** — Precedence clarifies PC operationalizes without contradicting SRS |

### Identified tension (resolved without changing locked documents)

| Tension | Resolution |
|---------|------------|
| Constitution location: `00-Governance/` vs `03-Project-Constitution/` folder in README | **Resolved:** Authoritative constitution is `00-Governance/Project-Constitution.md` per this prompt. Folder `03-Project-Constitution/` is organizational reserved space; governance master copy lives in `00-Governance`. **Proposed ADR-GOV-001** (not yet filed): *Standardize PC-AIPM-001 canonical path as 00-Governance/Project-Constitution.md* — does not modify locked SRS/SAD. |

### ADR proposals (no locked document modified)

| Proposed ADR | Title | Reason |
|--------------|-------|--------|
| ADR-GOV-001 | Canonical constitution path in 00-Governance | Align README folder `03` with governance master document location |

### Validation iterations

| Pass | Issues found | Resolution |
|------|--------------|------------|
| 1 | Location ambiguity vs README `03-Project-Constitution/` | Documented; ADR-GOV-001 proposed |
| 2 | Full section coverage 1–50 | All sections present with required fields |
| 3 | SRS/SAD contradiction scan | Zero conflicts |
| 4 | Checklist completeness | 25 checks defined |
| 5 | Final | **Zero conflicts remain** |

---

**PROJECT CONSTITUTION STATUS:**  
**APPROVED**

---

**END OF PC-AIPM-001 v1.0.0**
