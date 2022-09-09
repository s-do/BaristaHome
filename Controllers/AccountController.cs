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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using WebMatrix.WebData;

namespace BaristaHome.Controllers
{
    public class AccountController : Controller
    {
        private readonly BaristaHomeContext _context;

        public AccountController(BaristaHomeContext context)
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
        public async Task<IActionResult> Register([Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode")] User register)
        {
            register.Color = "#FFFFFF";
            if (ModelState.IsValid)
            {
                Console.WriteLine("gaygayygaygaygayag");
                var existingEmail = (from u in _context.User
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
            ModelState.AddModelError(string.Empty, "There was an issue creating an account.");
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
        public async Task<IActionResult> Login(LoginViewModel user, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // checking existing email
                var validUser = (from u in _context.User
                                 where u.Email.Equals(user.Email)
                                 select u).FirstOrDefault();

                // validating password with email's hashed password with input password
                if (validUser != null && Crypto.VerifyHashedPassword(validUser.Password, user.Password))
                {
                    //A claim is a statement about a subject by an issuer and    
                    //represent attributes of the subject that are useful in the context of authentication and authorization operations.    
                    var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(validUser.UserId)),
                        new Claim(ClaimTypes.Email, validUser.Email),
                        };

                    //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                    var principal = new ClaimsPrincipal(identity);

                    // AUTHENTICATION PROPERTIES
                    // AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                            
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.


                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                    new AuthenticationProperties()
                    {
                        IsPersistent = user.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
                    });
                    return Redirect(ReturnUrl == null ? "/Account/Index" : ReturnUrl);
                    //return RedirectToAction("Index", "Account");
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
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registerViewModel = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
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

            var registerViewModel = await _context.User.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,InviteCode")] User registerViewModel)
        {
            if (id != registerViewModel.UserId)
            {
                return NotFound();
            }

            var existingEmail = (from u in _context.User
                                 where u.Email.Equals(registerViewModel.Email) && !u.UserId.Equals(registerViewModel.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
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
                    if (!RegisterViewModelExists(registerViewModel.UserId))
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

            var registerViewModel = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
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
            var registerViewModel = await _context.User.FindAsync(id);
            _context.User.Remove(registerViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegisterViewModelExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
