using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <summary>
/// FR-122 tenant-scoped index for live portfolio aggregate rollups.
/// Trace: FR-122, ADR-SAD-008, ADR-005.
/// </summary>
public partial class _202607085_AddProjectionReadIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_portfolio_projects_TenantId_ProgramId_Status",
            table: "portfolio_projects",
            columns: new[] { "TenantId", "ProgramId", "Status" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_portfolio_projects_TenantId_ProgramId_Status",
            table: "portfolio_projects");
    }
}
