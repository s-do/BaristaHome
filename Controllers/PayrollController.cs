using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            /*            var workers = await _context.Payroll
                                        .Where(u => u.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value))
                                        .OrderByDescending(u => u.RoleId)
                                        .ThenBy(u => u.FirstName)
                                        .ToListAsync();*/

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

            return View();
        }




    }
}
