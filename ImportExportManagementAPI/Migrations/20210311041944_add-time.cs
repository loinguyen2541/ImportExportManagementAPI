using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addtime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeTemplateItemId",
                table: "Schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TimeTemplate",
                columns: table => new
                {
                    TimeTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeTemplateName = table.Column<int>(type: "int", nullable: false),
                    TimeTemplateStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTemplate", x => x.TimeTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "TimeTemplateItem",
                columns: table => new
                {
                    TimeTemplateItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<float>(type: "real", nullable: false),
                    ScheduleTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTemplateItem", x => x.TimeTemplateItemId);
                    table.ForeignKey(
                        name: "FK_TimeTemplateItem_TimeTemplate_TimeTemplateId",
                        column: x => x.TimeTemplateId,
                        principalTable: "TimeTemplate",
                        principalColumn: "TimeTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TimeTemplateItemId",
                table: "Schedule",
                column: "TimeTemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTemplateItem_TimeTemplateId",
                table: "TimeTemplateItem",
                column: "TimeTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_TimeTemplateItem_TimeTemplateItemId",
                table: "Schedule",
                column: "TimeTemplateItemId",
                principalTable: "TimeTemplateItem",
                principalColumn: "TimeTemplateItemId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_TimeTemplateItem_TimeTemplateItemId",
                table: "Schedule");

            migrationBuilder.DropTable(
                name: "TimeTemplateItem");

            migrationBuilder.DropTable(
                name: "TimeTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Schedule_TimeTemplateItemId",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "TimeTemplateItemId",
                table: "Schedule");
        }
    }
}
