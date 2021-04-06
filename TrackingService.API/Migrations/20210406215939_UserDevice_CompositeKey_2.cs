using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackingService.API.Migrations
{
    public partial class UserDevice_CompositeKey_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_device_id_user_id",
                schema: "tracking",
                table: "user_device",
                columns: new[] { "device_id", "user_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_device_device_id_user_id",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device",
                columns: new[] { "device_id", "user_id" });
        }
    }
}
