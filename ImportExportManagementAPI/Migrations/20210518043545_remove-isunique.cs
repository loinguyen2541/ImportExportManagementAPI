using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class removeisunique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction",
                column: "ScheduleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction",
                column: "ScheduleId",
                unique: true,
                filter: "[ScheduleId] IS NOT NULL");
        }
    }
}
