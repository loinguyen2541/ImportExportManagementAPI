using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updatetransactionschedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_IdentityCard_IdentityCardId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_IdentityCardId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "IdentityCardId",
                table: "Transaction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityCardId",
                table: "Transaction",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IdentityCardId",
                table: "Transaction",
                column: "IdentityCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_IdentityCard_IdentityCardId",
                table: "Transaction",
                column: "IdentityCardId",
                principalTable: "IdentityCard",
                principalColumn: "IdentityCardId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
