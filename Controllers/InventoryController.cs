using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {

        private readonly BaristaHomeContext _context;

        public InventoryController(BaristaHomeContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.Units = new SelectList(_context.Unit.ToList(), "UnitName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("ItemName")] Item item, [Bind("Quantity,PricePerUnit")] InventoryItem inventory)
        {
            

            


            // TODO: POST THAT USER SELECTION AND PUT IT IN HERE
            var unitId = (from u in _context.Unit
                          where u.UnitName.Equals("UsersSelectedUnitInput")
                          select u.UnitId).FirstOrDefault();

            //item.UnitId = unitId;

            item.UnitId = 1;

            // First add the new item into the Item db
            _context.Add(item);
            // _context.SaveChanges();
            await _context.SaveChangesAsync(); // use this to save changes to the db instead, so the method pauses until this task completes

            if (ModelState.IsValid)
            {
                // Get the item id so we can save this InventoryItem to it's repsective store
                var itemId = (from i in _context.Item
                              where i.ItemName.Equals(item.ItemName)
                              select i.ItemId).FirstOrDefault();

                // Add the item id to the InventoryItem object
                inventory.ItemId = itemId;
                // Now the StoreId, you can find the StoreId of the current logged in user by using this line
                inventory.StoreId = Convert.ToInt16(User.FindFirst("StoreId").Value);

                // Now we add the InventoryItem to the database with the correct reference to the store it belongs to and the item it's named
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                // return RedirectToAction("Index"); <-- the syntax of this method is RedirectToAction("ActionName", "ControllerName");

                return View();
            }

            ModelState.AddModelError(string.Empty, "There was an issue adding an item.");
            return View();
        }
















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
