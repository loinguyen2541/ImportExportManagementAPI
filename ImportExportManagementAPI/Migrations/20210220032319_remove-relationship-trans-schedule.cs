using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class removerelationshiptransschedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Schedule_ScheduleId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ScheduleId",
                table: "Transaction",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Schedule_ScheduleId",
                table: "Transaction",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
