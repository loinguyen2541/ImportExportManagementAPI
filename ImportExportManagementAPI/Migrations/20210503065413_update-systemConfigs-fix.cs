using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updatesystemConfigsfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "23:00:00");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: "5/3/2021 12:00:00 AM");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "5/3/2021 12:00:00 AM");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: null);
        }
    }
}
