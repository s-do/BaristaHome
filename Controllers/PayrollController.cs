using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly BaristaHomeContext _context;
        public PayrollController(BaristaHomeContext context)
        {
            _context = context;
        }
        public IActionResult Owner()
        {
            return View();
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
    }
}
