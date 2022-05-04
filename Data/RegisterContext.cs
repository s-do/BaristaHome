using Microsoft.EntityFrameworkCore;
using BaristaHome.Models;

namespace BaristaHome.Data
{
    public class RegisterContext : DbContext
    {
        public RegisterContext(DbContextOptions<RegisterContext> options)
            : base(options)
        {
        }

        public DbSet<RegisterViewModel> Register { get; set; }
    }
}