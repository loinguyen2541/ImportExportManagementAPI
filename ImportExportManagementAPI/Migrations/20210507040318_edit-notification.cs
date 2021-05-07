using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_AccountUsername",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Schedule_ScheduleId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Transaction_TransactionId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AccountUsername",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "AccountUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Notification");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: "5/7/2021 12:00:00 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Username",
                table: "Notification",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Account_Username",
                table: "Notification",
                column: "Username",
                principalTable: "Account",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_Username",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_Username",
                table: "Notification");

            migrationBuilder.AddColumn<string>(
                name: "AccountUsername",
                table: "Notification",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: "5/5/2021 12:00:00 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountUsername",
                table: "Notification",
                column: "AccountUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TransactionId",
                table: "Notification",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Account_AccountUsername",
                table: "Notification",
                column: "AccountUsername",
                principalTable: "Account",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);

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
    }
}
