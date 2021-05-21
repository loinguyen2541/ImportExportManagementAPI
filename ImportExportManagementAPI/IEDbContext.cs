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
            modelBuilder.Entity<Partner>().HasOne(p => p.Account).WithOne(a => a.Partner).HasForeignKey<Partner>(p => p.Username).IsRequired(false);

            //IdentityCard
            modelBuilder.Entity<IdentityCard>().HasKey(i => i.IdentityCardId);

            //Transaction
            modelBuilder.Entity<Transaction>().Property(t => t.WeightOut).HasDefaultValue(0);
            modelBuilder.Entity<Transaction>().HasIndex(t => t.ScheduleId).IsUnique(false);

            //SystemConfig
            modelBuilder.Entity<SystemConfig>().HasKey(s => s.AttributeKey);

            //Notification
            modelBuilder.Entity<Notification>().HasKey(n => n.NotificationId);

            //ActivityLog
            modelBuilder.Entity<ActivityLog>().HasKey(n => n.ActivityLogId);

            //Notification
            modelBuilder.Entity<Notification>().HasOne(n => n.Account).WithMany(n => n.Notifications).HasForeignKey(n => n.Username);

            SystemConfig storgeCapcacity = new SystemConfig();
            storgeCapcacity.AttributeKey = AttributeKey.StorageCapacity.ToString();
            storgeCapcacity.AttributeValue = configuration.GetValue<String>("SystemConfigs:StorageCapacity");

            SystemConfig autoSchedule = new SystemConfig();
            autoSchedule.AttributeKey = AttributeKey.AutoSchedule.ToString();
            autoSchedule.AttributeValue = configuration.GetValue<String>("SystemConfigs:AutoSchedule");

            SystemConfig startWorking = new SystemConfig();
            startWorking.AttributeKey = AttributeKey.StartWorking.ToString();
            startWorking.AttributeValue = configuration.GetValue<String>("SystemConfigs:StartWorking");

            SystemConfig finishWorking = new SystemConfig();
            finishWorking.AttributeKey = AttributeKey.FinishWorking.ToString();
            finishWorking.AttributeValue = configuration.GetValue<String>("SystemConfigs:FinishWorking");

            SystemConfig startBreak = new SystemConfig();
            startBreak.AttributeKey = AttributeKey.StartBreak.ToString();
            startBreak.AttributeValue = configuration.GetValue<String>("SystemConfigs:StartBreak");

            SystemConfig finishBreak = new SystemConfig();
            finishBreak.AttributeKey = AttributeKey.FinishBreak.ToString();
            finishBreak.AttributeValue = configuration.GetValue<String>("SystemConfigs:FinishBreak");

            SystemConfig timeBetweenSlot = new SystemConfig();
            timeBetweenSlot.AttributeKey = AttributeKey.TimeBetweenSlot.ToString();
            timeBetweenSlot.AttributeValue = configuration.GetValue<String>("SystemConfigs:TimeBetweenSlot");

            SystemConfig maximumCanceledSchechule = new SystemConfig();
            maximumCanceledSchechule.AttributeKey = AttributeKey.MaximumCanceledSchechule.ToString();
            maximumCanceledSchechule.AttributeValue = configuration.GetValue<String>("SystemConfigs:MaximumCanceledSchechule");

            SystemConfig maximumSlot = new SystemConfig();
            maximumSlot.AttributeKey = AttributeKey.MaximumSlot.ToString();
            maximumSlot.AttributeValue = configuration.GetValue<String>("SystemConfigs:MaximumSlot ");

            modelBuilder.Entity<SystemConfig>().HasData(storgeCapcacity);
            modelBuilder.Entity<SystemConfig>().HasData(autoSchedule);
            modelBuilder.Entity<SystemConfig>().HasData(startBreak);
            modelBuilder.Entity<SystemConfig>().HasData(finishBreak);
            modelBuilder.Entity<SystemConfig>().HasData(startWorking);
            modelBuilder.Entity<SystemConfig>().HasData(finishWorking);
            modelBuilder.Entity<SystemConfig>().HasData(timeBetweenSlot);
            modelBuilder.Entity<SystemConfig>().HasData(maximumCanceledSchechule);
            modelBuilder.Entity<SystemConfig>().HasData(maximumSlot);
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
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
    }
}
