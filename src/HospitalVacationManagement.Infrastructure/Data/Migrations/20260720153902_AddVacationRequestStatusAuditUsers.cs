using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalVacationManagement.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationRequestStatusAuditUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByUserId",
                table: "vacation_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "vacation_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedByUserId",
                table: "vacation_requests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "vacation_requests");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "vacation_requests");

            migrationBuilder.DropColumn(
                name: "RejectedByUserId",
                table: "vacation_requests");
        }
    }
}
