using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Car_Rental_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarRentalDetails");

            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "CarRentals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarRentals_CarId",
                table: "CarRentals",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarRentals_Cars_CarId",
                table: "CarRentals",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarRentals_Cars_CarId",
                table: "CarRentals");

            migrationBuilder.DropIndex(
                name: "IX_CarRentals_CarId",
                table: "CarRentals");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "CarRentals");

            migrationBuilder.CreateTable(
                name: "CarRentalDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    CarRentalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentalDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarRentalDetails_CarRentals_CarRentalId",
                        column: x => x.CarRentalId,
                        principalTable: "CarRentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarRentalDetails_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalDetails_CarId",
                table: "CarRentalDetails",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalDetails_CarRentalId",
                table: "CarRentalDetails",
                column: "CarRentalId");
        }
    }
}
