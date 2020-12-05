using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class TopicAddTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flair",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "Flair",
                table: "Posts",
                type: "text",
                nullable: true);
        }
    }
}
