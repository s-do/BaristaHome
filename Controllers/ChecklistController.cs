using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checklist([Bind("ChecklistTitle")] Checklist checklist)
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
    }
}
