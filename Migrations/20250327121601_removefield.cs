using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms_api.Migrations
{
    /// <inheritdoc />
    public partial class removefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_RolePermissions_RolePermissionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RolePermissionId",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "RolePermissionId",
                table: "RolePermissions");

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$4s1VXK4bzsMIZ5Uazr2M/eG8UDWhna1/FrTpbAA8aJMEUXbIQSxwi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RolePermissionId",
                table: "RolePermissions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "RolePermissionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$UlKLQXy.0Ggx9AjzGu/A9O.tU71ux4GXt9KPOsbLyinZGeibWAStu");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RolePermissionId",
                table: "RolePermissions",
                column: "RolePermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_RolePermissions_RolePermissionId",
                table: "RolePermissions",
                column: "RolePermissionId",
                principalTable: "RolePermissions",
                principalColumn: "Id");
        }
    }
}
