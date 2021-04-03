using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExportManagementAPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    GoodsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuantityOfInventory = table.Column<float>(type: "real", nullable: false),
                    GoodsStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.GoodsId);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpeningStock = table.Column<float>(type: "real", nullable: false),
                    RecordedDate = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                });

            migrationBuilder.CreateTable(
                name: "PartnerType",
                columns: table => new
                {
                    PartnerTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerType", x => x.PartnerTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

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

            migrationBuilder.CreateTable(
                name: "TimeTemplate",
                columns: table => new
                {
                    TimeTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeTemplateName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ApplyingDate = table.Column<DateTime>(type: "Date", nullable: false),
                    TimeTemplateStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeTemplate", x => x.TimeTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Account_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeTemplateItem",
                columns: table => new
                {
                    TimeTemplateItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Inventory = table.Column<float>(type: "real", nullable: false),
                    ScheduleTime = table.Column<TimeSpan>(type: "time", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Partner",
                columns: table => new
                {
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PartnerStatus = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    PartnerTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partner", x => x.PartnerId);
                    table.ForeignKey(
                        name: "FK_Partner_Account_Username",
                        column: x => x.Username,
                        principalTable: "Account",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partner_PartnerType_PartnerTypeId",
                        column: x => x.PartnerTypeId,
                        principalTable: "PartnerType",
                        principalColumn: "PartnerTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityCard",
                columns: table => new
                {
                    IdentityCardId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IdentityCardStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityCard", x => x.IdentityCardId);
                    table.ForeignKey(
                        name: "FK_IdentityCard_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDetail",
                columns: table => new
                {
                    InventoryDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDetail", x => x.InventoryDetailId);
                    table.ForeignKey(
                        name: "FK_InventoryDetail_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "GoodsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryDetail_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "InventoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryDetail_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisteredWeight = table.Column<float>(type: "real", nullable: false),
                    ActualWeight = table.Column<float>(type: "real", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    ScheduleStatus = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    GoodsId = table.Column<int>(type: "int", nullable: false),
                    TimeTemplateItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_Schedule_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "GoodsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedule_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedule_TimeTemplateItem_TimeTemplateItemId",
                        column: x => x.TimeTemplateItemId,
                        principalTable: "TimeTemplateItem",
                        principalColumn: "TimeTemplateItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeightIn = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeightOut = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    IdentificationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoodsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transaction_Goods_GoodsId",
                        column: x => x.GoodsId,
                        principalTable: "Goods",
                        principalColumn: "GoodsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Partner_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partner",
                        principalColumn: "PartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "StorageCapacity", "2000" });

            migrationBuilder.InsertData(
                table: "SystemConfig",
                columns: new[] { "AttributeKey", "AttributeValue" },
                values: new object[] { "AutoSchedule", "20:00:00" });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityCard_PartnerId",
                table: "IdentityCard",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetail_GoodsId",
                table: "InventoryDetail",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetail_InventoryId",
                table: "InventoryDetail",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetail_PartnerId",
                table: "InventoryDetail",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_PartnerTypeId",
                table: "Partner",
                column: "PartnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_Username",
                table: "Partner",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_GoodsId",
                table: "Schedule",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_PartnerId",
                table: "Schedule",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TimeTemplateItemId",
                table: "Schedule",
                column: "TimeTemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTemplateItem_TimeTemplateId",
                table: "TimeTemplateItem",
                column: "TimeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_GoodsId",
                table: "Transaction",
                column: "GoodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PartnerId",
                table: "Transaction",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityCard");

            migrationBuilder.DropTable(
                name: "InventoryDetail");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "SystemConfig");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "TimeTemplateItem");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Partner");

            migrationBuilder.DropTable(
                name: "TimeTemplate");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "PartnerType");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
