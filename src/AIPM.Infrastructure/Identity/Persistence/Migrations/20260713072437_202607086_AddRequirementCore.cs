using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <summary>
/// BC-02 Slice 1: requirement aggregate + acceptance criteria tables.
/// Trace: AGG-005, CON-013, CON-014, CMD-030, FR-010, FR-011, ADR-005.
/// </summary>
public partial class _202607086_AddRequirementCore : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "requirements_requirements",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Statement = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Parsed = table.Column<bool>(type: "boolean", nullable: false),
                DocumentTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                DocumentContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                DocumentUriOrName = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_requirements_requirements", x => x.Id);
                table.ForeignKey(
                    name: "FK_requirements_requirements_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_requirements_requirements_portfolio_projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "portfolio_projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "requirements_acceptance_criteria",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                RequirementId = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Statement = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_requirements_acceptance_criteria", x => x.Id);
                table.ForeignKey(
                    name: "FK_requirements_acceptance_criteria_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_requirements_acceptance_criteria_requirements_requirements_~",
                    column: x => x.RequirementId,
                    principalTable: "requirements_requirements",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_requirements_acceptance_criteria_RequirementId",
            table: "requirements_acceptance_criteria",
            column: "RequirementId");

        migrationBuilder.CreateIndex(
            name: "IX_requirements_acceptance_criteria_TenantId",
            table: "requirements_acceptance_criteria",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_requirements_acceptance_criteria_TenantId_RequirementId",
            table: "requirements_acceptance_criteria",
            columns: new[] { "TenantId", "RequirementId" });

        migrationBuilder.CreateIndex(
            name: "IX_requirements_requirements_ProjectId",
            table: "requirements_requirements",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_requirements_requirements_TenantId_Id",
            table: "requirements_requirements",
            columns: new[] { "TenantId", "Id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_requirements_requirements_TenantId_ProjectId",
            table: "requirements_requirements",
            columns: new[] { "TenantId", "ProjectId" });

        migrationBuilder.CreateIndex(
            name: "IX_requirements_requirements_TenantId_ProjectId_Status",
            table: "requirements_requirements",
            columns: new[] { "TenantId", "ProjectId", "Status" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "requirements_acceptance_criteria");

        migrationBuilder.DropTable(
            name: "requirements_requirements");
    }
}
