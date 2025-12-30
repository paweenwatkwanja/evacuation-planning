using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class EvacuationStatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evacuation_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    zone_id = table.Column<long>(type: "bigint", nullable: false),
                    total_evacuated = table.Column<int>(type: "integer", nullable: false),
                    remaining_people = table.Column<int>(type: "integer", nullable: false),
                    last_vehicle_used = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evacuation_status", x => x.id);
                    table.ForeignKey(
                        name: "FK_evacuation_status_evacuation_zone_zone_id",
                        column: x => x.zone_id,
                        principalTable: "evacuation_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_evacuation_status_vehicle_last_vehicle_used",
                        column: x => x.last_vehicle_used,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evacuation_status_last_vehicle_used",
                table: "evacuation_status",
                column: "last_vehicle_used");

            migrationBuilder.CreateIndex(
                name: "IX_evacuation_status_zone_id",
                table: "evacuation_status",
                column: "zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evacuation_status");
        }
    }
}
