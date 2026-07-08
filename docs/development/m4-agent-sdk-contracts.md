# M4 — Agent SDK & Contracts

**Status:** Complete  
**Dependencies:** M2

## Objective

Expose a stable, versioned agent-type catalog contract with manifest-based discovery.

## Acceptance criteria (met)

| Criterion | Evidence |
|-----------|----------|
| Agent SDK contract stub exists | `AIPM.Application/Runtime/Contracts/AgentTypeContract.cs` |
| Manifest scanner provides discovered types | `IPluginLoader.LoadAsync()` called by catalog endpoint |
| `GET /api/v1/agent-types` available | Host endpoint returns schema version + catalog |
| Sample manifest discovered | `echo-agent` returned from `plugins/echo-agent/manifest.json` |
| Contract tests pass | `AgentTypeContractMapperTests`, `ApiV1EndpointTests.AgentTypes_ReturnsCatalog` |

## API

| Endpoint | Description |
|----------|-------------|
| `GET /api/v1/agent-types` | Returns `{ schemaVersion, agentTypes[] }` |

## Explicitly not included

- AI provider integrations
- Business aggregates or business workflows
- Agent dispatch redesign (M2 runtime remains intact)
