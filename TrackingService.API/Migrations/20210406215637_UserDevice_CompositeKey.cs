using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TrackingService.API.Migrations
{
    public partial class UserDevice_CompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.DropIndex(
                name: "ix_user_device_device_id",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.AlterColumn<decimal>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<decimal>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device",
                columns: new[] { "device_id", "user_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "tracking",
                table: "user_device",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<double>(
                name: "speed",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "direction",
                schema: "tracking",
                table: "positions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_device",
                schema: "tracking",
                table: "user_device",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_user_device_device_id",
                schema: "tracking",
                table: "user_device",
                column: "device_id");
        }
    }
}
