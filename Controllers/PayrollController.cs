using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly ILogger<PayrollController> _logger;
        public PayrollController(ILogger<PayrollController> logger)
        {
            _logger = logger;
        }
        public IActionResult Owner()
        {
            return View();
        }

        public IActionResult Worker()
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
