using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updatescheduletransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScheduled",
                table: "Transaction",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApplyingDate",
                table: "TimeTemplate",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "Date",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "RealWeight",
                table: "Schedule",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScheduled",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "RealWeight",
                table: "Schedule");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApplyingDate",
                table: "TimeTemplate",
                type: "Date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
