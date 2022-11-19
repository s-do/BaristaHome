using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.WebPages.Html;

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


            ListDictionary descriptions = new ListDictionary();
            foreach (var review in reviews)
            {
                descriptions.Add(review.Title, review.Description);
            }

            ViewData["reviews"] = descriptions;

            return View(reviews);
        }

        [HttpGet]
        public IActionResult Worker()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Worker([Bind("FeedbackId,Title,Description,StoreId,UserId")] Feedback feedback)
        {  
            feedback.StoreId = Convert.ToInt16(User.FindFirst("StoreId").Value);
            feedback.UserId = Convert.ToInt16(User.FindFirst("UserId").Value);

            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction("Worker", "Feedback");
            }
                
         
         ModelState.AddModelError(string.Empty, "There was an issue creating an account.");
         return View(feedback);
        }














        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
