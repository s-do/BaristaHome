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

            return View(reviews);
        }

        [HttpPost]
        public JsonResult SearchDescription(string topic)
        {
            // getting data for feedbacks to correctly display whichever one the user clicks on
            var feedbacks = (from f in _context.Feedback
                           join store in _context.Store on f.StoreId equals store.StoreId
                           where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value)) 
                           select f).ToList();

            return Json(feedbacks);
        }

        // POST: Delete Feedback 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // requery item then delete the feedback
            var feedbackItem = await _context.Feedback.FindAsync(id);

            if (feedbackItem == null)
            {
                return View("Error");
            }

            _context.Feedback.Remove(feedbackItem);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Owner));
            // this is the best way i could "refresh" the page to update the feedback list once it's deleted, as the above statement doens't work idk why
            return Json(new { redirectToUrl = Url.Action("Owner", "Feedback") });
        }

        // Views that the worker will see
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
            return View(feedback);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}