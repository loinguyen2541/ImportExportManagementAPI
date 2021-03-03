using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class inventorydetailid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail");

            migrationBuilder.AddColumn<int>(
                name: "InventoryDetailId",
                table: "InventoryDetail",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail",
                column: "InventoryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetail_GoodsId",
                table: "InventoryDetail",
                column: "GoodsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDetail_GoodsId",
                table: "InventoryDetail");

            migrationBuilder.DropColumn(
                name: "InventoryDetailId",
                table: "InventoryDetail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail",
                columns: new[] { "GoodsId", "InventoryId" });
        }
    }
}
