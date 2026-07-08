using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialIdentity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "identity_permission_assignments",
            columns: table => new
            {
                RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                PermissionCode = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                AssignedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_permission_assignments", x => new { x.RoleId, x.PermissionCode });
            });

        migrationBuilder.CreateTable(
            name: "identity_role_assignments",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                AssignedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_role_assignments", x => new { x.UserId, x.RoleId });
            });

        migrationBuilder.CreateTable(
            name: "identity_roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                FailClosed = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "identity_tenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "identity_users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_users", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_identity_roles_TenantId_Name",
            table: "identity_roles",
            columns: new[] { "TenantId", "Name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_identity_users_TenantId_Email",
            table: "identity_users",
            columns: new[] { "TenantId", "Email" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "identity_permission_assignments");

        migrationBuilder.DropTable(
            name: "identity_role_assignments");

        migrationBuilder.DropTable(
            name: "identity_roles");

        migrationBuilder.DropTable(
            name: "identity_tenants");

        migrationBuilder.DropTable(
            name: "identity_users");
    }
}
