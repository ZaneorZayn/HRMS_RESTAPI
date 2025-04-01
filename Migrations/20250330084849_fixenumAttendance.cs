using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hrms_api.Migrations
{
    /// <inheritdoc />
    public partial class fixenumAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$XL/WW77q.gbgGlVP8dBUAehTCU1c0W.KSxlVsamChvQRc3yCw7mrq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$0caAzztlGVaUB8e3CAPqKO7p/JBckEtaVb.IZDbiLzjbvZwq1Rvp2");
        }
    }
}
