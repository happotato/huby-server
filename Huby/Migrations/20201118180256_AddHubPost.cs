using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddHubPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HubId",
                table: "Posts",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(12)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_HubId1",
                table: "Posts",
                column: "HubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Hubs_HubId1",
                table: "Posts",
                column: "HubId",
                principalTable: "Hubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Hubs_HubId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HubId1",
                table: "Posts");

            migrationBuilder.AlterColumn<string>(
                name: "HubId",
                table: "Posts",
                type: "character varying(12)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
