using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        //Returns a view displaying a message that says there's no existing timers for the current store
        public async Task<IActionResult> NoTimers()
        {
            return View();
        }

        //Returns a view displaying all timers, or redirects to NoTimers() if there's no existing timer for the store
        public async Task<IActionResult> Timers()
        {

            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            var timers = (List<StoreTimer>)(from s in _context.Store
                                          join t in _context.StoreTimer on s.StoreId equals t.StoreId
                                          where s.StoreId == storeId
                                          select t).ToList();

            var listOfTimers = (from s in _context.Store
                                                  join t in _context.StoreTimer on s.StoreId equals t.StoreId
                                                  where s.StoreId == storeId
                                                  select t)
                                            .Select(t => new
                                            {
                                                Text = t.TimerName,
                                                Value = t.StoreTimerId
                                            });

            ViewBag.Timers = new SelectList(listOfTimers, "Value", "Text");
            //StoreTimer ? timer = await _context.StoreTimer.FindAsync(12);

            if (!timers.Any())
            {
                return RedirectToAction("NoTimers");
            }
            else
            {
                StoreTimer timer = timers.First();
                ViewBag.x = timers;
                
                //get all timers into a list. find out how many there are. 
                return View(timers);
            }
            
        }

        //Returns a page for creating a timer 
        public async Task<IActionResult> CreateTimer()
        {
            return View();
        }

        //Saves new timer to the database
        [HttpPost]
        public async Task<IActionResult> CreateTimer([Bind("TimerName, DurationMin")] StoreTimer timer)
        {
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            timer.StoreId = storeId;
            _context.Add(timer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Timers");
        }

        //Deletes a timer from the database
        public async Task<IActionResult> DeleteTimer(int timerId)
        {
            var timer = await _context.StoreTimer.FindAsync(timerId);
            _context.Remove(timer); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Timers");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
