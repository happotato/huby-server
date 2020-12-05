using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddTopicStickied : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Locked",
                table: "Posts",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "Stickied",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stickied",
                table: "Posts");

            migrationBuilder.AlterColumn<bool>(
                name: "Locked",
                table: "Posts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}
