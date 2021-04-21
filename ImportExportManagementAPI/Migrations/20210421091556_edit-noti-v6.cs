using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotiv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Schedule_ScheduleId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Notification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ScheduleId",
                table: "Notification",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Schedule_ScheduleId",
                table: "Notification",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
