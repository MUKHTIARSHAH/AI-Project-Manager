using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <inheritdoc />
public partial class _202607084_AddScopeChangeCore : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "portfolio_scope_changes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                AffectedRequirementCitation = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_portfolio_scope_changes", x => x.Id);
                table.ForeignKey(
                    name: "FK_portfolio_scope_changes_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_portfolio_scope_changes_portfolio_projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "portfolio_projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_scope_changes_ProjectId",
            table: "portfolio_scope_changes",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_scope_changes_TenantId",
            table: "portfolio_scope_changes",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_scope_changes_TenantId_ProjectId",
            table: "portfolio_scope_changes",
            columns: new[] { "TenantId", "ProjectId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "portfolio_scope_changes");
    }
}
