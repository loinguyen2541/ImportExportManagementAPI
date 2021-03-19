using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addSystemCongig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemConfig",
                columns: table => new
                {
                    AttributeKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttributeValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfig", x => x.AttributeKey);
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "StorageCapacity", "0" });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "AutoSchedule", "0" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemConfig");
        }
    }
}
