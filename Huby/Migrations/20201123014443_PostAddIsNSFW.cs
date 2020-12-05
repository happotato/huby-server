using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class PostAddIsNSFW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsNSFW",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsNSFW",
                table: "Posts",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
