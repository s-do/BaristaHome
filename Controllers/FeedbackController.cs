using BaristaHome.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ILogger<FeedbackController> _logger;
        public FeedbackController(ILogger<FeedbackController> logger)
        {
            _logger = logger;
        }
        public IActionResult Feedback()
        {
            return View();
        }


        public IActionResult Worker()
        {
            return View();
        }



















        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
