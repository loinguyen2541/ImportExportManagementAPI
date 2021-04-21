using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class editnotiv10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trans",
                table: "Notification",
                newName: "Type");

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

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notification",
                newName: "Trans");
        }
    }
}
