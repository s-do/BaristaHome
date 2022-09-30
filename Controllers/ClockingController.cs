using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    public class ClockingController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ClockingController(BaristaHomeContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Clocking()
        {
            var tags = await (from s in _context.Store
                       join d in _context.Drink on s.StoreId equals d.StoreId
                       join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                       join t in _context.Tag on dt.TagId equals t.TagId
                       select t).ToListAsync();
            ViewData["Tags"] = new SelectList(tags.Distinct(), "TagId", "TagName");
            //    join store in _context.Store on user.StoreId equals store.StoreId
            var userList = (IEnumerable<User>)from u in _context.User
                                              orderby u.UserId descending
                                              select u;

            //ViewBag.Workers = await _context.User.ToListAsync();
            return View(userList);
        }

        [HttpPost]
        public async Task<IActionResult> Clocking(List<string> tagList)
        {
            Console.WriteLine("Here's all the selected workers IDS YaHAYEEtT");
            foreach (string u in tagList)
            {
                Console.WriteLine(u);
            }
            return RedirectToAction("Index", "Calendar");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
