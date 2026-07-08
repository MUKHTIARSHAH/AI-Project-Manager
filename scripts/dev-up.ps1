# Start local infrastructure for AIPM development
param(
    [switch]$Down
)

$composeFile = Join-Path $PSScriptRoot "..\deploy\docker\docker-compose.yml"

if ($Down) {
    docker compose -f $composeFile down
    exit $LASTEXITCODE
}

docker compose -f $composeFile up -d
Write-Host "PostgreSQL: localhost:5432 | Redis: localhost:6379 | RabbitMQ: localhost:5672 (mgmt: 15672)"
