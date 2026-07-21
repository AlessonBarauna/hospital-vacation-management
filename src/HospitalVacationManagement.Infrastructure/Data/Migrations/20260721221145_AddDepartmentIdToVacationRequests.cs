using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalVacationManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIdToVacationRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "vacation_requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "vacation_requests");
        }
    }
}
