# Deployment

## Local (Docker Compose)

```powershell
.\scripts\dev-up.ps1
```

Services:

| Service | Port |
|---------|------|
| PostgreSQL 16 | 5432 |
| Redis 7 | 6379 |
| RabbitMQ | 5672 (AMQP), 15672 (management) |

Compose file: `deploy/docker/docker-compose.yml`

## Production

Kubernetes manifests planned under `deploy/kubernetes/` (Phase 1 stubs per IAD).

Cloud profile: Azure primary (ADR-TECH-001).
