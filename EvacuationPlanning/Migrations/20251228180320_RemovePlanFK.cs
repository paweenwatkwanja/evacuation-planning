using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class RemovePlanFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_log_evacuation_plan_EvacuationPlanID",
                table: "log");

            migrationBuilder.DropIndex(
                name: "IX_log_EvacuationPlanID",
                table: "log");

            migrationBuilder.DropColumn(
                name: "EvacuationPlanID",
                table: "log");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EvacuationPlanID",
                table: "log",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_log_EvacuationPlanID",
                table: "log",
                column: "EvacuationPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_log_evacuation_plan_EvacuationPlanID",
                table: "log",
                column: "EvacuationPlanID",
                principalTable: "evacuation_plan",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
