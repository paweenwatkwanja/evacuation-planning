using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvacuationPlanning.Migrations
{
    /// <inheritdoc />
    public partial class UpdateETADataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "eta",
                table: "evacuation_plan",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "eta",
                table: "evacuation_plan",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
