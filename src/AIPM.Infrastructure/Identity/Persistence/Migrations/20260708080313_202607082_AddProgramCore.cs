using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <inheritdoc />
public partial class _202607082_AddProgramCore : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "portfolio_programs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                PortfolioId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_portfolio_programs", x => x.Id);
                table.ForeignKey(
                    name: "FK_portfolio_programs_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_portfolio_programs_portfolio_portfolios_PortfolioId",
                    column: x => x.PortfolioId,
                    principalTable: "portfolio_portfolios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_programs_PortfolioId",
            table: "portfolio_programs",
            column: "PortfolioId");

        migrationBuilder.CreateIndex(
            name: "IX_portfolio_programs_TenantId_PortfolioId_Name",
            table: "portfolio_programs",
            columns: new[] { "TenantId", "PortfolioId", "Name" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "portfolio_programs");
    }
}
