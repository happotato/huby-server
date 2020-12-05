using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class HubRenameIsNsfw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsNsfw",
                table: "Hubs",
                newName: "IsNSFW");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsNSFW",
                table: "Hubs",
                newName: "IsNsfw");
        }
    }
}
