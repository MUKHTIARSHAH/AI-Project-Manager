using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <inheritdoc />
public partial class _202607081_AddPortfolioCore : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "portfolio_portfolios",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_portfolio_portfolios", x => x.Id);
                table.ForeignKey(
                    name: "FK_portfolio_portfolios_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_portfolios_TenantId_Name",
            table: "portfolio_portfolios",
            columns: new[] { "TenantId", "Name" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "portfolio_portfolios");
    }
}
