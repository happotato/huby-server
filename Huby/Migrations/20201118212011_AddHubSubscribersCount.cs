using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddHubSubscribersCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsNSFW",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<long>(
                name: "SubscribersCount",
                table: "Hubs",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribersCount",
                table: "Hubs");

            migrationBuilder.AlterColumn<bool>(
                name: "IsNSFW",
                table: "Posts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
