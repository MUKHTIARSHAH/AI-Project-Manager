using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPM.Infrastructure.Identity.Persistence.Migrations;

/// <inheritdoc />
public partial class AddIdentityForeignKeys : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_identity_role_assignments_RoleId",
            table: "identity_role_assignments",
            column: "RoleId");

        migrationBuilder.AddForeignKey(
            name: "FK_identity_permission_assignments_identity_roles_RoleId",
            table: "identity_permission_assignments",
            column: "RoleId",
            principalTable: "identity_roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_identity_role_assignments_identity_roles_RoleId",
            table: "identity_role_assignments",
            column: "RoleId",
            principalTable: "identity_roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_identity_role_assignments_identity_users_UserId",
            table: "identity_role_assignments",
            column: "UserId",
            principalTable: "identity_users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_identity_roles_identity_tenants_TenantId",
            table: "identity_roles",
            column: "TenantId",
            principalTable: "identity_tenants",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_identity_users_identity_tenants_TenantId",
            table: "identity_users",
            column: "TenantId",
            principalTable: "identity_tenants",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_identity_permission_assignments_identity_roles_RoleId",
            table: "identity_permission_assignments");

        migrationBuilder.DropForeignKey(
            name: "FK_identity_role_assignments_identity_roles_RoleId",
            table: "identity_role_assignments");

        migrationBuilder.DropForeignKey(
            name: "FK_identity_role_assignments_identity_users_UserId",
            table: "identity_role_assignments");

        migrationBuilder.DropForeignKey(
            name: "FK_identity_roles_identity_tenants_TenantId",
            table: "identity_roles");

        migrationBuilder.DropForeignKey(
            name: "FK_identity_users_identity_tenants_TenantId",
            table: "identity_users");

        migrationBuilder.DropIndex(
            name: "IX_identity_role_assignments_RoleId",
            table: "identity_role_assignments");
    }
}
