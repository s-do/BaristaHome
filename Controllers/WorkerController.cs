using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;

namespace BaristaHome.Controllers
{
    /*[Authorize]*/
    public class WorkerController : Controller
    {
        /*[Authorize(Policy = "AdminOnly")]*/
        private readonly BaristaHomeContext _context;

        public WorkerController(BaristaHomeContext context)
        {
            _context = context;
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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            /*//Get the list of all users
            List<User>? listOfUsers = await _context.User.ToListAsync();

            //Create a new list to store users that belong to the current store
            List<User> listOfStoreUsers = new List<User>();

            //Get the current user (which should be the owner/admin)
            var currentUser = await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value);

            //Get their invite code
            string storeInviteCode = currentUser.InviteCode;

            //Go through the list of all users
            foreach (var u in listOfUsers)
            {
                //And if their invite code is the same, add them to the new list
                if (storeInviteCode != null && u.InviteCode != null)
                {
                    if (u.InviteCode.Equals(storeInviteCode))
                    {
                        listOfStoreUsers.Add(u);
                    }
                }

            }
            //Pass the list of store users to the view
            return View(listOfStoreUsers);*/

            // you could simply use a LINQ query to display all workers inside a specific store using the storeid claim instead of using the invite code
            var workerList = (from u in _context.User
                              where u.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value) // Selecting very user with that storeid
                              orderby u.RoleId descending, u.FirstName // Order by the roles in alphabetical order
                              select u) as IEnumerable<User>;

            // A cool way to do a LINQ query asynchronously instead! I still don't understand when to use 
            // asynchronous and synchronous queries, best thing people said was it helps with a more responsive UI
            var workers = await _context.User
                            .Where(u => u.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value))
                            .OrderByDescending(u => u.RoleId)
                            .ThenBy(u => u.FirstName)
                            .ToListAsync();

            return View(workers);
        }

        public async Task<IActionResult> WorkerEdit()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // GET: Account/Edit/UserId
        /*        public async Task<IActionResult> WorkerEdit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var registerViewModel = await _context.User.FindAsync(id);
                    if (registerViewModel == null)
                    {
                        return NotFound();
                    }
                    return View(registerViewModel);
                }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WorkerEdit([Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage,UserDescription")] User worker)
        {

            var existingEmail = (from u in _context.User
                                 where u.Email.Equals(worker.Email) && !u.UserId.Equals(worker.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
                return View(worker);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    worker.Password = Crypto.HashPassword(worker.Password);
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(worker);
        }

        public async Task<IActionResult> OwnerEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OwnerEdit(User worker)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        public async Task<IActionResult> OwnerSelfEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OwnerSelfEdit([Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage,Wage,UserDescription")] User worker)
        {

            /*ViewBag.Roles = new SelectList("1","2");*/
            var existingEmail = (from u in _context.User
                                 where u.Email.Equals(worker.Email) && !u.UserId.Equals(worker.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
                return View(worker);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return View(worker);
                //return RedirectToAction(nameof(Index));
            }
            return View(worker);
        }

        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }


        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.User.FindAsync(id);
            _context.User.Remove(worker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Invite()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }
    }
}
