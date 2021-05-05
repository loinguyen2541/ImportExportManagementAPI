using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class edinoti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "StatusAdmin",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "StatusPartner",
                table: "Notification",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ContentForPartner",
                table: "Notification",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ContentForAdmin",
                table: "Notification",
                newName: "Content");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "Notification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AccountUsername",
                table: "Notification",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Notification",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: "5/5/2021 12:00:00 AM");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountUsername",
                table: "Notification",
                column: "AccountUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Account_AccountUsername",
                table: "Notification",
                column: "AccountUsername",
                principalTable: "Account",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Account_AccountUsername",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AccountUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "AccountUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notification",
                newName: "ContentForPartner");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Notification",
                newName: "StatusPartner");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notification",
                newName: "ContentForAdmin");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusAdmin",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "SystemConfig",
                keyColumn: "AttributeKey",
                keyValue: "SystemDate",
                column: "AttributeValue",
                value: "5/3/2021 12:00:00 AM");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Partner_PartnerId",
                table: "Notification",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
