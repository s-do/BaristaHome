using BaristaHome.Data;
using BaristaHome.Migrations;
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

        //Display list of workers that belong to the store
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var workers = await _context.User
                            .Where(u => u.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value))
                            .OrderByDescending(u => u.RoleId)
                            .ThenBy(u => u.FirstName)
                            .ToListAsync();

            return View(workers);
        }

        //Return a worker edit page for the current user
        public async Task<IActionResult> WorkerEdit()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

        //Return a details page for the current user
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


        //Retrieve all input from the worker edit page, and then update the user model and save the user model in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            WorkerEdit([Bind(include: ("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage,UserDescription,Image"))] User worker)
        {

            var existingEmail = (from u in _context.User
                                 where u.Email.Equals(worker.Email) && !u.UserId.Equals(worker.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
                return View(worker);
            }

            //Saves worker image as a byte array
            if (worker.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    worker.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    worker.UserImageData = fileBytes;
                }
            }

            var w = await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value);
            w.FirstName = worker.FirstName;
            w.LastName = worker.LastName;
            w.Email = worker.Email;
            w.Image = worker.Image;
            w.UserImage = worker.UserImage;
            w.UserImageData = worker.UserImageData;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(w);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(w);
        }

        //Return owner editing page based on their id
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


        //Updates and saves any new changes inputted by the user
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

        //Returns an owner self editing page based on their id
        public async Task<IActionResult> OwnerSelfEdit()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }



        //Saves any new changes to the user information inputted by the user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OwnerSelfEdit([Bind(include: ("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage,UserDescription,Image"))] User worker)
        {
            worker.Password = Crypto.HashPassword(worker.Password);
            worker.ConfirmPassword = worker.Password;

            var existingEmail = (from u in _context.User
                                 where u.Email.Equals(worker.Email) && !u.UserId.Equals(worker.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
                return View(worker);
            }

            //Saves worker image as a byte array
            if (worker.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    worker.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    worker.UserImageData = fileBytes;
                }
            }

            var w = await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value);
            w.FirstName = worker.FirstName;
            w.LastName = worker.LastName;
            w.Email = worker.Email;
            w.Color = worker.Color;
            w.Image = worker.Image;
            w.UserImage = worker.UserImage;
            w.UserImageData = worker.UserImageData;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(w);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(w);
        }


        // Returns a delete page based on the selected user's id
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


        // Removes the selected user from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.User.FindAsync(id);
            _context.User.Remove(worker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
            return _context.User.Any(worker => worker.UserId == id);
        }

        //Displays an invite code page based on the user id
        public async Task<IActionResult> Invite()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

        //Displays user profile based on the user id
        public async Task<IActionResult> Profile()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

        //Returns the current users image to the view
        public async Task<ActionResult> RenderImage(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.User.FirstOrDefaultAsync(m => m.UserId == id);
            if (worker == null)
            {
                return NotFound();
            }
            var image = (from w in _context.User
                         where w.UserId == worker.UserId
                         select worker.UserImageData).First();


            return File(image, "image/png");
        }

        //Returns a change password page based on the user id
        public async Task<IActionResult> ChangePassword()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

        //Updates and saves the new password to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage,Wage,UserDescription")] User worker)
        {
            worker.Password = Crypto.HashPassword(worker.Password);
            worker.ConfirmPassword = worker.Password;

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
                if (worker.RoleId == 1 || worker.RoleId == 2)
                {
                    return RedirectToAction("WorkerEdit");
                }
                else
                {
                    return RedirectToAction("OwnerSelfEdit");
                }
                //return RedirectToAction(nameof(Index));
            }



            return View(worker);
        }
    }
}
