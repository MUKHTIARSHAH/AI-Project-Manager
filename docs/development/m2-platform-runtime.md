# M2 — Platform Runtime

**Status:** Complete  
**Vertical slice:** EchoAgent (no AI)

## Demonstration

```powershell
cd src
dotnet run --project AIPM.Host
```

On Development startup the host:

1. Discovers `plugins/echo-agent/manifest.json`
2. Registers EchoAgent in the agent registry
3. Sends `ExecuteAgentCommand` via MediatR command bus
4. Runs workflow runtime
5. EchoAgent returns `Hello`
6. Publishes `AgentCompletedEvent`
7. Logs structured output via Serilog

Manual trigger: `POST /api/v1/runtime/demo/echo`

## Components delivered (priority order)

| # | Component | Location |
|---|-----------|----------|
| 1 | Execution context | `AIPM.SharedKernel/Execution/` |
| 2 | Plugin loader | `AIPM.Plugins/Loading/`, `Manifests/` |
| 3 | Agent registry | `AIPM.Plugins/Agents/`, `BuiltIn/EchoAgent.cs` |
| 4 | Command bus | MediatR + pipeline behaviors in `AIPM.Application/Runtime/Pipeline/` |
| 5 | Event dispatcher | `AIPM.Infrastructure/Events/` (+ dead-letter port stub) |
| 6 | Workflow runtime | `AIPM.Workflow/Runtime/` |
| 7 | Background worker host | `AIPM.Infrastructure/Workers/` |
| 8 | Resilience | `AIPM.Infrastructure/Resilience/` (Polly retry/timeout/circuit breaker) |

## Design decisions

- **Ports in Application, adapters in Plugins/Workflow/Infrastructure** — preserves Clean Architecture.
- **Built-in EchoAgent** — proves orchestration without LLM; external assemblies deferred.
- **In-memory event dispatcher** — MassTransit remains for integration events; platform events use in-process dispatcher with retry + dead-letter port.
- **BuildingBlocks folder** — deferred; see `docs/architecture/building-blocks-roadmap.md`.

## Explicitly not included

- No OpenAI / Anthropic / Gemini / Cursor integration
- No business aggregates (User, Project, Task)
- RFC 7807 Problem Details → M3
