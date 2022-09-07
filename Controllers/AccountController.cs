#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;

namespace BaristaHome.Controllers
{
    public class AccountController : Controller
    {
        private readonly RegisterContext _context;

        public AccountController(RegisterContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword, InviteCode")] RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = (from u in _context.Register
                                     where u.Email.Equals(register.Email)
                                     select u).FirstOrDefault();

                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "Account already exists under this email! Please use a different one.");
                    return View(register);
                }

                // hashing password (salt is also applied)
                register.Password = Crypto.HashPassword(register.Password);
                _context.Add(register);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            return View(register);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                // checking existing email
                var validUser = (from u in _context.Register
                                 where u.Email.Equals(user.Email)
                                 select u).FirstOrDefault();

                // validating password with email's hashed password with input password
                if (validUser != null && Crypto.VerifyHashedPassword(validUser.Password, user.Password))
                {
                    return RedirectToAction("Index", "Account");
                }
                ModelState.AddModelError(string.Empty, "Email or Password is Incorrect");
            }
            return View(user);
        }

        /*
         * literally a shit ton of code from creating a new scaffolding
         * this just helps you setup a lot of the crud operations for your model
         * not sure if we need this and their views or not
         */

        // GET: Account
        public async Task<IActionResult> Index()
        {
            return View(await _context.Register.ToListAsync());
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registerViewModel = await _context.Register
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registerViewModel == null)
            {
                return NotFound();
            }

            return View(registerViewModel);
        }

        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registerViewModel = await _context.Register.FindAsync(id);
            if (registerViewModel == null)
            {
                return NotFound();
            }
            return View(registerViewModel);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword, InviteCode")] RegisterViewModel registerViewModel)
        {
            if (id != registerViewModel.Id)
            {
                return NotFound();
            }

            var existingEmail = (from u in _context.Register
                                 where u.Email.Equals(registerViewModel.Email)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Account already exists under this email! Please use a different one.");
                return View(registerViewModel);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    registerViewModel.Password = Crypto.HashPassword(registerViewModel.Password);
                    _context.Update(registerViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisterViewModelExists(registerViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(registerViewModel);
        }

        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registerViewModel = await _context.Register
                .FirstOrDefaultAsync(m => m.Id == id);
            if (registerViewModel == null)
            {
                return NotFound();
            }

            return View(registerViewModel);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registerViewModel = await _context.Register.FindAsync(id);
            _context.Register.Remove(registerViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterViewModelExists(int id)
        {
            return _context.Register.Any(e => e.Id == id);
        }
    }
}
