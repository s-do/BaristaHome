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
            return View();
        }

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

        //When a user decides to "Clock Out" their clock out time will be saved
        //User will be taken to "Not Clocked In" status page
        /*[HttpPost]
        public async Task<IActionResult> NotClockedIn(int? id)
        {
            UserShiftStatus userSS = new UserShiftStatus();
            userSS.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            DateTime time = DateTime.Now;
            userSS.Time = time;
            userSS.ShiftStatusId = (int)id;

            _context.Add(userSS);
            await _context.SaveChangesAsync();

            return RedirectToAction("NotClockedIn", "Clocking");
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

        [HttpPost]
        public async Task<IActionResult> ShiftStatus(int? id, bool a)
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
                //Alex vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
                var userID = Convert.ToInt16(User.FindFirst("UserId").Value);
                //Start of shift
                var shiftstart = (from uss in _context.UserShiftStatus
                             where uss.UserId == userID
                             where uss.ShiftStatus.ShiftStatusId == 1
                             orderby uss.Time
                             select uss).Last();
                //End of shift
                var shiftend = userSS;

                var startTime = shiftstart.Time;
                var endTime = shiftend.Time;

                //Check if there were any breaks
                var count = (from uss in _context.UserShiftStatus
                             where uss.UserId == userID
                             where uss.ShiftStatus.ShiftStatusId == 3
                             orderby uss.Time
                             select uss).Count();
                var startBreak = new DateTime();
                var endBreak = new DateTime();
                if (count > 0)
                {
                    //Start of break
                    var shiftstartbreak = (from uss in _context.UserShiftStatus
                                           where uss.UserId == userID
                                           where uss.ShiftStatus.ShiftStatusId == 3
                                           orderby uss.Time
                                           select uss).Last();

                    if (shiftstartbreak.Time > startTime && shiftstartbreak.Time < endTime)
                    {
                        startBreak = shiftstartbreak.Time;
                        //an end of break must exist of a break has started
                        //End of break
                        var shiftendbreak = (from uss in _context.UserShiftStatus
                                             where uss.UserId == userID
                                             where uss.ShiftStatus.ShiftStatusId == 4
                                             orderby uss.Time
                                             select uss).Last();

                        if (shiftendbreak.Time > startBreak && shiftendbreak.Time < endTime)
                        {
                            endBreak = shiftendbreak.Time;
                        }
                    }
                }
                TimeSpan totalTime = new TimeSpan();
                if (startBreak.Year == 1)
                {
                    totalTime = (startTime - endTime).Duration();
                }
                else
                {
                    totalTime = (startTime - startBreak).Duration() + (endBreak - endTime).Duration();
                }

                var wage = (from u in _context.User
                            where u.UserId == userID
                            select u.Wage).First();

                if (wage == null)
                {
                    wage = 0;
                }


                //Calculate total hours
                double hours = totalTime.Hours;
                double minHours = totalTime.Minutes / 60.00;
                double secHours = totalTime.Seconds / 3600.00;
/*              //Testing  
                double hours = 2.00;
                double minHours = 30.00 / 60.00;
                double secHours = 0.00 / 3600.00;*/
                double totalTimeWorked = Math.Round(hours + minHours + secHours, 2);
                double totalPay = Math.Round((Convert.ToDouble(wage) * totalTimeWorked), 2);
                var dateWorked = startTime.Date;


                
                Payroll payroll = new Payroll();
                payroll.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);
                payroll.Amount = Convert.ToDecimal(totalPay);
                payroll.Date = dateWorked;
                payroll.Hours = Convert.ToDecimal(totalTimeWorked);

                _context.Add(payroll);
                await _context.SaveChangesAsync();
                //Alex ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


                return RedirectToAction("NotClockedIn", "Clocking");
            }

        }
    }
}
