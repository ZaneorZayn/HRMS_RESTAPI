using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms_api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveRejectionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RejectedById",
                table: "LeaveRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedDate",
                table: "LeaveRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "LeaveRequests",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$Qmhy6scjAan5t18oq875ae0W0.Vi5NHwNg/HqLm9PHXeWg/XYkAlS");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_RejectedById",
                table: "LeaveRequests",
                column: "RejectedById");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Employees_RejectedById",
                table: "LeaveRequests",
                column: "RejectedById",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Employees_RejectedById",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_RejectedById",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RejectedDate",
                table: "LeaveRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "LeaveRequests");

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$OR3LnUH8BTAFeo447wzcPuVADWIlKgePOcpdh4Vm8mr/LsS75WlM6");
        }
    }
}
