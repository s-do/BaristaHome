using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    public class FeedbackController : Controller
    {
        /*        private readonly ILogger<FeedbackController> _logger;
                public FeedbackController(ILogger<FeedbackController> logger)
                {
                    _logger = logger;
                }*/


        private readonly BaristaHomeContext _context;

        public FeedbackController(BaristaHomeContext context)
        {
            _context = context;
        }

        public IActionResult Worker()
        {
            return View();
        }


        public async Task <IActionResult> Owner()
        {
            var reviews = await _context.Feedback
                .Where(u => u.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value))
                .OrderByDescending(u => u.FeedbackId)
                .ToListAsync();

            /*            var reviews = (from feedback in _context.Feedback
                                       join store in _context.Store on feedback.StoreId equals store.StoreId
                                       join user in _context.User on store.StoreId equals user.StoreId
                                       where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                       orderby feedback.FeedbackId descending
                                       select feedback).ToList();*/

            return View(reviews);
        }

















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
