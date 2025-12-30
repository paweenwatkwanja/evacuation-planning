using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_vehicle_vehicle_id",
                table: "vehicle",
                column: "vehicle_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evacuation_zone_zone_id",
                table: "evacuation_zone",
                column: "zone_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vehicle_vehicle_id",
                table: "vehicle");

            migrationBuilder.DropIndex(
                name: "IX_evacuation_zone_zone_id",
                table: "evacuation_zone");
        }
    }
}
