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
        private IConfigurationRoot configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLoggerFactory(MyLoggerFactory)
            //    .UseSqlServer(@"Data Source =.\; Initial Catalog = ExportImportManagement; Integrated Security = True")
            configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
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

            //IdentityCard
            modelBuilder.Entity<IdentityCard>().HasKey(i => i.IdentityCardId);

            //Transaction
            modelBuilder.Entity<Transaction>().HasOne(t => t.IdentityCard).WithMany(c => c.Transactions).HasForeignKey(t => t.IdentityCardId);
            modelBuilder.Entity<Transaction>().Property(t => t.WeightOut).HasDefaultValue(0);

            //SystemConfig
            modelBuilder.Entity<SystemConfig>().HasKey(s => s.AttributeKey);

            SystemConfig storgeCapcacity = new SystemConfig();
            storgeCapcacity.AttributeKey = AttributeKey.StorageCapacity.ToString();
            storgeCapcacity.AttributeValue = configuration.GetValue<String>("SystemConfigs:StorageCapacity");

            SystemConfig autoSchedule = new SystemConfig();
            autoSchedule.AttributeKey = AttributeKey.AutoSchedule.ToString();
            autoSchedule.AttributeValue = configuration.GetValue<String>("SystemConfigs:AutoSchedule"); ;

            modelBuilder.Entity<SystemConfig>().HasData(storgeCapcacity);
            modelBuilder.Entity<SystemConfig>().HasData(autoSchedule);
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
        public DbSet<TimeTemplate> TimeTemplate { get; set; }
        public DbSet<TimeTemplateItem> TimeTemplateItem { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }
    }
}
