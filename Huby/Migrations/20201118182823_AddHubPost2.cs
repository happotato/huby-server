using Microsoft.EntityFrameworkCore.Migrations;

namespace Huby.Migrations
{
    public partial class AddHubPost2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Hubs_HubId1",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HubId1",
                table: "Posts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
