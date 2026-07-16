using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HospitalVacationManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialHospitalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "FullName", "Role", "SeniorityLevel" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Karol Barauna", 1, 1 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Ana Silva", 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "Id", "MaximumSimultaneousVacations", "Name" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 1, "Emergency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        }
    }
}
