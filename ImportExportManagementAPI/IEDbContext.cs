using ImportExportManagement_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportExportManagementAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ImportExportManagement_API
{
    public class IEDbContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(MyLoggerFactory)
            //    .UseSqlServer(@"Data Source =.\; Initial Catalog = ExportImportManagement; Integrated Security = True")
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseLoggerFactory(MyLoggerFactory).UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Account
            modelBuilder.Entity<Account>().HasKey(a => a.Username);

            //InventoryDetail
            modelBuilder.Entity<InventoryDetail>().HasKey(p => p.InventoryDetailId);

            //Inventory
            modelBuilder.Entity<Inventory>()
                    .Property(e => e.RecordedDate)
                    .HasColumnType("Date");

            //Partner
            modelBuilder.Entity<Partner>().HasOne(p => p.Account).WithOne(a => a.Partner).HasForeignKey<Partner>(p => p.Username);

            //Schedule
            modelBuilder.Entity<Schedule>().Property(s => s.IsCanceled).HasDefaultValue(false);

            //IdentityCard
            modelBuilder.Entity<IdentityCard>().HasKey(i => i.IdentityCardId);

            //PartnerPartnerType
            modelBuilder.Entity<PartnerPartnerType>().HasKey(p => new { p.PartnerId, p.PartnerTypeId });

            modelBuilder.Entity<Partner>()
            .HasMany(p => p.PartnerTypes)
            .WithMany(p => p.Partners)
            .UsingEntity<PartnerPartnerType>(
                j => j
                    .HasOne(pt => pt.PartnerType)
                    .WithMany(t => t.PartnerPartnerTypes)
                    .HasForeignKey(pt => pt.PartnerTypeId),
                j => j
                    .HasOne(pt => pt.Partner)
                    .WithMany(p => p.PartnerPartnerTypes)
                    .HasForeignKey(pt => pt.PartnerId)
         );

            //Transaction
            modelBuilder.Entity<Transaction>().HasOne(t => t.IdentityCard).WithMany(c => c.Transactions).HasForeignKey(t => t.IdentityCardId);
            modelBuilder.Entity<Transaction>().Property(t => t.WeightOut).HasDefaultValue(0);
        }

        public DbSet<Partner> Partner { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<IdentityCard> IdentityCard { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryDetail> InventoryDetail { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<ImportExportManagementAPI.Models.PartnerType> PartnerType { get; set; }
        public DbSet<ImportExportManagementAPI.Models.PartnerPartnerType> PartnerPartnerType { get; set; }
        public DbSet<TimeTemplate> TimeTemplate { get; set; }
        public DbSet<TimeTemplateItem> TimeTemplateItem { get; set; }
    }
}
