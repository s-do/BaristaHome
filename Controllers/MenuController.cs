using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer;
using BaristaHome.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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
/*        public IActionResult Menu()
        {
            return View();
        }*/

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
        public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImageData,DrinkImage,StoreId")] Drink drink)
        {
/*            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            drink.StoreId = storeId;*/

            if (ModelState.IsValid)
            {
                _context.Add(drink);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Menu");
            }
            ModelState.AddModelError(string.Empty, drink.DrinkName);
            return View(drink);

        }

        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            // Used to get drink list
            // Use type casting to return a IEnumerable<Model> with a LINQ query instead of doing await _context.Model.ToListAsync()
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            var drinkList = (IEnumerable<Drink>)from d in _context.Drink
                                                where d.StoreId == storeId
                                                orderby d.DrinkId descending
                                                select d;

            // To get tags from database
            var tags = (IEnumerable<Tag>)(from s in _context.Store
                              join d in _context.Drink on s.StoreId equals d.StoreId
                              join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                              join t in _context.Tag on dt.TagId equals t.TagId
                              select t);
            ViewData["Tags"] = new SelectList(tags.Distinct(), "TagId", "TagName");
            /*List<Tag> tagQuery = (from tag in _context.Tag
                                  select new Tag
                                  {
                                      TagName = tag.TagName
                                  }).ToList();
            ViewBag.TagList = tagQuery;*/

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Menu(IEnumerable<Tag> tagList)
        {
            foreach(var tag in tagList)
            {
                Console.WriteLine(tag.ToString());
            }
            return View(tagList);
            
        }


        // GET: Drink Details
        public async Task<IActionResult> Drink(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == id);
            if (drink == null)
            {
                return NotFound();
            }
            else
            {
                List<Tag> drinkTagQuery = (from d in _context.Drink
                                      join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                      join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                      where d.DrinkId == drink.DrinkId
                                      select new Tag
                                      {
                                          TagName = tag.TagName
                                      }).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
            }

            return View(drink);
        }

        // GET: Drink Details
        public async Task<IActionResult> EditItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == id);
            if (drink == null)
            {
                return NotFound();
            }
            else
            {
                List<Tag> drinkTagQuery = (from d in _context.Drink
                                           join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                           join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                           where d.DrinkId == drink.DrinkId
                                           /*join item in _context.Item on inventory.ItemId equals item.ItemId  */
                                           select new Tag
                                           {
                                               TagName = tag.TagName
                                           }).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
            }

            return View(drink);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem([Bind("DrinkId,DrinkName,Description,Instructions,DrinkImageData,DrinkImage,StoreId")] Drink drink)
        {
            var existingDrink = (from d in _context.Drink
                                 where d.DrinkName.Equals(drink.DrinkName) && !d.DrinkId.Equals(drink.DrinkId)
                                 select d).FirstOrDefault();

            if (existingDrink != null)
            {
                ModelState.AddModelError(string.Empty, "Drink name in use");
                return View(drink);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Menu", "Menu");
            }
            return View(drink);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var registerViewModel = await _context.Drink.FindAsync(id);
            _context.Drink.Remove(registerViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Menu", "Menu");
        }




    }
}
