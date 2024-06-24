using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedsensortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SensorId",
                table: "PlantData",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantData_SensorId",
                table: "PlantData",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantData_Sensors_SensorId",
                table: "PlantData",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantData_Sensors_SensorId",
                table: "PlantData");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_PlantData_SensorId",
                table: "PlantData");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "PlantData");
        }
    }
}
