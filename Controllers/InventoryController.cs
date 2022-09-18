using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;

using System.Collections;

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


        public IActionResult Index()
        {
            // dw, with the utilization of ItemViewModel we can display a store's inventory instead
            // https://stackoverflow.com/questions/57727635/how-to-pass-selected-query-list-using-viewbag i used this link to display list of store's items
            List<ItemViewModel> itemQuery = (from store in _context.Store
                                             join inventory in _context.InventoryItem on store.StoreId equals inventory.StoreId // link store and inventoryitem by storeid
                                             join item in _context.Item on inventory.ItemId equals item.ItemId                  // link inventoryitem and item by itemid
                                             where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))       // filter items by user's store
                                             select new ItemViewModel
                                             {
                                                    Name = item.ItemName,                  // now we can send a 
                                                    Quantity = inventory.Quantity,         // ItemViewModel object
                                                    PricePerUnit = inventory.PricePerUnit  // to the view
                                             }).ToList();
            ViewBag.Inventory = itemQuery;
            return View();


            /*  ViewBag.Units = new SelectList(_context.Unit.ToList(), "UnitName");*//*

            List<Unit> uni = new List<Unit>();
            uni = _context.Unit.ToList();

            List<SelectListItem> Units = new List<SelectListItem>();
            
                *//*new SelectListItem(){Text="Fluid Ounces",Value ="fl"}*//*
            foreach( var u in uni)
            {
                Units.Add(new SelectListItem
                    {
                        Text = u.UnitName,
                        Value = u.UnitId.ToString()
                    });
            }

            ViewBag.UnitList = Units;
            return View();*/
        }

        [HttpPost]
        public async Task<IActionResult> Index(ItemViewModel itemViewModel)
        {
            List<ItemViewModel> itemQuery = (from store in _context.Store
                                             join inventory in _context.InventoryItem on store.StoreId equals inventory.StoreId // link store and inventoryitem by storeid
                                             join item in _context.Item on inventory.ItemId equals item.ItemId                  // link inventoryitem and item by itemid
                                             where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))       // filter items by user's store
                                             select new ItemViewModel
                                             {
                                                 Name = item.ItemName,                  // now we can send a 
                                                 Quantity = inventory.Quantity,         // ItemViewModel object
                                                 PricePerUnit = inventory.PricePerUnit  // to the view
                                             }).ToList();
            ViewBag.Inventory = itemQuery;

            // Look at our properties for ItemViewModel.cs under the Models folder, you can see how I use ViewModels to generate a view to input
            // those fields so I can validate it existing in the actual database tables in the controller here

            if (ModelState.IsValid)
            {
                // Get the id of the unit name         
                var unitId = (from u in _context.Unit
                              where u.UnitName.Equals(itemViewModel.UnitName)
                              select u.UnitId).FirstOrDefault();

                // Now check if the item name exists in the database
                var existingItem = (from i in _context.Item
                                    where i.ItemName.Equals(itemViewModel.Name)
                                    select i).FirstOrDefault();

                if (existingItem != null)
                {
                    // Okay the item exists in our Item table, now we check if this item is already in THIS STORE
                    var existingInventoryItem = (from inv in _context.InventoryItem
                                                 where inv.ItemId.Equals(existingItem.ItemId)
                                                 select inv).FirstOrDefault();

                    if (existingInventoryItem != null)
                    {
                        ModelState.AddModelError(string.Empty, "You already have this item in your inventory! Use Edit to change the quantity instead.");
                        return View(itemViewModel); // We can sort of fix this later, the error above pops up when you reopen the modal (i don't know how to send the view back with it still open)
                    }

                    // Alright so if the item exists in Item but it isn't inside InventoryItem yet, instantiate a new InventoryItem with the inputted fields and Add()
                    var inventoryItem = new InventoryItem()
                    {
                        ItemId = existingItem.ItemId,
                        StoreId = Convert.ToInt32(User.FindFirst("StoreId").Value),
                        Quantity = itemViewModel.Quantity,
                        PricePerUnit = itemViewModel.PricePerUnit
                    };
                    _context.Add(inventoryItem);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    // existingItem is null, so we add it into the Item table
                    var newItem = new Item()
                    {
                        ItemName = itemViewModel.Name,
                        UnitId = unitId
                    };
                    _context.Add(newItem);
                    await _context.SaveChangesAsync();

                    // And then add it to the store's InventoryItem table
                    var inventoryItem = new InventoryItem()
                    {
                        ItemId = newItem.ItemId, // The new item id we just added into the database
                        StoreId = Convert.ToInt32(User.FindFirst("StoreId").Value),
                        Quantity = itemViewModel.Quantity,
                        PricePerUnit = itemViewModel.PricePerUnit
                    };
                    _context.Add(inventoryItem);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Inventory"); // new InventoryItem should be successfully added into the user's store
            }
            ModelState.AddModelError(string.Empty, "There was an issue adding this item into your inventory.");
            return View();

            // old code you can delete or save for reference ig

            /*// First add the new item into the Item db
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
            }*/


        }
















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
