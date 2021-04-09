using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class DeviceImeiUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_devices_imei",
                schema: "tracking",
                table: "devices");

            migrationBuilder.CreateIndex(
                name: "ix_devices_imei",
                schema: "tracking",
                table: "devices",
                column: "imei",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_devices_imei",
                schema: "tracking",
                table: "devices");

            migrationBuilder.CreateIndex(
                name: "ix_devices_imei",
                schema: "tracking",
                table: "devices",
                column: "imei");
        }
    }
}
