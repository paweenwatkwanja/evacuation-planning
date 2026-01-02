using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class LocationCoordicateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "location_coordinate_id",
                table: "evacuation_zone",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "location_coordinate",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_coordinate", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evacuation_zone_location_coordinate_id",
                table: "evacuation_zone",
                column: "location_coordinate_id");

            migrationBuilder.AddForeignKey(
                name: "FK_evacuation_zone_location_coordinate_location_coordinate_id",
                table: "evacuation_zone",
                column: "location_coordinate_id",
                principalTable: "location_coordinate",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_evacuation_zone_location_coordinate_location_coordinate_id",
                table: "evacuation_zone");

            migrationBuilder.DropTable(
                name: "location_coordinate");

            migrationBuilder.DropIndex(
                name: "IX_evacuation_zone_location_coordinate_id",
                table: "evacuation_zone");

            migrationBuilder.DropColumn(
                name: "location_coordinate_id",
                table: "evacuation_zone");
        }
    }
}
