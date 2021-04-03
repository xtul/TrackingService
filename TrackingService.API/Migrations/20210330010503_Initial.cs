using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TrackingService.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tracking");

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "tracking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    last_position_id = table.Column<int>(type: "integer", nullable: false),
                    imei = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                schema: "tracking",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    protocol = table.Column<string>(type: "text", nullable: true),
                    lon = table.Column<float>(type: "real", precision: 6, nullable: false),
                    lat = table.Column<float>(type: "real", precision: 6, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    speed = table.Column<float>(type: "real", precision: 2, nullable: false),
                    direction = table.Column<float>(type: "real", precision: 2, nullable: false),
                    misc_info = table.Column<string>(type: "text", nullable: true),
                    imei = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_positions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_devices_imei",
                schema: "tracking",
                table: "devices",
                column: "imei");

            migrationBuilder.CreateIndex(
                name: "ix_positions_imei",
                schema: "tracking",
                table: "positions",
                column: "imei");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices",
                schema: "tracking");

            migrationBuilder.DropTable(
                name: "positions",
                schema: "tracking");
        }
    }
}
