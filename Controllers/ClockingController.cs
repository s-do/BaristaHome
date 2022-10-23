using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaristaHome.Controllers
{
    public class ClockingController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ClockingController(BaristaHomeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Clocking()
        {
            // Getting current user's first name
            var firstName = (from u in _context.User
                             where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                             select u.FirstName).FirstOrDefault();
            ViewBag.FirstName = firstName;


            // Getting current user's last name
            var lastName = (from u in _context.User
                            where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                            select u.LastName).FirstOrDefault();

            ViewBag.LastName = lastName;

            //Get all the shift status names
            List<ShiftStatus> shiftStatus = (from ss in _context.ShiftStatus
                                             select ss).ToList();
            ViewBag.ShiftStatus = shiftStatus;

            return View();
        }

        //When a user decides to "Clock Out" their clock out time will be saved
        //User will be taken to "Not Clocked In" status page
        /*[HttpPost]
        public async Task<IActionResult> Clocking(int? id)
        {
            UserShiftStatus userSS = new UserShiftStatus();
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;
            userSS.ShiftStatusId = (int)id;

            _context.Add(userSS);
            await _context.SaveChangesAsync();

            return RedirectToAction("Clocking", "Clocking");
        }*/

        [HttpGet]
        public IActionResult ClockIn()
        {
            // Getting current user's first name
            var firstName = (from u in _context.User
                             where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                             select u.FirstName).FirstOrDefault();
            ViewBag.FirstName = firstName;


            // Getting current user's last name
            var lastName = (from u in _context.User
                            where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                            select u.LastName).FirstOrDefault();

            ViewBag.LastName = lastName;

            //Get all the shift status names
            List<ShiftStatus> shiftStatus = (from ss in _context.ShiftStatus
                                             select ss).ToList();
            ViewBag.ShiftStatus = shiftStatus;
            return View();
        }

        //When a user decides to "Clock In" their clock in time will be saved
        //Or when a user clicks on "End Break" the end of their break time will be saved
        //User will be taken to "Working" status page
        /*[HttpPost]
        public async Task<IActionResult> ClockIn(int? id)
        {
            UserShiftStatus userSS = new UserShiftStatus();
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;
            userSS.ShiftStatusId = (int)id;

            _context.Add(userSS);
            await _context.SaveChangesAsync();

            return RedirectToAction("ClockIn", "Clocking");
        }*/

        [HttpGet]
        public IActionResult StartBreak()
        {
            // Getting current user's first name
            var firstName = (from u in _context.User
                             where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                             select u.FirstName).FirstOrDefault();
            ViewBag.FirstName = firstName;


            // Getting current user's last name
            var lastName = (from u in _context.User
                            where u.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                            select u.LastName).FirstOrDefault();

            ViewBag.LastName = lastName;

            //Get all the shift status names
            List<ShiftStatus> shiftStatus = (from ss in _context.ShiftStatus
                                             select ss).ToList();
            ViewBag.ShiftStatus = shiftStatus;
            return View();
        }

        //When a user decides to "Start Break" their break time will be saved
        //User will be taken to "Taking Break" status page
        /*[HttpPost]
        public async Task<IActionResult> StartBreak(int? id)
        {
            UserShiftStatus userSS = new UserShiftStatus();
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;
            userSS.ShiftStatusId = (int)id;

            _context.Add(userSS);
            await _context.SaveChangesAsync();

            return RedirectToAction("StartBreak", "Clocking");
        }*/

        [HttpPost]
        public async Task<IActionResult> ShiftStatus(int? id)
        {
            UserShiftStatus userSS = new UserShiftStatus();
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;
            userSS.ShiftStatusId = (int)id;

            _context.Add(userSS);
            await _context.SaveChangesAsync();

            //Clock In and End Break
            //Show status as "working"
            if (id == 1 || id == 4)
            {
                return RedirectToAction("ClockIn", "Clocking");
            }
            //Clock Out
            //Shows status as "not clocked in"
            else if (id == 2)
            {
                return RedirectToAction("Clocking", "Clocking");
            }
            //Start Break
            //Shows status as "taking break"
            else
            {
                return RedirectToAction("StartBreak", "Clocking");
            }

        }
    }
}
