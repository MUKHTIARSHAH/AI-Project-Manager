# Coding Standards — Phase 1

**Document ID:** STD-CODE-PH1-001  
**Version:** 1.0.0  
**Date:** 2026-07-07  
**Status:** APPROVED  
**Parent:** [Implementation-Architecture-Phase1.md](Implementation-Architecture-Phase1.md)

## Traceability

Every change must cite Blueprint IDs (CAP, CON, AGG, CMD, EVT) or IAD section. New aggregates require ADR (ADR-GOV-007).

## Languages (ADR-TECH-001 — LOCKED)

| Area | Language |
|------|----------|
| Control plane | C# / .NET 9 / ASP.NET Core 9 |
| Admin UI | TypeScript / Next.js |
| Analytics worker | C# (preferred) or isolated Python utilities |

Supersedes ADR-008 / IAD Go references for runtime.

## Go

- `gofmt`, `golangci-lint` required
- `context.Context` first parameter
- No panic in request/command paths
- Domain packages: no infrastructure imports
- Table-driven tests; prefer `testify` assertions

## Pull requests

- `make verify` green
- Conventional commit messages
- No secrets, `.env`, or credentials in diff

## AI agents

- Read locked Blueprint before coding
- Do not invent domain concepts
- Run architecture linter before completion
