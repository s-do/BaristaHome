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

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
    }
}