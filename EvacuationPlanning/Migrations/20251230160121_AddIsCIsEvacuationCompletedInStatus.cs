using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCIsEvacuationCompletedInStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_evacuation_completed",
                table: "evacuation_status",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_evacuation_completed",
                table: "evacuation_status");
        }
    }
}
