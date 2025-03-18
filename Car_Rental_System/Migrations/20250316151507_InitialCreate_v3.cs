using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Car_Rental_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentPendingAt",
                table: "Cars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentPendingAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);
        }
    }
}
