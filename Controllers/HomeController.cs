using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Pipelines;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BaristaHomeContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(BaristaHomeContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Getting store name of logged in user and displaying to view
            var storeName = (from user in _context.User
                             join store in _context.Store on user.StoreId equals store.StoreId
                             where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                             select store.StoreName).FirstOrDefault();

            ViewBag.StoreName = storeName;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}