using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddFixPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Posts");

            migrationBuilder.AlterColumn<bool>(
                name: "IsNSFW",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hubs_Name",
                table: "Hubs",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hubs_Name",
                table: "Hubs");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "IsNSFW",
                table: "Posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Posts",
                type: "text",
                nullable: true);
        }
    }
}
