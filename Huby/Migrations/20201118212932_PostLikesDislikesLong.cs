using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class PostLikesDislikesLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Likes",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Dislikes",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Likes",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Dislikes",
                table: "Posts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
