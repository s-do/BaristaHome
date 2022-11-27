using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
using Microsoft.AspNetCore.Mvc.Rendering;
>>>>>>> Admim_Payroll
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer;
using BaristaHome.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
<<<<<<< HEAD
        private readonly BaristaHomeContext _context;
        public PayrollController(BaristaHomeContext context)
        {
            _context = context;
        }
        public IActionResult Owner()
        {
            return View();
=======
        //private readonly ILogger<PayrollController> _logger;
        private readonly BaristaHomeContext _context;


        public PayrollController(BaristaHomeContext context)
        {
            _context = context;
            //_logger = (ILogger<PayrollController>?)logger;
>>>>>>> Admim_Payroll
        }

        /*        public IActionResult Worker()
                {
                    return View();
                }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


<<<<<<< HEAD
        [HttpGet]
        public async Task<IActionResult> Worker(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User.FirstOrDefaultAsync(m => m.UserId == id);

            if (worker == null)
            {
                return NotFound();
            }

            List<Payroll> payrollQuery = (from u in _context.User
                                          join payroll in _context.Payroll on u.UserId equals payroll.UserId
                                          where payroll.UserId == worker.UserId
                                          select payroll).ToList();
            ViewBag.PayrollList = payrollQuery;

            return View(worker);
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (worker == null)
            {
                return NotFound();
            }
            var image = (from u in _context.User
                         where u.UserId == worker.UserId
                         select worker.UserImageData).First();


            return File(image, "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> Worker(int month)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId").Value);
            var worker = await _context.User.FirstOrDefaultAsync(m => m.UserId == userId);
            //Get list of payroll
            var payrollList = (List<Payroll>)(from p in _context.Payroll
                                              where p.UserId == userId
                                              select p).ToList();
            ViewBag.PayrollList = payrollList;
            List<Payroll> resultPayroll = new List<Payroll>();
            if (month != null && month > 0)
            {
                resultPayroll = (from p in payrollList
                                where p.Date.Month == month
                                select p).ToList();
                ViewBag.PayrollList = resultPayroll;
            }
            else
            {
                resultPayroll = (from u in _context.User
                                              join payroll in _context.Payroll on u.UserId equals payroll.UserId
                                              where payroll.UserId == worker.UserId
                                              select payroll).ToList();
                ViewBag.PayrollList = resultPayroll;
            }

            return View(worker);


        }
=======
        public async Task<IActionResult> Owner()
        {
            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).OrderBy(o => o.FullName).ToList();

            ViewBag.Payroll = payrolls;

            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            /* Pulls list of workers under the same store id for select list  */
            List<PayrollOwnerViewModel> users  = (from user in _context.User
                                join store in _context.Store on user.StoreId equals store.StoreId
                                where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                select new PayrollOwnerViewModel
                                {
                                    UserId = user.UserId,
                                    FullName = user.FirstName + " " + user.LastName,
                                }).ToList();

            ViewData["FullNames"] = new SelectList(users, "UserId", "FullName");

            return View();
        }


        /* Adds a payroll into the database */
        [HttpPost]
        public async Task<IActionResult> Owner(PayrollOwnerViewModel payrollOwnerViewModel)
        {
            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).OrderBy(o => o.FullName).ToList();
            ViewBag.Payroll = payrolls;

            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            List<PayrollOwnerViewModel> users = (from user in _context.User
                                                 join store in _context.Store on user.StoreId equals store.StoreId
                                                 where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                 select new PayrollOwnerViewModel
                                                 {
                                                     UserId = user.UserId,
                                                     FullName = user.FirstName + " " + user.LastName,
                                                 }).ToList();


            /* Shows the full name but when selected, it will give the userId as Value so database knows what user is being added */
            ViewData["FullNames"] = new SelectList(users, "UserId", "FullName");


            if (ModelState.IsValid)
            {
                var payroll1 = new Payroll()
                {
                    Hours = payrollOwnerViewModel.Hours,
                    Amount = payrollOwnerViewModel.Amount,
                    Date = payrollOwnerViewModel.Date,
                    UserId = payrollOwnerViewModel.UserId
                };

                _context.Add(payroll1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Owner));


            }

            ViewData["FullNames"] = new SelectList(users, "FullName", "FullName");
            ModelState.AddModelError(string.Empty, "There was an issue adding this item into your Payroll");
            return View();

        }


        // Get: Payroll/Delete/PayrollId
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var PayrollEntry = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);

            if (PayrollEntry == null)
            {
                return NotFound();
            }

            var PayrollName = (from pr in _context.Payroll
                               join user in _context.User on pr.UserId equals user.UserId
                               where pr.PayrollId == id
                               select user.FirstName).FirstOrDefault();

            ViewBag.PayrollName = PayrollName;
            /*ViewData["PayrollName"] = PayrollName;*/

            return View(PayrollEntry);
        }


        // POST: Payroll/Delete/PayrollId
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // requery the item and  d e l e t e
            var PayrollEntry = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);
            _context.Payroll.Remove(PayrollEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Owner));
        }



        // For Edit
        private bool PayrollOwnerViewModelExists(int payId)
        {
            return _context.Payroll.Any(e => e.PayrollId == payId);
        }


        // GET Edit Payroll
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editViewModel = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);

            // Query to get Worker Name as Viewbag to display, since can't call Payroll.User.FirstName
            var PayrollName = (from pr in _context.Payroll
                               join user in _context.User on pr.UserId equals user.UserId
                               where pr.PayrollId == id
                               select user.FirstName).FirstOrDefault();

            ViewBag.PayrollName = PayrollName;


            if (editViewModel == null)
            {
                return NotFound();
            }
            return View(editViewModel);
        }


        // Edit Value in Payroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,Hours,Amount,Date,UserId")] Payroll editViewModel)
        {
            if (id != editViewModel.PayrollId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollOwnerViewModelExists(editViewModel.PayrollId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Owner));
            }
            return View(editViewModel);
        }



        // SEARCH BAR FEATURE
        // Shows the page again after searching the user wanted
        public async Task<IActionResult> SearchBarResults(int userId)
        {
            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    && payroll.UserId.Equals(userId)
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).ToList();
            ViewBag.Payroll = payrolls;
            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            /* Pulls list of workers under the same store id for select list  */
            List<PayrollOwnerViewModel> users = (from user in _context.User
                                                 join store in _context.Store on user.StoreId equals store.StoreId
                                                 where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                 select new PayrollOwnerViewModel
                                                 {
                                                     UserId = user.UserId,
                                                     FullName = user.FirstName + " " + user.LastName,
                                                 }).ToList();

            return View();
        }














>>>>>>> Admim_Payroll
    }
}
