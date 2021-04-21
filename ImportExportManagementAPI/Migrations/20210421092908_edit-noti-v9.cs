using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotiv9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Notification",
                newName: "Trans");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trans",
                table: "Notification",
                newName: "TransactionId");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Notification",
                type: "int",
                nullable: true);
        }
    }
}
