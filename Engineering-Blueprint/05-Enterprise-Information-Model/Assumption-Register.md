# Assumption Register — EIM-AIPM-001

| ASS ID | Description | Why assumed | Evidence | Risk if wrong | Validation plan | Status |
|--------|-------------|-------------|----------|---------------|-----------------|--------|
| ASS-001 | One Project belongs to exactly one Workspace | Simplifies tenancy and RBAC | FR-001, BCM CAP-001 | Cross-workspace reporting breaks | Confirm with enterprise PM interviews | Validated |
| ASS-002 | Program is optional; Project may roll up directly to Portfolio | Flexibility for mid-market | ADR-GOV-005, IDL-001 | Executive reporting gaps | Portfolio dashboard prototype | Open |
| ASS-003 | Initiative is optional sub-structure under Project only | Avoid over-modeling small projects | BCM, SRS scope | Large programs need more hierarchy | Year-2 roadmap review | Open |
| ASS-004 | ExternalWorkItem sync is bidirectional for status; AIPM authoritative for orchestration | ADR-005 | ADR-005 accepted | Sync conflicts with ITSM | Integration conformance tests | Validated |
| ASS-005 | Artifact metadata in AIPM; binary content in external VCS/object store | Control plane does not host app code | CON-001, PC §2 | Storage cost in PM | Architecture spike in `09-Database` | Open |
| ASS-006 | 64 concepts sufficient for GA vocabulary | Covers CAP-001–056 | Traceability matrix | Missing concept forces ADR | DDD authoring review | Open |
| ASS-007 | English canonical names; i18n at presentation layer | NFR-017 | SRS NFR-017 | Localized term drift | UI i18n strategy in `16` | Validated |
| ASS-008 | Deployment (CON-047) means customer app deploy, not AIPM platform deploy | Disambiguate integration vs folder 15 | User guidance, IDL needed | Term collision in ops docs | Glossary enforcement QG-06 | Validated |
