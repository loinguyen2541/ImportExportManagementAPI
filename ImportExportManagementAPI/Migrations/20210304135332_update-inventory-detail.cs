using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updateinventorydetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "InventoryDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetail_PartnerId",
                table: "InventoryDetail",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetail_Partner_PartnerId",
                table: "InventoryDetail",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetail_Partner_PartnerId",
                table: "InventoryDetail");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDetail_PartnerId",
                table: "InventoryDetail");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "InventoryDetail");
        }
    }
}
