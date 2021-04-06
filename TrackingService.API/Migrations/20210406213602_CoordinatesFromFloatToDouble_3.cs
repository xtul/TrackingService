using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class CoordinatesFromFloatToDouble_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<float>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
