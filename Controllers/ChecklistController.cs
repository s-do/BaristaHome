using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Helpers;

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
        public async Task<IActionResult> Checklist()
        {
            //List of a store's checklists
            var checklist = await (from c in _context.Checklist
                                   where c.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                                   select c).ToListAsync();
            ChecklistViewModel checklistInfo = GetChecklistInfo(checklist);
            return View(checklistInfo);
        }

        //Creates a new checklist
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checklist([Bind("ChecklistId,ChecklistTitle")] Checklist checklist)
        {
            // Capitalize the first letter of every word
            checklist.ChecklistTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(checklist.ChecklistTitle);
            var checklistViewModel = await (from c in _context.Checklist
                                            where c.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                                            select c).ToListAsync();

            checklist.StoreId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            if (ModelState.IsValid)
            {
                //Checks if entered in checklist name already exists or not
                var existingChecklist = (from c in _context.Checklist
                                         where c.ChecklistTitle.Equals(checklist.ChecklistTitle) && c.StoreId.Equals(Convert.ToInt32(User.FindFirst("StoreId").Value))
                                         select c).FirstOrDefault();

                if (existingChecklist != null)
                {
                    TempData["addChecklistError"] = "Checklist name already exists! Please use a different one.";
                    // call helper method to regenerate all of the checklist info for invalid inputs
                    return View("Checklist", GetChecklistInfo(checklistViewModel));
                }

                _context.Add(checklist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Checklist", "Checklist");
            }
            TempData["addChecklistError"] = "There was an issue creating a checklist.";
            return View("Checklist", GetChecklistInfo(checklistViewModel));
        }

        // Helper function to get a checklist's respective number of categories and tasks
        public ChecklistViewModel GetChecklistInfo(List<Checklist> checklist)
        {
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

            ChecklistViewModel checklistViewModel = new ChecklistViewModel { ChecklistInfo = checklistInfo };

            // a dictionary of key-value pairs of the checklists and their numbers of tasks and categories
            return checklistViewModel;
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

            // Call helper function to get a ViewModel of a Checklist an a dictionary of their categories and respective tasks
            CategoryViewModel checklistViewModel = GetCategoryTasks(checklist);
            return View(checklistViewModel);
        }

        // Helper function to get a checklist's respective categories and tasks
        public CategoryViewModel GetCategoryTasks(Checklist checklist)
        {
            //List of a checklist's categories
            var checklistCategory = (from s in _context.Store
                                     join c in _context.Checklist on s.StoreId equals c.StoreId
                                     join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                     where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && c.ChecklistId == checklist.ChecklistId
                                     select cat).ToList();

            //checklistInfo = { {categoryName, {list of tasks in category}}, ...}
            Dictionary<Category, List<TaskViewModel>> checklistInfo = new Dictionary<Category, List<TaskViewModel>>();

            //Finds tasks in a category and adds it to a list where the category is the key
            foreach (var cc in checklistCategory)
            {
                var checklistTasks = (from s in _context.Store
                                      join c in _context.Checklist on s.StoreId equals c.StoreId
                                      join cat in _context.Category on c.ChecklistId equals cat.ChecklistId
                                      join ct in _context.CategoryTask on cat.CategoryId equals ct.CategoryId
                                      join st in _context.StoreTask on ct.StoreTaskId equals st.StoreTaskId
                                      where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && ct.CategoryId == cc.CategoryId
                                      orderby st.TaskName
                                      select new TaskViewModel
                                      {
                                          StoreTaskId = st.StoreTaskId,
                                          TaskName = st.TaskName,
                                          IsFinished = ct.IsFinished
                                      }).ToList();

                checklistInfo[cc] = checklistTasks;
            }

            CategoryViewModel checklistViewModel = new CategoryViewModel
            {
                ChecklistId = checklist.ChecklistId,
                ChecklistTitle = checklist.ChecklistTitle,
                CategoryTasks = checklistInfo
            };

            // a dictionary of key-value pairs of the categories and their list of tasks
            return checklistViewModel;
        }

        //Deletes a checklist from the db
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChecklist(int id)
        {
            var checklist = await _context.Checklist.FindAsync(id);
            if (checklist == null)
            {
                return NotFound();
            }
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
            // Generate all of the categories and tasks of the selected checklist
            CategoryViewModel checklistViewModel = GetCategoryTasks(checklist);
            return View(checklistViewModel);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditChecklist([Bind("ChecklistId,ChecklistTitle,StoreId")] Checklist checklist)
        {
            // Capitalize the first letter of every word
            checklist.ChecklistTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(checklist.ChecklistTitle);

            if (ModelState.IsValid)
            {
                var existingChecklist = await _context.Checklist.FirstOrDefaultAsync(c => c.ChecklistTitle == checklist.ChecklistTitle && c.StoreId == checklist.StoreId);
                if (existingChecklist != null)
                {
                    TempData["editChecklistError"] = "Checklist already exists! Please use a different one.";
                    var checklistViewModel1 = await _context.Checklist.FirstOrDefaultAsync(m => m.ChecklistId == checklist.ChecklistId);
                    return View(GetCategoryTasks(checklistViewModel1));
                }

                try
                {
                    _context.Update(checklist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChecklistExists(checklist.ChecklistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditChecklist", new { id = checklist.ChecklistId });
            }
            TempData["editChecklistError"] = "There was an issue editing this checklist.";
            var checklistViewModel = await _context.Checklist.FirstOrDefaultAsync(m => m.ChecklistId == checklist.ChecklistId);
            return View(GetCategoryTasks(checklistViewModel));
        }
        
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory([Bind("CategoryName,ChecklistId")] Category category)
        {
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == category.ChecklistId
                                   select c).FirstOrDefaultAsync();
            if (checklist == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // checking for existing cateogry only for those in the same store AND same checklist (dupes are therefore allowed outside these parameters)
                var existingCategory = await (from cat in _context.Category
                                              join c in _context.Checklist on cat.ChecklistId equals c.ChecklistId
                                              where c.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && cat.CategoryName == category.CategoryName && cat.ChecklistId == category.ChecklistId
                                              select cat).FirstOrDefaultAsync();
                if (existingCategory != null)
                {
                    TempData["addCategoryError"] = "Category already exists! Please use a different one.";
                    return View("EditChecklist", GetCategoryTasks(checklist));
                }

                // reopen the edit page with the new category for that checklist
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditChecklist", new { id = category.ChecklistId });
            }
            TempData["addCategoryError"] = "There was an issue creating a category.";
            return View("EditChecklist", GetCategoryTasks(checklist));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory([Bind("ChecklistId, CategoryId, CategoryName")] Category category)
        {
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == category.ChecklistId
                                   select c).FirstOrDefaultAsync();
            if (checklist == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // see if category name exists in this checklist
                var existingCategory = await (from cat in _context.Category
                                              // this Equals() method does a case-sensitive comparison between the strings (cat.CategoryName == category.CategoryName is case-insensitive)
                                              where cat.ChecklistId == category.ChecklistId && EF.Functions.Collate(cat.CategoryName, "SQL_Latin1_General_CP1_CS_AS") == category.CategoryName
                                              select cat).FirstOrDefaultAsync();
                if (existingCategory != null)
                {
                    TempData["editCategoryError"] = "Category already exists in this checklist!";
                    return View("EditChecklist", GetCategoryTasks(checklist));
                }

                // edit name and save changes
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditChecklist", new { id = category.ChecklistId });
            }
            TempData["editCategoryError"] = "There was an issue editing a category.";
            return View("EditChecklist", GetCategoryTasks(checklist));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int checklistId, int categoryId)
        {
            // query checklist to redisplay view
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == checklistId
                                   select c).FirstOrDefaultAsync();
            var category = await _context.Category.FindAsync(categoryId);
            if (checklist == null || category == null)
            {
                return NotFound();
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("EditChecklist", new { id = checklist.ChecklistId });
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTask(int checkListId, int categoryId, [Bind("TaskName")] StoreTask task)
        {
            // force lower case to prevent case sensitivity upon displaying
            task.TaskName = task.TaskName.ToLower();
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == checkListId
                                   select c).FirstOrDefaultAsync();
            if (checklist == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // checks if new task already exists in category
                var existingTask = await (from t in _context.StoreTask
                                          where t.TaskName == task.TaskName
                                          select t).FirstOrDefaultAsync();
                if (existingTask != null)
                {
                    if (CategoryTaskExists(categoryId, existingTask.StoreTaskId))
                    {
                        TempData["addTaskError"] = "Task already exists in this category! Please use a different one.";
                        return View("EditChecklist", GetCategoryTasks(checklist));
                    }
                }

                // Creating category task based on the task name existing or not
                CategoryTask newCategoryTask = new CategoryTask { CategoryId = categoryId };
                // adding new task to db if unique and does not exist anywhere
                if (existingTask == null)
                {
                    StoreTask newTask = new StoreTask { TaskName = task.TaskName };
                    _context.Add(newTask);
                    await _context.SaveChangesAsync();

                    // updating the categoryTask's id with the new task's id
                    newCategoryTask.StoreTaskId = newTask.StoreTaskId;
                }
                else
                {
                    // updating the categoryTask's id with the existing task's id
                    newCategoryTask.StoreTaskId = existingTask.StoreTaskId;
                }
                newCategoryTask.IsFinished = false;

                _context.Add(newCategoryTask);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditChecklist", new { id = checklist.ChecklistId });
            }
            TempData["addTaskError"] = "There was an issue creating a Task.";
            return View("EditChecklist", GetCategoryTasks(checklist));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int checklistId, int categoryId, int taskId, [Bind("TaskName")] StoreTask task)
        {
            // force lower case to prevent case sensitivity upon displaying
            task.TaskName = task.TaskName.ToLower();
            // getting the categorytask to update
            var categoryTask = await (from ct in _context.CategoryTask
                                              join t in _context.StoreTask on ct.StoreTaskId equals t.StoreTaskId
                                              where ct.CategoryId == categoryId && t.StoreTaskId == taskId
                                              select ct).FirstOrDefaultAsync();
            // getting the checklist to return view
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == checklistId
                                   select c).FirstOrDefaultAsync();

            if (checklist == null || categoryTask == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // checking if the edited name already exists in the category
                var existingTask = await (from t in _context.StoreTask
                                            where t.TaskName == task.TaskName
                                            select t).FirstOrDefaultAsync();
                if (existingTask != null)
                {
                    if (CategoryTaskExists(categoryId, existingTask.StoreTaskId))
                    {
                        TempData["editTaskError"] = "Task already exists in this category! Please use a different one.";
                        return View("EditChecklist", GetCategoryTasks(checklist));
                    }
                }

                CategoryTask newCategoryTask = new CategoryTask { CategoryId = categoryId };
                // adding new task to db if unique and does not exist anywhere
                if (existingTask == null)
                {
                    StoreTask newTask = new StoreTask { TaskName = task.TaskName };
                    _context.Add(newTask);
                    await _context.SaveChangesAsync();

                    // updating the categoryTask's id with the new task's id
                    newCategoryTask.StoreTaskId = newTask.StoreTaskId;
                }
                else
                {
                    // updating the categoryTask's id with the existing task's id
                    newCategoryTask.StoreTaskId = existingTask.StoreTaskId;
                }
                newCategoryTask.IsFinished = false;

                // for some reason EF advises to not update and change the ID of an existing object, but instead create a new one
                _context.CategoryTask.Remove(categoryTask);
                await _context.SaveChangesAsync();

                // now we can "update" the categoryTask by adding a new one with the updated task id
                _context.Add(newCategoryTask);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditChecklist", new { id = checklist.ChecklistId });
            }
            TempData["editTaskError"] = "There was an issue editing this task.";
            return View("EditChecklist", GetCategoryTasks(checklist));
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int checklistId, int categoryId, int taskId)
        {
            var checklist = await (from c in _context.Checklist
                                   where c.ChecklistId == checklistId
                                   select c).FirstOrDefaultAsync();
            var categoryTask = await _context.CategoryTask.FindAsync(categoryId, taskId);
            if (checklist == null || categoryTask == null)
            {
                return NotFound();
            }
            _context.CategoryTask.Remove(categoryTask);
            await _context.SaveChangesAsync();
            return RedirectToAction("EditChecklist", new { id = checklist.ChecklistId });
        }

        [HttpPost]
        public async Task<JsonResult> MarkTask(int categoryId, int taskId)
        {
            var categoryTask = await (from ct in _context.CategoryTask
                                      where ct.CategoryId == categoryId && ct.StoreTaskId == taskId
                                      select ct).FirstOrDefaultAsync();

            var taskName = await (from st in _context.StoreTask
                                  where st.StoreTaskId == taskId
                                  select st).FirstOrDefaultAsync();

            TaskViewModel updatedTask = new TaskViewModel { TaskName = "", CategoryId = categoryId, StoreTaskId = taskId };

            if (categoryTask != null && taskName != null)
            {

                // flip the marked state of the task
                if (categoryTask.IsFinished)
                {
                    categoryTask.IsFinished = false;
                    updatedTask.TaskName += "❌ " + taskName.TaskName;
                }
                else
                {
                    categoryTask.IsFinished = true;
                    updatedTask.TaskName += "✔️ " + taskName.TaskName;
                }

                _context.Update(categoryTask);
                await _context.SaveChangesAsync();

            }
            return Json(updatedTask);
        }

        // Helper functions for validation checks
        private bool CategoryTaskExists(int categoryId, int taskId)
        {
            return _context.CategoryTask.Any(ct => ct.CategoryId == categoryId && ct.StoreTaskId == taskId);
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(cat => cat.CategoryId == id);
        }

        private bool ChecklistExists(int id)
        {
            return _context.Checklist.Any(c => c.ChecklistId == id);
        }
        /* PETER ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ */
    }
}