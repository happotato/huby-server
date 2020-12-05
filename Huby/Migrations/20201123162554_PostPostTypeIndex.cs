using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class PostPostTypeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_PostType",
                table: "Posts",
                column: "PostType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_PostType",
                table: "Posts");
        }
    }
}
