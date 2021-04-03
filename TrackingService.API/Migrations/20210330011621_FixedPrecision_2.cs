using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class FixedPrecision_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float(2)");

            migrationBuilder.AlterColumn<float>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float(6)");

            migrationBuilder.AlterColumn<float>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 6,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float(6)");

            migrationBuilder.AlterColumn<float>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 2,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "float(2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "float(2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 2);

            migrationBuilder.AlterColumn<float>(
                name: "lon",
                schema: "tracking",
                table: "positions",
                type: "float(6)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 6);

            migrationBuilder.AlterColumn<float>(
                name: "lat",
                schema: "tracking",
                table: "positions",
                type: "float(6)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 6);

            migrationBuilder.AlterColumn<float>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "float(2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 2);
        }
    }
}
