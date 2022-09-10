using Microsoft.EntityFrameworkCore;
using BaristaHome.Models;

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
            builder.Entity<UserShiftStatus>().HasKey(table => new
            {
                table.UserId,
                table.ShiftStatusId,
                table.Time
            });
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<ShiftStatus> ShiftStatus { get; set; }
        public DbSet<UserShiftStatus> UserShiftStatus { get; set; }
    }
}