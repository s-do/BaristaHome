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
        public async Task<IActionResult> Clocking()
        {
            var latestStatus = (from s in _context.Store
                                 join u in _context.User on s.StoreId equals u.StoreId
                                 join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                 join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                 where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                                 group us by us.UserId into uss
                                 select new UserShiftStatus()
                                 {
                                     UserId = uss.Key,
                                     ShiftStatusId = uss.First(y => y.Time == uss.Max(x => x.Time)).ShiftStatusId,
                                     Time = uss.Max(x => x.Time),
                                 }).ToList();

            List<ClockingViewModel> list = new List<ClockingViewModel>();
            foreach(var status in latestStatus)
            {
                var statusView = (from s in _context.Store
                                  join u in _context.User on s.StoreId equals u.StoreId
                                  join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                  join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                  where us.UserId == status.UserId && us.ShiftStatusId == status.ShiftStatusId
                                  select new ClockingViewModel()
                                  {
                                      UserId = u.UserId,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      ShiftStatus = ss.ShiftStatusName,
                                      Time = status.Time,
                                  }).FirstOrDefault();
                list.Add(statusView);
            }
            /*from us in _context.UserShiftStatus
                                 join u in _context.User on us.UserId equals u.UserId
                                 join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId*/


            ViewBag.LatestStatus = list;

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


                //When a user decides to "Clock Out" their clock out time will be saved
                //User will be taken to "Not Clocked In" status page
                return RedirectToAction("NotClockedIn", "Clocking");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ViewStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var allStatus = (from s in _context.Store
                                join u in _context.User on s.StoreId equals u.StoreId
                                join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && us.UserId == user.UserId
                                select new ClockingViewModel
                                {
                                    UserId = u.UserId,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    ShiftStatusId = us.ShiftStatusId,
                                    ShiftStatus = ss.ShiftStatusName,
                                    Time = us.Time,
                                }).ToList();
            ViewBag.AllStatus = allStatus;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStatus(int userId, int statusId, DateTime time)
        {
            var userShiftStatus = await _context.UserShiftStatus.FindAsync(userId, statusId, time);
            _context.UserShiftStatus.Remove(userShiftStatus);
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewStatus", "Clocking");
        }
    }
}
