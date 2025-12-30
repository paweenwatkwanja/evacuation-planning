using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlanInEvacuationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_evacuation_plan_evacuation_plan_id",
                table: "log");

            migrationBuilder.RenameColumn(
                name: "evacuation_plan_id",
                table: "log",
                newName: "EvacuationPlanID");

            migrationBuilder.RenameIndex(
                name: "IX_log_evacuation_plan_id",
                table: "log",
                newName: "IX_log_EvacuationPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_log_evacuation_plan_EvacuationPlanID",
                table: "log",
                column: "EvacuationPlanID",
                principalTable: "evacuation_plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_evacuation_plan_EvacuationPlanID",
                table: "log");

            migrationBuilder.RenameColumn(
                name: "EvacuationPlanID",
                table: "log",
                newName: "evacuation_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_log_EvacuationPlanID",
                table: "log",
                newName: "IX_log_evacuation_plan_id");

            migrationBuilder.AddForeignKey(
                name: "FK_log_evacuation_plan_evacuation_plan_id",
                table: "log",
                column: "evacuation_plan_id",
                principalTable: "evacuation_plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
