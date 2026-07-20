using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalVacationManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationRequestCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "vacation_requests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "vacation_requests");
        }
    }
}
