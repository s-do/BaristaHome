using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {

        private Data.BaristaHomeContext _context;

        public InventoryController(BaristaHomeContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var inventories = _context.InventoryItem.ToList();
            ViewBag.Inventories = inventories;
            return View();
        }

















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
