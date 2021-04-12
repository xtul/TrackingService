using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class DevicesRemovedLastPositionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_position_id",
                schema: "tracking",
                table: "devices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "last_position_id",
                schema: "tracking",
                table: "devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
