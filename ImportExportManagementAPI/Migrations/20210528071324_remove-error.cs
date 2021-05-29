using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class removeerror : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_PartnerId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Inventory",
                table: "TimeTemplateItem");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Notification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Inventory",
                table: "TimeTemplateItem",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_PartnerId",
                table: "Notification",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
