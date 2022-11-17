using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace BaristaHome.Controllers
{
    [Authorize]
    public class ChecklistController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ChecklistController(BaristaHomeContext context)
        {
            _context = context;
        }

        public IActionResult Checklist()
        {
            var checklist = (from c in _context.Checklist
                             select c).ToList();

            ViewBag.Checklist = checklist;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checklist([Bind("ChecklistId,ChecklistTitle")] Checklist checklist)
        {
            checklist.StoreId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            if (ModelState.IsValid)
            {
                //checks if entered in checklist name already exists or not
                var existingChecklist = (from c in _context.Checklist
                                         where c.ChecklistTitle.Equals(checklist.ChecklistTitle)
                                         select c).FirstOrDefault();

                if (existingChecklist != null)
                {
                    ModelState.AddModelError(string.Empty, "Checklist name already exists! Please use a different one.");
                    return View(checklist);
                }

                _context.Add(checklist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Checklist", "Checklist");
            }
            ModelState.AddModelError(string.Empty, "There was an issue creating a checklist.");
            return View(checklist);
        }

        [HttpGet]
        // GET: 
        public async Task<IActionResult> ViewChecklist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checklist = await _context.Checklist
                .FirstOrDefaultAsync(m => m.ChecklistId == id);
            if (checklist == null)
            {
                return NotFound();
            }

            /*var lastestStatus = (from s in _context.Store
                                 join u in _context.User on s.StoreId equals u.StoreId
                                 join us in _context.UserShiftStatus on u.UserId equals us.UserId
                                 join ss in _context.ShiftStatus on us.ShiftStatusId equals ss.ShiftStatusId
                                 where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)*/

            var checklistCategory = (from s in _context.Store
                                 join c in _context.Checklist on s.StoreId equals c.StoreId
                                 join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                 where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && c.ChecklistId == checklist.ChecklistId
                                 select cat).ToList();

            Dictionary<string, string> checklistInfo = new Dictionary<string, string>();

            foreach (var cc in checklistCategory)
            {
                var checklistTasks = (from s in _context.Store
                                      join c in _context.Checklist on s.StoreId equals c.StoreId
                                      join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                      join ct in _context.CategoryTask on cat.CategoryId equals ct.CategoryId
                                      join st in _context.StoreTask on ct.StoreTaskId equals st.StoreTaskId
                                      where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && ct.CategoryId == cc.CategoryId
                                      select st).ToList();
                foreach(var st in checklistTasks)
                {
                    checklistInfo.Add(cc.CategoryName, st.TaskName);
                }
            }

            ViewBag.ChecklistInfo = checklistInfo;

            return View(checklist);
        }

        [HttpGet]
        public IActionResult EditChecklist()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MarkChecklist()
        {
            return View();
        }
    }
}
