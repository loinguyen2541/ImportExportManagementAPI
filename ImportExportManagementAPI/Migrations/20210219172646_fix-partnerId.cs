using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class fixpartnerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityCard_Partner_PartnerId",
                table: "IdentityCard");

            migrationBuilder.DropColumn(
                name: "ParnerId",
                table: "IdentityCard");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "IdentityCard",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityCard_Partner_PartnerId",
                table: "IdentityCard",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityCard_Partner_PartnerId",
                table: "IdentityCard");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerId",
                table: "IdentityCard",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ParnerId",
                table: "IdentityCard",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityCard_Partner_PartnerId",
                table: "IdentityCard",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
