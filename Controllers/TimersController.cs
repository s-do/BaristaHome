using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class TimersController : Controller
    {
        private readonly ILogger<TimersController> _logger;
        public TimersController(ILogger<TimersController> logger)
        {
            _logger = logger;
        }
        public IActionResult Timers()
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
