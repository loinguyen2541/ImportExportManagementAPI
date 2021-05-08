using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addsysconfigdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate");

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[,]
                {
                    { "StartBreak", "11:30:00" },
                    { "FinishBreak", "13:00:00" },
                    { "StartWorking", "8:00:00" },
                    { "FinishWorking", "5:00:00" },
                    { "TimeBetweenSlot", "30" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "FinishBreak");

            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "FinishWorking");

            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "StartBreak");

            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "StartWorking");

            migrationBuilder.DeleteData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "TimeBetweenSlot");

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "SystemDate", "5/7/2021 12:00:00 AM" });
        }
    }
}
