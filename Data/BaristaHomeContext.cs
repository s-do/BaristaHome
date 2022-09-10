using Microsoft.EntityFrameworkCore;
using BaristaHome.Models;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Diagnostics.Metrics;

namespace BaristaHome.Data
{
    public class BaristaHomeContext : DbContext
    {
        public BaristaHomeContext(DbContextOptions<BaristaHomeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Creating candidate key for models (multiple columns for pk)
            builder.Entity<CategoryTask>().HasKey(table => new
            {
                table.CategoryId,
                table.StoreTaskId
            });

            builder.Entity<InventoryItem>().HasKey(table => new
            {
                table.ItemId,
                table.StoreId
            });

            builder.Entity<UserShiftStatus>().HasKey(table => new
            {
                table.UserId,
                table.ShiftStatusId,
                table.Time
            });

            // Setting decimal precision for Models with decimal properties (2 decimals)
            builder.Entity<InventoryItem>().Property(table =>  table.Quantity).HasPrecision(16, 2);
            builder.Entity<InventoryItem>().Property(table => table.PricePerUnit).HasPrecision(16, 2);

            builder.Entity<Payroll>().Property(table => table.Hours).HasPrecision(16, 2);
            builder.Entity<Payroll>().Property(table => table.Amount).HasPrecision(16, 2);

            builder.Entity<Sale>().Property(table => table.UnitsSold).HasPrecision(16, 2);
            builder.Entity<Sale>().Property(table => table.Profit).HasPrecision(16, 2);
        }

        // Data models to represent the database for querying/data manipulation
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryTask> CategoryTask { get; set; }
        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<InventoryItem> InventoryItem { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Sale> Sale { get; set; }
        public DbSet<ShiftStatus> ShiftStatus { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreTimer> StoreTimer { get; set; }
        public DbSet<StoreTask> StoreTask { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserShiftStatus> UserShiftStatus { get; set; }
    }
}