using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BaristaHome.Data;
using System;
using System.Linq;
using System.Web.Helpers;

namespace BaristaHome.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RegisterContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<RegisterContext>>()))
            {
                // Look for any movies.
                if (context.Register.Any())
                {
                    return;   // DB has been seeded
                }

                context.Register.AddRange(
                  new RegisterViewModel
                  {
                      FirstName = "John",
                      LastName = "Nguyen",
                      Email = "johnnguyen@gmail.com",
                      Password = Crypto.HashPassword("Johnpass123!"),
                  },

                  new RegisterViewModel
                  {
                      FirstName = "Test",
                      LastName = "Name",
                      Email = "test@gmail.com",
                      Password = Crypto.HashPassword("Testpass123!"),
                  },

                  new RegisterViewModel
                  {
                      FirstName = "Fei",
                      LastName = "Hoffman",
                      Email = "feihoffman@gmail.com",
                      Password = Crypto.HashPassword("Feipass123!"),
                  }
                );
                context.SaveChanges();
            }
        }
    }
}