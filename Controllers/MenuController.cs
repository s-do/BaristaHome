using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer;
using BaristaHome.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BaristaHome.Controllers
{
    [Authorize]
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

        /*        [HttpPost]
                public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImage")] Drink home)
                {
                    //byte[] bytes = System.IO.File.ReadAllBytes(home.DrinkImage);
                    //home.DrinkImageData = bytes;

                    if (ModelState.IsValid)
                    {
                        _context.Add(home);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Menu", "Menu");
                    }
                    ModelState.AddModelError(string.Empty, home.DrinkName);
                    return View(home);
                }*/

        [HttpPost]
        public async Task<IActionResult> AddItem(List<IFormFile> files, [Bind("DrinkName,Instructions,Description,DrinkImage")] Drink home)
        {
            files.Count();
            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        home.DrinkImageData = fileBytes;
                        string s = Convert.ToBase64String(fileBytes);
                        home.DrinkImage = s;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Menu");
            }
            ModelState.AddModelError(string.Empty, home.DrinkName);
            return View(home);

        }


    }
}
