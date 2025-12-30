using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEvacuationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_evacuation_zone_zone_id",
                table: "log");

            migrationBuilder.DropColumn(
                name: "remaining_people",
                table: "log");

            migrationBuilder.DropColumn(
                name: "total_evacuated",
                table: "log");

            migrationBuilder.RenameColumn(
                name: "zone_id",
                table: "log",
                newName: "evacuation_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_log_zone_id",
                table: "log",
                newName: "IX_log_evacuation_plan_id");

            migrationBuilder.AddColumn<double>(
                name: "eta",
                table: "log",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "is_evacuation_completed",
                table: "log",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_log_evacuation_plan_evacuation_plan_id",
                table: "log",
                column: "evacuation_plan_id",
                principalTable: "evacuation_plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_evacuation_plan_evacuation_plan_id",
                table: "log");

            migrationBuilder.DropColumn(
                name: "eta",
                table: "log");

            migrationBuilder.DropColumn(
                name: "is_evacuation_completed",
                table: "log");

            migrationBuilder.RenameColumn(
                name: "evacuation_plan_id",
                table: "log",
                newName: "zone_id");

            migrationBuilder.RenameIndex(
                name: "IX_log_evacuation_plan_id",
                table: "log",
                newName: "IX_log_zone_id");

            migrationBuilder.AddColumn<int>(
                name: "remaining_people",
                table: "log",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_evacuated",
                table: "log",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_log_evacuation_zone_zone_id",
                table: "log",
                column: "zone_id",
                principalTable: "evacuation_zone",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
