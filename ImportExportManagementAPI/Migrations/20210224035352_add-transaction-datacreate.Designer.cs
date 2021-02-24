﻿// <auto-generated />
using System;
using ImportExportManagement_API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ImportExportManagementAPI.Migrations
{
    [DbContext(typeof(IEDbContext))]
    [Migration("20210224035352_add-transaction-datacreate")]
    partial class addtransactiondatacreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ImportExportManagementAPI.Models.PartnerPartnerType", b =>
                {
                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<int>("PartnerTypeId")
                        .HasColumnType("int");

                    b.HasKey("PartnerId", "PartnerTypeId");

                    b.HasIndex("PartnerTypeId");

                    b.ToTable("PartnerPartnerType");
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

            modelBuilder.Entity("ImportExportManagement_API.Models.Account", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

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
                        .HasColumnType("nvarchar(450)");

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

                    b.Property<DateTime>("RecordedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("InventoryId");

                    b.ToTable("Inventory");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.InventoryDetail", b =>
                {
                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<int>("InventoryId")
                        .HasColumnType("int");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.HasKey("GoodsId", "InventoryId");

                    b.HasIndex("InventoryId");

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

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PartnerId");

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
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCanceled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("PartnerId")
                        .HasColumnType("int");

                    b.Property<float>("RegisteredWeight")
                        .HasColumnType("real");

                    b.Property<DateTime>("ScheduleDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ScheduleId");

                    b.HasIndex("GoodsId");

                    b.HasIndex("PartnerId");

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

                    b.Property<int>("GoodsId")
                        .HasColumnType("int");

                    b.Property<string>("IdentityCardId")
                        .HasColumnType("nvarchar(450)");

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
                        .HasColumnType("real");

                    b.HasKey("TransactionId");

                    b.HasIndex("GoodsId");

                    b.HasIndex("IdentityCardId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.PartnerPartnerType", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Partner", "Partner")
                        .WithMany("PartnerPartnerTypes")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagementAPI.Models.PartnerType", "PartnerType")
                        .WithMany("PartnerPartnerTypes")
                        .HasForeignKey("PartnerTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Partner");

                    b.Navigation("PartnerType");
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

                    b.Navigation("Goods");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Partner", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Account", "Account")
                        .WithOne("Partner")
                        .HasForeignKey("ImportExportManagement_API.Models.Partner", "Username");

                    b.Navigation("Account");
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

                    b.Navigation("Goods");

                    b.Navigation("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Transaction", b =>
                {
                    b.HasOne("ImportExportManagement_API.Models.Goods", "Goods")
                        .WithMany("Transactions")
                        .HasForeignKey("GoodsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ImportExportManagement_API.Models.IdentityCard", "IdentityCard")
                        .WithMany("Transactions")
                        .HasForeignKey("IdentityCardId");

                    b.Navigation("Goods");

                    b.Navigation("IdentityCard");
                });

            modelBuilder.Entity("ImportExportManagementAPI.Models.PartnerType", b =>
                {
                    b.Navigation("PartnerPartnerTypes");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Account", b =>
                {
                    b.Navigation("Partner");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Goods", b =>
                {
                    b.Navigation("InventoryDetails");

                    b.Navigation("Schedules");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.IdentityCard", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Inventory", b =>
                {
                    b.Navigation("InventoryDetails");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Partner", b =>
                {
                    b.Navigation("IdentityCards");

                    b.Navigation("PartnerPartnerTypes");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("ImportExportManagement_API.Models.Role", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
