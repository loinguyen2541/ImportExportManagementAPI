using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class updateinventorydetailtransactionschedulecard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Partner_PartnerId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PartnerId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Inventory");

            migrationBuilder.AddColumn<string>(
                name: "IdentityCardId",
                table: "Transaction",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Schedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Schedule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "InventoryDetail",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "IdentityCard",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_IdentityCardId",
                table: "Transaction",
                column: "IdentityCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_IdentityCard_IdentityCardId",
                table: "Transaction",
                column: "IdentityCardId",
                principalTable: "IdentityCard",
                principalColumn: "IdentityCardId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_IdentityCard_IdentityCardId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_IdentityCardId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "IdentityCardId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "InventoryDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "IdentityCard");

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Transaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "Inventory",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PartnerId",
                table: "Transaction",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Partner_PartnerId",
                table: "Transaction",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
