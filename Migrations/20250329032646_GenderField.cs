using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms_api.Migrations
{
    /// <inheritdoc />
    public partial class GenderField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 4,
                column: "PermissionName",
                value: "Generate Payroll");

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$Qo.4glYJ0xW.dtV875WPvesTHV67l4I9q42KVoXRgo1J6215gf/.S");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 4,
                column: "PermissionName",
                value: "Create Payroll");

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$geQg98T03ye3iD9Km0KAQuQawA7G1O8wPuNlJepQrV12Fl.jYPHQm");
        }
    }
}
