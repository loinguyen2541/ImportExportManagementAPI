using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotiv8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Transaction_TransactionId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Notification",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Notification");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Transaction_TransactionId",
                table: "Notification",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
