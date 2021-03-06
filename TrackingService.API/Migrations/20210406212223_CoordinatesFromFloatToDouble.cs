using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class CoordinatesFromFloatToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                precision: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                precision: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 6);

            migrationBuilder.AlterColumn<float>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 6);
        }
    }
}
