using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updateGetValueFromAppsettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "18:00:00");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "StorageCapacity",
                column: "AttributeValue",
                value: "2000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "0");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "StorageCapacity",
                column: "AttributeValue",
                value: "0");
        }
    }
}
