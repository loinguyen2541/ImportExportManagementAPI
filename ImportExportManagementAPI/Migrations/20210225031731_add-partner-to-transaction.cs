using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addpartnertotransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PartnerId",
                table: "Transaction",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Partner_PartnerId",
                table: "Transaction",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Partner_PartnerId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PartnerId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Transaction");
        }
    }
}
