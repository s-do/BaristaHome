using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class TimersController : Controller
    {
        //private readonly ILogger<TimersController> _logger;

        private readonly BaristaHomeContext _context;

        public TimersController(BaristaHomeContext context)
        {
            _context = context;
        }
        /*public TimersController(ILogger<TimersController> logger)
        {
            _logger = logger;
        }
        */
        public async Task<IActionResult> Timers()
        {
            var timer = await _context.StoreTimer.FindAsync(1);
            return View(timer);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimer([Bind("TimerName, DurationMin")] string name, int minutes)
        {
            StoreTimer timer = new StoreTimer();
            timer.TimerName = name;
            timer.DurationMin = minutes;
            _context.Add(timer);
            await _context.SaveChangesAsync();
            return View(timer);
        }
    }
}
