using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hrms_api.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "DOB", "DepartmentId", "Email", "Gender", "HiredDate", "ImageUrl", "Name", "PhoneNumber", "PositionId", "SystemUserId" },
                values: new object[,]
                {
                    { 1, "123 Main St", new DateTime(1990, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "johndoe@example.com", 1, new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/john.jpg", "John Doe", "1234567890", null, null },
                    { 2, "456 Oak St", new DateTime(1992, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "janesmith@example.com", 2, new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/jane.jpg", "Jane Smith", "9876543210", null, null },
                    { 3, "789 Pine St", new DateTime(1988, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "michaeljohnson@example.com", 1, new DateTime(2019, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/michael.jpg", "Michael Johnson", "5551234567", null, null },
                    { 4, "101 Maple St", new DateTime(1995, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "emilydavis@example.com", 2, new DateTime(2022, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/emily.jpg", "Emily Davis", "4449876543", null, null },
                    { 5, "202 Birch St", new DateTime(1991, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "williambrown@example.com", 1, new DateTime(2020, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/william.jpg", "William Brown", "2223334444", null, null },
                    { 6, "303 Cedar St", new DateTime(1993, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "sophiamiller@example.com", 2, new DateTime(2021, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/sophia.jpg", "Sophia Miller", "3334445555", null, null },
                    { 7, "404 Elm St", new DateTime(1989, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "jameswilson@example.com", 1, new DateTime(2018, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/james.jpg", "James Wilson", "1112223333", null, null },
                    { 8, "505 Walnut St", new DateTime(1996, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "oliviamartinez@example.com", 2, new DateTime(2022, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/olivia.jpg", "Olivia Martinez", "6667778888", null, null },
                    { 9, "606 Willow St", new DateTime(1994, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "danielanderson@example.com", 1, new DateTime(2020, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/daniel.jpg", "Daniel Anderson", "9990001111", null, null },
                    { 10, "707 Cherry St", new DateTime(1997, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "avathomas@example.com", 2, new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://example.com/images/ava.jpg", "Ava Thomas", "7778889999", null, null }
                });

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$5jfPN0nuIXBNlOgfZB6XkuiWuDONmeXN1lxNMVm3tD.xK5JvPxixW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "SystemUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$10$4drBXMrwLw3.9xljusvH8Oh6rcXUAfwXNph7pKoBgVlCCo/HhrcLu");
        }
    }
}
