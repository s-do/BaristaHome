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

        [HttpPost]
        public JsonResult SearchDescription(string topic)
        {

            var reviews = (from f in _context.Feedback
                           join store in _context.Store on f.StoreId equals store.StoreId
                           where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value)) 
                           select f).ToList();


            return Json(reviews);

        }


        /*        // POST: Resolve Feedback
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Delete(string descript)
                {
                    // requery item then delete
                    *//*var feedbackItem = await _context.Feedback.FindAsync(id, Convert.ToInt32(User.FindFirst("StoreId").Value));*//*

                    var feedbackItem = (from f in _context.Feedback
                                        join store in _context.Store on f.StoreId equals store.StoreId
                                        where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                        select f).Where(i => i.Description.Contains(descript));



                    if (feedbackItem == null)
                    {
                        return new EmptyResult();
                    }

                    _context.Feedback.Remove((Feedback)feedbackItem);
                    _context.SaveChanges();
                    //await _context.SaveChangesAsync();
                    return Json(feedbackItem);
                }*/



        // POST: Resolve Feedback
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // requery item then delete
            var feedbackItem = await _context.Feedback.FindAsync(id);                   

            if (feedbackItem == null)
            {
                return View("Error");
            }

            _context.Feedback.Remove(feedbackItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Owner));
        }






        // Get: Test Delete Page
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

         
            var feedbackItem = await _context.Feedback.FirstOrDefaultAsync(m => m.FeedbackId == id);




            return View(feedbackItem);
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
