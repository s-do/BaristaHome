using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BaristaHome.Data;
using System;
using System.Linq;
using System.Web.Helpers;
using System.Drawing.Printing;

namespace BaristaHome.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BaristaHomeContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<BaristaHomeContext>>()))
            {
                // Look for any movies.
                if (context.User.Any())
                {
                    Console.WriteLine("Already seeded");
                    return;   // DB has been seeded
                }

                context.User.AddRange(
                  new User
                  {
                      FirstName = "John",
                      LastName = "Nguyen",
                      Email = "Johnpass123!@gmail.com",
                      Password = Crypto.HashPassword("Johnpass123!"),
                      Color = "#000000",
                      InviteCode = "12345"
                  },

                  new User
                  {
                      FirstName = "Test",
                      LastName = "Name",
                      Email = "test@gmail.com",
                      Password = Crypto.HashPassword("Testpass123!"),
                      Color = "#000000",
                      InviteCode = "12345"
                  },

                  new User
                  {
                      FirstName = "Fei",
                      LastName = "Hoffman",
                      Email = "feihoffman@gmail.com",
                      Password = Crypto.HashPassword("Feipass123!"),
                      Color = "#000000",
                      InviteCode = "12345"
                  }
                );
                context.SaveChanges();
            }
        }
    }
}