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
            name: "identity_tenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "identity_roles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                FailClosed = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_identity_roles_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "identity_users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_users", x => x.Id);
                table.ForeignKey(
                    name: "FK_identity_users_identity_tenants_TenantId",
                    column: x => x.TenantId,
                    principalTable: "identity_tenants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "identity_permission_assignments",
            columns: table => new
            {
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                PermissionCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_permission_assignments", x => new { x.RoleId, x.PermissionCode });
                table.ForeignKey(
                    name: "FK_identity_permission_assignments_identity_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "identity_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "identity_role_assignments",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                AssignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_identity_role_assignments", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_identity_role_assignments_identity_roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "identity_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_identity_role_assignments_identity_users_UserId",
                    column: x => x.UserId,
                    principalTable: "identity_users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_identity_role_assignments_RoleId",
            table: "identity_role_assignments",
            column: "RoleId");

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
            name: "identity_users");

        migrationBuilder.DropTable(
            name: "identity_tenants");
    }
}
