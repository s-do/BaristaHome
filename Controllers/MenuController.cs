using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer;
using BaristaHome.Data;

namespace BaristaHome.Controllers
{
    public class MenuController : Controller
    {
        private readonly BaristaHomeContext _context;

        public MenuController(BaristaHomeContext context)
        {
            _context = context;
        }
        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Additem()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImage")] Drink home)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(home.DrinkImage);
            home.DrinkImageData = bytes;

            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Menu");
            }
            ModelState.AddModelError(string.Empty, home.DrinkName);
            return View(home);
        }
        public class Image
        {
            public int Id { get; set; }
            public string ImageTitle { get; set; }
            public byte[] ImageData { get; set; }
        }
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<Image> Images { get; set; }
        }


    }
}
