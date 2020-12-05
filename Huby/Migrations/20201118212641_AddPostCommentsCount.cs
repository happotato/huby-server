using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddPostCommentsCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommentsCount",
                table: "Posts",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsCount",
                table: "Posts");
        }
    }
}
