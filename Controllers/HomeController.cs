using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Pipelines;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BaristaHomeContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(BaristaHomeContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Getting store name of logged in user and displaying to view
            var storeName = (from user in _context.User
                             join store in _context.Store on user.StoreId equals store.StoreId
                             where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                             select store.StoreName).FirstOrDefault();

            ViewBag.StoreName = storeName;

            var newestShiftStatus = (from u in _context.User
                                     join st in _context.Store on u.StoreId equals st.StoreId
                                     join uss in _context.UserShiftStatus on u.UserId equals uss.UserId
                                     join ss in _context.ShiftStatus on uss.ShiftStatusId equals ss.ShiftStatusId
                                     orderby uss.Time descending
                                     where uss.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                                     select uss.ShiftStatusId).FirstOrDefault();

            ViewBag.NewestShiftStatus = newestShiftStatus;

            var announcement = (from a in _context.Announcements
                                where a.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                select a).FirstOrDefault();
            if (announcement == null || string.IsNullOrWhiteSpace(announcement.AnnouncementText))
            {
                Announcement nullAnnouncement = new Announcement();
                nullAnnouncement.AnnouncementText = "No Announcement";
                ViewBag.StoreAnnouncement = nullAnnouncement.AnnouncementText;
            }
            else
            {
                ViewBag.StoreAnnouncement = announcement.AnnouncementText;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("AnnouncementText,StoreId")] Announcement newAnnouncement)
        {
            Console.WriteLine(newAnnouncement.AnnouncementText);
            var announcement = (from a in _context.Announcements
                                where a.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                select a).FirstOrDefault();
            //If store is missing announcement id db
            if (announcement == null)
            {
                var id = Convert.ToInt16(User.FindFirst("StoreId").Value);
                Announcement nullAnnouncement = new Announcement
                {
                    AnnouncementText = newAnnouncement.AnnouncementText,
                    StoreId = id

                };
                if (ModelState.IsValid)
                {
                    _context.Add(nullAnnouncement);
                    await _context.SaveChangesAsync();
                    return Index();
                }
            }
            else
            {
                announcement.AnnouncementText = newAnnouncement.AnnouncementText;
            }

            if (ModelState.IsValid)
            {
                _context.Update(announcement);
                await _context.SaveChangesAsync();
            }

            return Index();
        }

        public IActionResult Privacy()
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