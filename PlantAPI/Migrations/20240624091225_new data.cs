using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantAPI.Migrations
{
    /// <inheritdoc />
    public partial class newdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DHT_Humidity",
                table: "PlantData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DHT_Temperature",
                table: "PlantData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DHT_Humidity",
                table: "PlantData");

            migrationBuilder.DropColumn(
                name: "DHT_Temperature",
                table: "PlantData");
        }
    }
}
