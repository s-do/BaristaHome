﻿using BaristaHome.Models;
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
        public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImageData,DrinkImage")] Drink home)
        {
            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Menu");
            }
            ModelState.AddModelError(string.Empty, home.DrinkName);
            return View(home);

        }

        // GET: Drink
        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            // Used to get drink list
            // Use type casting to return a IEnumerable<Model> with a LINQ query instead of doing await _context.Model.ToListAsync()
            var drinkList = (IEnumerable<Drink>)from d in _context.Drink
                                              orderby d.DrinkId descending
                                              select d;

            // To get tags from database
            List<Tag> tagQuery = (from tag in _context.Tag
                                      select new Tag
                                      { 
                                          TagName = tag.TagName
                                      }).ToList();
            ViewBag.TagList = tagQuery;

            return View(drinkList);
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
                                      /*join item in _context.Item on inventory.ItemId equals item.ItemId  */
                                      select new Tag
                                      {
                                          TagName = tag.TagName
                                      }).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
            }

            return View(drink);
        }

        public IActionResult Edititem()
        {
            return View();
        }


    }
}
