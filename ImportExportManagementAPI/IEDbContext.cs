using ImportExportManagement_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            modelBuilder.Entity<InventoryDetail>().HasKey(p => new { p.GoodsId, p.InventoryId });

            //Partner
            modelBuilder.Entity<Partner>().HasOne(p => p.Account).WithOne(a => a.Partner).HasForeignKey<Partner>(p => p.Username);

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

    }
}
