using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addinventoryquantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "InventoryQuantity",
                table: "Inventory",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryQuantity",
                table: "Inventory");
        }
    }
}
