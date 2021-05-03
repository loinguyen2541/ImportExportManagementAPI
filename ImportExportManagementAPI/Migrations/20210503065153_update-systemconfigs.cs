using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updatesystemconfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "5/3/2021 12:00:00 AM");

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "SystemDate", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate");

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "AutoSchedule",
                column: "AttributeValue",
                value: "13:00:00");
        }
    }
}
