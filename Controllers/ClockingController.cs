using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web.Helpers;
using System.Transactions;
using System.Runtime.CompilerServices;

namespace BaristaHome.Controllers
{
    public class ClockingController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ClockingController(BaristaHomeContext context)
        {
            _context = context;
        }

        /*SELINAvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        //Displays the admin view for clocking system
        [HttpGet]
        public async Task<IActionResult> Clocking()
        {
            //List of UserShiftStatus objects where it gets the most recent time a user changed their work status
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

            //Gets the information of a user based on the latestStatus list
            List<ClockingViewModel> list = new List<ClockingViewModel>();
            foreach (var status in latestStatus)
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
            ViewBag.LatestStatus = list;

            //List of all users in a store
            var listOfUsers = (from s in _context.Store
                               join u in _context.User on s.StoreId equals u.StoreId
                               where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                               select u).ToList();
            ViewBag.ListOfUsers = listOfUsers;
            

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
            /*SELINA^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
           
            //Clock Out (id == 2)
            else
            {
                //Alex vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
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
                //Alex ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


                //When a user decides to "Clock Out" their clock out time will be saved
                //User will be taken to "Not Clocked In" status page
                return RedirectToAction("NotClockedIn", "Clocking");
            }

        }

        /*SELINAvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        //Gets all the work status changes of a user 
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

            //List of a user's information and the history of their working status changes
            var allStatus = (from s in _context.Store
                             join u in _context.User on s.StoreId equals u.StoreId
                             join us in _context.UserShiftStatus on u.UserId equals us.UserId
                             join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                             where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && us.UserId == user.UserId
                             select us/*new ClockingViewModel
                             {
                                 UserId = u.UserId,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 ShiftStatusId = us.ShiftStatusId,
                                 ShiftStatus = ss.ShiftStatusName,
                                 Time = us.Time,
                             }*/).ToList();
            ViewBag.AllStatus = allStatus;

            //List of the different work statuses
            ViewData["StatusOptions"] = new SelectList(_context.ShiftStatus, "ShiftStatusId", "ShiftStatusName");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteStatus(int userId, int statusId, DateTime time)
        {

            /*var userShiftStatus = await _context.UserShiftStatus.FindAsync(userId, statusId, time);*/

            //List of UserShiftStatus that match with the chosen user ID and work status ID
            var userShiftStatus = (from s in _context.Store
                                   join u in _context.User on s.StoreId equals u.StoreId
                                   join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                   join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                   where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && us.UserId == userId
                                        && us.ShiftStatusId == statusId /*&& us.Time == time*/
                                   select us).ToList();

            //Find the UserShiftStatus with the chosen time and deletes it from the db
            foreach (var s in userShiftStatus)
            {
                if (s.Time.ToString() == time.ToString())
                {
                    _context.UserShiftStatus.Remove(s);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(ViewStatus), new { id = userId });
        }

        //Allows an admin to edit an existing UserShiftStatus
        [HttpPost]
        //public async Task<IActionResult> EditStatus(int user, int shift, DateTime t, [Bind("UserId,ShiftStatusId,Time")] UserShiftStatus c)
        public async Task<IActionResult> EditStatus([Bind("UserId,ShiftStatusId,Time")] UserShiftStatus uss, int originalShift, DateTime originalTime)
        {
            // Requerying to redisplay view for errors
            var allStatus = (from s in _context.Store
                             join u in _context.User on s.StoreId equals u.StoreId
                             join us in _context.UserShiftStatus on u.UserId equals us.UserId
                             join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                             where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && us.UserId == uss.UserId
                             select us/*new ClockingViewModel
                             {
                                 UserId = u.UserId,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 ShiftStatusId = us.ShiftStatusId,
                                 ShiftStatus = ss.ShiftStatusName,
                                 Time = us.Time,
                             }*/).ToList();
            ViewBag.AllStatus = allStatus;


            ViewData["StatusOptions"] = new SelectList(_context.ShiftStatus, "ShiftStatusId", "ShiftStatusName");

            if (ModelState.IsValid)
            {
                try
                {
                    // Attach orginal row to the context
                    // so for some reason DateTimes is pretty fucking retarded. clearly the purpose of this row is to query the original user shift status whose EXACT values are LITERALLY
                    // sent back. now here's the fucking stupid thing: it WORKS when the time is in ANY format where the SECONDS is 0, so 10/21/2022 11:48:00 PM works. but if the seconds are 
                    // not 0? then it just queries null signifying that it "dOEsNT EXIsT HUe hhUE" when it CLEARLY SHOWS THAT IT FUCKING EXIS- yeah no wonder dealing with data in the form of dates is so fucking stupid and cancer
                    var originalRow = await (from u in _context.UserShiftStatus
                                             where u.Time.ToString() == originalTime.ToString()
                                             select u).FirstOrDefaultAsync();
                    //var originalRow = _context.UserShiftStatus.FirstOrDefault(x => x.UserId == uss.UserId && x.ShiftStatusId == originalShift && x.Time == originalTime); // && x.Time.CompareTo(originalTime) == 0


                    if (originalRow == null)
                    {
                        /*Added this part to try to fix the seconds thing******************************************************************************/
                        //Selects all the user's usershiftstatuses that have original old shift id
                        var userShiftStatus = (from s in _context.Store
                                               join u in _context.User on s.StoreId equals u.StoreId
                                               join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                               join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                               //i tried adding us.Time.toString() == originalTime.toString() here in the where clause
                                               //but it didn't work so I made another loop down there
                                               where us.UserId == uss.UserId && us.ShiftStatusId == originalShift
                                               select us).ToList();

                        var originalRow2 = originalRow;

                        //Loops through the chosen usershiftstatuses with the original shift id 
                        foreach (var s in userShiftStatus)
                        {
                            /*Console.WriteLine(s);*/
                            //Tries to find the original shift time in the db
                            if (s.Time.ToString() == originalTime.ToString())
                            {
                                originalRow2 = s;
                            }
                        }

                        if (originalRow2 == null)
                        {
                            TempData["editUserShiftStatusError"] = "There was an issue editing this user's shift status.";
                            //return RedirectToAction(nameof(ViewStatus), new { id = uss.UserId });
                            return View("ViewStatus");
                        }
                        originalRow = originalRow2;
                        /*******************************************************************************************************************************/

                        //TempData["editUserShiftStatusError"] = "There was an issue editing this user's shift status.";
                        //return RedirectToAction(nameof(ViewStatus), new { id = uss.UserId });
                        //return View("ViewStatus");
                    }

                    Console.WriteLine(originalRow.Time);
                    Console.WriteLine(originalTime);
                    Console.WriteLine(originalRow.Time == originalTime);

                    // similarly to the logic in editing tasks, we "update" the shift status by deleting the row then adding the new one with the updated values :p
                    _context.UserShiftStatus.Remove(originalRow);
                    await _context.SaveChangesAsync();

                    // now we "update" this shift status by adding the new one with the new values
                    _context.UserShiftStatus.Add(uss);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(ViewStatus), new { id = uss.UserId });
            }
            TempData["editUserShiftStatusError"] = "There was an issue editing this user's shift status.";
            //return RedirectToAction(nameof(ViewStatus), new { id = uss.UserId });
            return View("ViewStatus");



            /*UserShiftStatus uss = new UserShiftStatus
            {
                UserId = c.UserId,
                ShiftStatusId = newStatusOptionId,
                Time = newTime
            };
            var userShiftStatus = (from s in _context.Store
                                   join u in _context.User on s.StoreId equals u.StoreId
                                   join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                   join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                   where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && us.UserId == c.UserId
                                        && us.ShiftStatusId == c.ShiftStatusId *//*&& us.Time == time*//*
                                   select us).ToList();

            foreach (var s in userShiftStatus)
            {
                var a = s;
                Console.WriteLine(a);
                if (s.Time.ToString() == c.Time.ToString())
                {
                    s.ShiftStatusId = newStatusOptionId;
                    s.Time = newTime;
                    _context.Update(s);
                    await _context.SaveChangesAsync();
                }
            }*/
            /*var a = userShiftStatus;
            _context.Update(userShiftStatus);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewStatus), new { id = c.UserId });*/
        }
        /*SELINA^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
    }
}
