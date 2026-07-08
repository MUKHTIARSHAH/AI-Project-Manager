param(
    [string]$HostName = "127.0.0.1",
    [int]$Port = 5432,
    [string]$AdminUser = "postgres",
    [string]$DatabaseName = "aipm_dev",
    [string]$Password = $env:AIPM_POSTGRES_PASSWORD
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($Password)) {
    Write-Error @"
PostgreSQL password is required. Set the AIPM_POSTGRES_PASSWORD environment variable or pass -Password.

Example:
  `$env:AIPM_POSTGRES_PASSWORD = 'your-postgres-password'
  .\scripts\provision-aipm-dev-db.ps1
"@
}

$adminConnection = "Host=$HostName;Port=$Port;Database=postgres;Username=$AdminUser;Password=$Password;Timeout=15"
$identityConnection = "Host=$HostName;Port=$Port;Database=$DatabaseName;Username=$AdminUser;Password=$Password;Timeout=15"

$repoRoot = Split-Path $PSScriptRoot -Parent
$provisionProject = Join-Path $repoRoot "tools\AIPM.DbProvision\AIPM.DbProvision.csproj"

Write-Host "Verifying PostgreSQL connectivity (Npgsql)..."
dotnet build $provisionProject -c Release | Out-Null
dotnet run --project $provisionProject -c Release --no-build -- verify $adminConnection
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Creating database '$DatabaseName' if not exists..."
dotnet run --project $provisionProject -c Release --no-build -- create-database $adminConnection $DatabaseName
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Applying EF Core migrations..."
$env:AIPM_MIGRATION_CONNECTION = $identityConnection
dotnet ef database update `
    --project (Join-Path $repoRoot "src\AIPM.Infrastructure\AIPM.Infrastructure.csproj") `
    --startup-project (Join-Path $repoRoot "src\AIPM.Host\AIPM.Host.csproj") `
    --context IdentityDbContext `
    --connection $identityConnection

if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Inspecting schema..."
dotnet run --project $provisionProject -c Release --no-build -- inspect $identityConnection

Write-Host ""
Write-Host "Configure user secrets for local development:"
Write-Host "  cd src/AIPM.Host"
Write-Host "  dotnet user-secrets set `"ConnectionStrings:IdentityDb`" `"Host=$HostName;Port=$Port;Database=$DatabaseName;Username=$AdminUser;Password=***`""
