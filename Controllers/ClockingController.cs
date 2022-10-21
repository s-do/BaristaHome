using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    public class ClockingController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ClockingController(BaristaHomeContext context)
        {
            _context = context;
        }
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

        
    }
}
