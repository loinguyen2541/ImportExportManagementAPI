﻿// <auto-generated />
using System;
using ImportExportManagement_API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImportExportManagementAPI.Migrations
{
    [DbContext(typeof(IEDbContext))]
    partial class IEDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ImportExportManagementAPI.Models.ActivityLog", b =>
                {
                    b.Property<int>("ActivityLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountUsername")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ActivityLogId");

                    b.HasIndex("AccountUsername");

                    b.ToTable("ActivityLog");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<int?>("PartnerId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("NotificationId");

                    b.HasIndex("PartnerId");

                    b.HasIndex("Username");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.PartnerType", b =>
                {
                    b.Property<int>("PartnerTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PartnerTypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PartnerTypeId");

                    b.ToTable("PartnerType");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.SystemConfig", b =>
                {
                    b.Property<string>("AttributeKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AttributeValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AttributeKey");

                    b.ToTable("SystemConfig");

                    b.HasData(
                        new
                        {
                            AttributeKey = "StorageCapacity",
                            AttributeValue = "2000"
                        },
                        new
                        {
                            AttributeKey = "AutoSchedule",
                            AttributeValue = "23:00:00"
                        },
                        new
                        {
                            AttributeKey = "StartBreak",
                            AttributeValue = "11:30:00"
                        },
                        new
                        {
                            AttributeKey = "FinishBreak",
                            AttributeValue = "13:00:00"
                        },
                        new
                        {
                            AttributeKey = "StartWorking",
                            AttributeValue = "8:00:00"
                        },
                        new
                        {
                            AttributeKey = "FinishWorking",
                            AttributeValue = "5:00:00"
                        },
                        new
                        {
                            AttributeKey = "TimeBetweenSlot",
                            AttributeValue = "30"
                        },
                        new
                        {
                            AttributeKey = "MaximumCanceledSchechule",
                            AttributeValue = "3"
                        },
                        new
                        {
                            AttributeKey = "MaximumSlot",
                            AttributeValue = "10"
                        });
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.TimeTemplate", b =>
                {
                    b.Property<int>("TimeTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ApplyingDate")
                        .HasColumnType("Date");

                    b.Property<string>("TimeTemplateName")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("TimeTemplateStatus")
                        .HasColumnType("int");

                    b.HasKey("TimeTemplateId");

                    b.ToTable("TimeTemplate");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.TimeTemplateItem", b =>
                {
                    b.Property<int>("TimeTemplateItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Inventory")
                        .HasColumnType("real");

                    b.Property<TimeSpan>("ScheduleTime")
                        .HasColumnType("time");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TimeTemplateId")
                        .HasColumnType("int");

                    b.HasKey("TimeTemplateItemId");

                    b.HasIndex("TimeTemplateId");

                    b.ToTable("TimeTemplateItem");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Account", b =>
                {
                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DeviceToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.HasIndex("RoleId");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Goods", b =>
                {
                    b.Property<int>("GoodsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GoodName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("GoodsStatus")
                        .HasColumnType("int");

                    b.Property<float>("QuantityOfInventory")
                        .HasColumnType("real");

                    b.HasKey("GoodsId");

                    b.ToTable("Goods");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.IdentityCard", b =>
                {
                    b.Property<string>("IdentityCardId")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdentityCardStatus")
                        .HasColumnType("int");

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.HasKey("IdentityCardId");

                    b.HasIndex("PartnerId");

                    b.ToTable("IdentityCard");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Inventory", b =>
                {
                    b.Property<int>("InventoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("OpeningStock")
                        .HasColumnType("real");

                    b.Property<DateTime>("RecordedDate")
                        .HasColumnType("Date");

                    b.HasKey("InventoryId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.InventoryDetail", b =>
                {
                    b.Property<int>("InventoryDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<int>("InventoryId")
                        .HasColumnType("int");

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<int?>("Type")
                        .HasColumnType("int");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("InventoryDetailId");

                    b.HasIndex("GoodsId");

                    b.HasIndex("InventoryId");

                    b.HasIndex("PartnerId");

                    b.ToTable("InventoryDetail");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Partner", b =>
                {
                    b.Property<int>("PartnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PartnerStatus")
                        .HasColumnType("int");

                    b.Property<int>("PartnerTypeId")
                        .HasColumnType("int");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("PartnerId");

                    b.HasIndex("PartnerTypeId");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("RoleName")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float?>("ActualWeight")
                        .HasColumnType("real");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<float>("RegisteredWeight")
                        .HasColumnType("real");

                    b.Property<DateTime>("ScheduleDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduleStatus")
                        .HasColumnType("int");

                    b.Property<int>("TimeTemplateItemId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ScheduleId");

                    b.HasIndex("GoodsId");

                    b.HasIndex("PartnerId");

                    b.HasIndex("TimeTemplateItemId");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<string>("IdentificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsScheduled")
                        .HasColumnType("bit");

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<int?>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeOut")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionStatus")
                        .HasColumnType("int");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<float>("WeightIn")
                        .HasColumnType("real");

                    b.Property<float>("WeightOut")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("real")
                        .HasDefaultValue(0f);

                    b.HasKey("TransactionId");

                    b.HasIndex("GoodsId");

                    b.HasIndex("PartnerId");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.ActivityLog", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Account", "Account")
                        .WithMany("ActivityLogs")
                        .HasForeignKey("AccountUsername");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.Notification", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Partner", null)
                        .WithMany("Notifications")
                        .HasForeignKey("PartnerId");

                    b.HasOne("ImportExportManagement_API.Models.Account", "Account")
                        .WithMany("Notifications")
                        .HasForeignKey("Username");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.TimeTemplateItem", b =>
                {
                    b.HasOne("ImportExportManagementAPI.Models.TimeTemplate", "TimeTemplate")
                        .WithMany("TimeTemplateItems")
                        .HasForeignKey("TimeTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TimeTemplate");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Account", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.IdentityCard", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Partner", "Partner")
                        .WithMany("IdentityCards")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.InventoryDetail", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Goods", "Goods")
                        .WithMany("InventoryDetails")
                        .HasForeignKey("GoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Inventory", "Inventory")
                        .WithMany("InventoryDetails")
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Partner", "Partner")
                        .WithMany("InventoryDetails")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goods");

                    b.Navigation("Inventory");

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Partner", b =>
                {
                    b.HasOne("ImportExportManagementAPI.Models.PartnerType", "PartnerType")
                        .WithMany("Partners")
                        .HasForeignKey("PartnerTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Account", "Account")
                        .WithOne("Partner")
                        .HasForeignKey("ImportExportManagement_API.Models.Partner", "Username");

                    b.Navigation("Account");

                    b.Navigation("PartnerType");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Schedule", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Goods", "Goods")
                        .WithMany("Schedules")
                        .HasForeignKey("GoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Partner", "Partner")
                        .WithMany("Schedules")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagementAPI.Models.TimeTemplateItem", "TimeTemplateItem")
                        .WithMany("Schedules")
                        .HasForeignKey("TimeTemplateItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Goods");

                    b.Navigation("Partner");

                    b.Navigation("TimeTemplateItem");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Transaction", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Goods", "Goods")
                        .WithMany("Transactions")
                        .HasForeignKey("GoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Partner", "Partner")
                        .WithMany("Transactions")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.Schedule", "Schedule")
                        .WithOne("Transaction")
                        .HasForeignKey("ImportExportManagement_API.Models.Transaction", "ScheduleId");

                    b.Navigation("Goods");

                    b.Navigation("Partner");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.PartnerType", b =>
                {
                    b.Navigation("Partners");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.TimeTemplate", b =>
                {
                    b.Navigation("TimeTemplateItems");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.TimeTemplateItem", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Account", b =>
                {
                    b.Navigation("ActivityLogs");

                    b.Navigation("Notifications");

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Goods", b =>
                {
                    b.Navigation("InventoryDetails");

                    b.Navigation("Schedules");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Inventory", b =>
                {
                    b.Navigation("InventoryDetails");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Partner", b =>
                {
                    b.Navigation("IdentityCards");

                    b.Navigation("InventoryDetails");

                    b.Navigation("Notifications");

                    b.Navigation("Schedules");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Schedule", b =>
                {
                    b.Navigation("Transaction");
                });
#pragma warning restore 612, 618
        }
    }
}
