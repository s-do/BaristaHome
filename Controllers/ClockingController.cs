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

        //Displays the admin view for clocking system
        [HttpGet]
        public IActionResult Clocking()
        {
            return View();
        }

        //Displays the view for when a user is not clocked in or is clocked out
        [HttpGet]
        public IActionResult NotClockedIn()
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

        //Displays the view for when a user is clocked in
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

        ////Displays the view for when a user is on break
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

        //Displays current shift status view after returning back to the Clocking View 
        [HttpGet]
        public IActionResult ShiftStatus(int? id)
        {
            //Clock In and End Break
            //Show status as "working"
            if (id == 1 || id == 4)
            {
                return RedirectToAction("ClockIn", "Clocking");
            }
            //Start Break
            //Shows status as "taking break"
            else if (id == 3)
            {
                return RedirectToAction("StartBreak", "Clocking");
            }
            //Clock Out (id == 2)
            //Shows status as "not clocked in"
            else
            {
                return RedirectToAction("NotClockedIn", "Clocking");
            }
        }

        //Saves user shift status information to db and takes user to the corresponding page based on the button clicked
        [HttpPost]
        public async Task<IActionResult> ShiftStatus(int? id, bool a)
        {
            //UserShiftStatus to be added to db
            UserShiftStatus userSS = new UserShiftStatus();

            //User Id
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);

            //Time user clicked button
            DateTime time = DateTime.Now;
            userSS.Time = time;

            //ShiftStatus
            userSS.ShiftStatusId = (int)id;

            //Save to db
            _context.Add(userSS);
            await _context.SaveChangesAsync();

            //Clock In and End Break
            if (id == 1 || id == 4)
            {
                //When a user decides to "Clock In" their clock in time will be saved
                //Or when a user clicks on "End Break" the end of their break time will be saved
                //User will be taken to "Working" status page
                return RedirectToAction("ClockIn", "Clocking");
            }
            //Start Break
            else if (id == 3)
            {
                //When a user decides to "Start Break" their break time will be saved
                //User will be taken to "Taking Break" status page
                return RedirectToAction("StartBreak", "Clocking");
            }
            //Clock Out (id == 2)
            else
            {
                //When a user decides to "Clock Out" their clock out time will be saved
                //User will be taken to "Not Clocked In" status page
                return RedirectToAction("NotClockedIn", "Clocking");
            }

        }
    }
}
