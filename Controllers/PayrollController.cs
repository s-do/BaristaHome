using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        //private readonly ILogger<PayrollController> _logger;
        private readonly BaristaHomeContext _context;


        public PayrollController(BaristaHomeContext context)
        {
            _context = context;
            //_logger = (ILogger<PayrollController>?)logger;
        }


/*        public PayrollController(ILogger<PayrollController> logger)
        {
            _logger = logger;
        }*/

/*        public IActionResult Owner()
        {
            return View();
        }
*/
        public IActionResult Worker()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Owner()
        {
            /*            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
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
                                                               }).ToList();*/

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
                                                    }).ToList();

            ViewBag.Payroll = payrolls;



            /* A list of non duplicate full names, takes the first payroll with that full name.  */
            List<PayrollOwnerViewModel> diff_name_payrolls = new List<PayrollOwnerViewModel>();
            diff_name_payrolls = payrolls
                                .GroupBy(element => element.FullName)
                                .Select(e => e.First())
                                .ToList();


            ViewData["FullNames"] = new SelectList(diff_name_payrolls, "UserId", "FullName");

            return View();
        }


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
                                                    }).ToList();
            ViewBag.Payroll = payrolls;

/*          To give a selectlist without any duplicate names*/
            List<PayrollOwnerViewModel> diff_name_payrolls = new List<PayrollOwnerViewModel>();
            diff_name_payrolls = payrolls
                                .GroupBy(element => element.FullName)
                                .Select(e => e.First())
                                .ToList();

/*            Shows the full name but when selected, it will give the userId as Value so database knows what user is being added*/
            ViewData["FullNames"] = new SelectList(diff_name_payrolls, "UserId", "FullName");



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

            ViewData["FullNames"] = new SelectList(diff_name_payrolls, "FullName", "FullName");
            ModelState.AddModelError(string.Empty, "There was an issue adding this item into your Payroll");
            return View();

        }



    


















    }
}
