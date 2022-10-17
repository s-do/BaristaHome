using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly ILogger<PayrollController> _logger;
        private readonly BaristaHomeContext _context;
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


        [HttpGet]
        public async Task<IActionResult> Worker(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User.FirstOrDefault(m => m.UserId == id);
            /*            var worker = (from u in _context.User
                                        where u.UserId.Equals(id)
                                        select u).FirstOrDefault();*/


            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }
    }
}
