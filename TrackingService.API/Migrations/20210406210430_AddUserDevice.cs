using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TrackingService.API.Migrations
{
    public partial class AddUserDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 2);

            migrationBuilder.AlterColumn<double>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldPrecision: 2);

            migrationBuilder.CreateTable(
                name: "user_device",
                schema: "tracking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    device_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_device", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_device_devices_device_id",
                        column: x => x.device_id,
                        principalSchema: "tracking",
                        principalTable: "devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_device_tracking_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "tracking",
                        principalTable: "tracking_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_device_device_id",
                schema: "tracking",
                table: "user_device",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_user_id",
                schema: "tracking",
                table: "user_device",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_device",
                schema: "tracking");

            migrationBuilder.AlterColumn<float>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<float>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "real",
                precision: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
