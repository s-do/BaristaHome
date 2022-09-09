using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    public class ClockingController : Controller
    {
        private readonly ILogger<ClockingController> _logger;
        public ClockingController(ILogger<ClockingController> logger)
        {
            _logger = logger;
        }
        public IActionResult Clocking()
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
