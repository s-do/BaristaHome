using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        //Displays all the checklists in a store
        [HttpGet]
        /*SELINAvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        public IActionResult Checklist()
        {
            //List of a store's checklists
            var checklist = (from c in _context.Checklist
                             where c.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                             select c).ToList();

            //checklistInfo = { {Checklist, {# of categorys, # of tasks}},..., }
            Dictionary<Checklist, List<int>> checklistInfo = new Dictionary<Checklist, List<int>>();

            //Calculates the total number of categories and tasks a checklist has
            //The calculated number gets added to a list where the Checklist object is the key
            foreach (var check in checklist)
            {
                var numOfCategory = 0;
                var numOfTask = 0;
                var checklistCategory = (from s in _context.Store
                                         join c in _context.Checklist on s.StoreId equals c.StoreId
                                         join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                         where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && c.ChecklistId == check.ChecklistId
                                         select cat).ToList();

                if (checklistCategory != null)
                {
                    numOfCategory += checklistCategory.Count();

                    foreach (var cc in checklistCategory)
                    {
                        var checklistTasks = (from s in _context.Store
                                              join c in _context.Checklist on s.StoreId equals c.StoreId
                                              join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                              join ct in _context.CategoryTask on cat.CategoryId equals ct.CategoryId
                                              join st in _context.StoreTask on ct.StoreTaskId equals st.StoreTaskId
                                              where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && ct.CategoryId == cc.CategoryId
                                              select st.TaskName).ToList();

                        if (checklistTasks != null)
                        {
                            numOfTask += checklistTasks.Count();
                        }
                    }
                }
                List<int> count = new List<int>();
                count.Add(numOfCategory);
                count.Add(numOfTask);
                checklistInfo[check] = count;
            }
            
            ViewBag.Checklist = checklistInfo;
            return View();
        }

        //Creates a new checklist
        [HttpPost]
        public async Task<IActionResult> Checklist([Bind("ChecklistId,ChecklistTitle")] Checklist checklist)
        {
            checklist.StoreId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            if (ModelState.IsValid)
            {
                //Checks if entered in checklist name already exists or not
                var existingChecklist = (from c in _context.Checklist
                                         where c.ChecklistTitle.Equals(checklist.ChecklistTitle) && c.StoreId.Equals(Convert.ToInt32(User.FindFirst("StoreId").Value))
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
        // GET:  Gets a checklist's categories and tasks
        public async Task<IActionResult> ViewChecklist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checklist = await _context.Checklist.FirstOrDefaultAsync(m => m.ChecklistId == id);
            if (checklist == null)
            {
                return NotFound();
            }

            //List of a checklist's categories
            var checklistCategory = (from s in _context.Store
                                 join c in _context.Checklist on s.StoreId equals c.StoreId
                                 join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                 where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && c.ChecklistId == checklist.ChecklistId
                                 select cat).ToList();

            //checklistInfo = { {categoryName, {list of tasks in category}}, ...}
            Dictionary<string, List<string>> checklistInfo = new Dictionary<string, List<string>>();

            //Finds tasks in a category and adds it to a list where the category is the key
            foreach (var cc in checklistCategory)
            {
                var checklistTasks = (from s in _context.Store
                                      join c in _context.Checklist on s.StoreId equals c.StoreId
                                      join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                      join ct in _context.CategoryTask on cat.CategoryId equals ct.CategoryId
                                      join st in _context.StoreTask on ct.StoreTaskId equals st.StoreTaskId
                                      where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && ct.CategoryId == cc.CategoryId
                                      select st.TaskName).ToList();

                checklistInfo[cc.CategoryName] = checklistTasks;
            }

            ViewBag.ChecklistInfo = checklistInfo;

            return View(checklist);
        }

        //Deletes a checklist from the db
        [HttpPost]
        public async Task<IActionResult> DeleteChecklist(int id)
        {
            var checklist = await _context.Checklist.FindAsync(id);
            _context.Checklist.Remove(checklist);
            await _context.SaveChangesAsync();
            return RedirectToAction("Checklist", "Checklist");
        }
        /*SELINA^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        /* PETER ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼ */
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditChecklist(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checklist = await _context.Checklist.FirstOrDefaultAsync(m => m.ChecklistId == id);
            if (checklist == null)
            {
                return NotFound();
            }
            string title = checklist.ChecklistTitle;
            ViewBag.Title = title;
            TempData["Title"] = checklist.ChecklistTitle;

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AddCategory([Bind("CategoryName,ChecklistId")] Category category)
        {
            if (ModelState.IsValid)
            {
                // checking for existing cateogry only for those in the same store AND same checklist (dupes are therefore allowed outside these parameters)
                var existingCategory = await (from cat in _context.Category
                                              join c in _context.Checklist on cat.ChecklistId equals c.ChecklistId
                                              where c.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && cat.CategoryName == category.CategoryName && cat.ChecklistId == category.ChecklistId
                                              select cat).FirstOrDefaultAsync();
                if (existingCategory != null)
                {
                    ModelState.AddModelError(string.Empty, "Category name already exists! Please use a different one.");
                    return View(category);
                }

                // reopen the edit page with the new category for that checklist
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditChecklist", new { id = category.ChecklistId });
            }
            ModelState.AddModelError(string.Empty, "There was an issue creating a category.");
            return View(category);
        }
        /* PETER ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ */


        [HttpGet]
        public IActionResult MarkChecklist()
        {
            return View();
        }
    }
}
