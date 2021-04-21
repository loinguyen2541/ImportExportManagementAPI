using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotiv12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Schedule_ScheduleId",
                table: "Notification",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Transaction_TransactionId",
                table: "Notification",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Schedule_ScheduleId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Transaction_TransactionId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification");
        }
    }
}
