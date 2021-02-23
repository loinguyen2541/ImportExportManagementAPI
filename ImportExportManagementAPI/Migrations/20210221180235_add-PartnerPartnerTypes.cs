using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class addPartnerPartnerTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerType_Partner_PartnersPartnerId",
                table: "PartnerPartnerType");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerType_PartnerType_PartnerTypesPartnerTypeId",
                table: "PartnerPartnerType");

            migrationBuilder.RenameColumn(
                name: "PartnersPartnerId",
                table: "PartnerPartnerType",
                newName: "PartnerTypeId");

            migrationBuilder.RenameColumn(
                name: "PartnerTypesPartnerTypeId",
                table: "PartnerPartnerType",
                newName: "PartnerId");

            migrationBuilder.RenameIndex(
                name: "IX_PartnerPartnerType_PartnersPartnerId",
                table: "PartnerPartnerType",
                newName: "IX_PartnerPartnerType_PartnerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerType_Partner_PartnerId",
                table: "PartnerPartnerType",
                column: "PartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerType_PartnerType_PartnerTypeId",
                table: "PartnerPartnerType",
                column: "PartnerTypeId",
                principalTable: "PartnerType",
                principalColumn: "PartnerTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerType_Partner_PartnerId",
                table: "PartnerPartnerType");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerPartnerType_PartnerType_PartnerTypeId",
                table: "PartnerPartnerType");

            migrationBuilder.RenameColumn(
                name: "PartnerTypeId",
                table: "PartnerPartnerType",
                newName: "PartnersPartnerId");

            migrationBuilder.RenameColumn(
                name: "PartnerId",
                table: "PartnerPartnerType",
                newName: "PartnerTypesPartnerTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PartnerPartnerType_PartnerTypeId",
                table: "PartnerPartnerType",
                newName: "IX_PartnerPartnerType_PartnersPartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerType_Partner_PartnersPartnerId",
                table: "PartnerPartnerType",
                column: "PartnersPartnerId",
                principalTable: "Partner",
                principalColumn: "PartnerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerPartnerType_PartnerType_PartnerTypesPartnerTypeId",
                table: "PartnerPartnerType",
                column: "PartnerTypesPartnerTypeId",
                principalTable: "PartnerType",
                principalColumn: "PartnerTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
