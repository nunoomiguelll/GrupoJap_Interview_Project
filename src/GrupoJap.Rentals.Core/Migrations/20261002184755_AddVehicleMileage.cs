using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoJap.Rentals.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleMileage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Vehicles");
        }
    }
}
