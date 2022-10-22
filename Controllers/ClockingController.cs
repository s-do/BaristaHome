using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<IActionResult> ClockingIn([Bind("ShiftStatusId")] UserShiftStatus userSS)
        {
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;

            /*_context.Add(userSS);
            await _context.SaveChangesAsync();*/


            //Get all the shift status names
            List<ShiftStatus> shiftStatus = (from ss in _context.ShiftStatus
                                             select ss).ToList();
            ViewBag.ShiftStatus = shiftStatus;

            return RedirectToAction("ClockIn", "Clocking");
        }

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
    }
}
