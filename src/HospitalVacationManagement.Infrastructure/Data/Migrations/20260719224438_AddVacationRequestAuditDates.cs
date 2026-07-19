using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalVacationManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationRequestAuditDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "vacation_requests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "vacation_requests",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "vacation_requests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "vacation_requests");
        }
    }
}
