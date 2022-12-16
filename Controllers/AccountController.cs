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
        

        [HttpGet]
        // GET: Account/Register
        // Displays the Register user view
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId")] User register)
        {
            // IF SOMEONE COULD TELL ME A BETTER WAY TO SET DEFAULTS THAN DOING THIS PLEASE LET ME KNOW!!! DOING IT IN THE MODELS FOLDER DOESN'T FUCKING WORK
            register.Color = "#000000";
            register.RoleId = 1;
            if (ModelState.IsValid)
            {
                //checks if entered in email already exists or not
                var existingEmail = (from u in _context.User
                                     where u.Email.Equals(register.Email)
                                     select u).FirstOrDefault();

                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "Account already exists under this email! Please use a different one.");
                    return View(register);
                }

                //checks if entered in invite code exists
                var existingInvite = (from u in _context.Store
                         where u.StoreInviteCode.Equals(register.InviteCode)
                         select u).FirstOrDefault();
                if (existingInvite == null)
                {
                    ModelState.AddModelError(string.Empty, "No store is associated with this invite code. Please use an existing one.");
                    return View(register);
                }
                else
                {
                    var existingStore = (from u in _context.Store
                                         where u.StoreInviteCode.Equals(register.InviteCode)
                                         select u.StoreId).FirstOrDefault();
                    register.StoreId = existingStore;
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
            /*
             * For hard-coding lost accounts lol
             * var acc = _context.User.Find(80);
            acc.Password = Crypto.HashPassword("amogus");
            acc.ConfirmPassword = acc.Password;
            _context.User.Update(acc);
            _context.SaveChanges();*/

            if (User.Identity.IsAuthenticated) 
                return RedirectToAction("Index", "Home");
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
                        new Claim("UserId", Convert.ToString(validUser.UserId)),
                        new Claim("FirstName", Convert.ToString(validUser.FirstName)),
                        new Claim("LastName", Convert.ToString(validUser.LastName)),
                        new Claim("Password", Convert.ToString(validUser.Password)),
                        new Claim("Email", validUser.Email),
                        new Claim("RoleId",  Convert.ToString(validUser.RoleId)),
                        new Claim("InviteCode", validUser.InviteCode),
                        new Claim("StoreId", Convert.ToString(validUser.StoreId)),
                        }; // have to represent ints as strings i guess

                    if (validUser.UserImageData != null) { claims.Add(new Claim("UserImageData", Convert.ToString(validUser.UserImageData))); };
                    if (validUser.UserImage != null) { claims.Add(new Claim("UserImage", Convert.ToString(validUser.UserImage))); };
                    if (validUser.Color != null) { claims.Add(new Claim("Color", Convert.ToString(validUser.Color))); };
                    
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

                            
                    // To do these add a new AuthenticationProperties() { PropertyName = Value }, you can add this as an argument in SignInAsync()

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    // return Redirect(ReturnUrl == null ? "/Home/Index" : ReturnUrl);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Email or Password is Incorrect");
            }
            return View(user);
        }
     

        [HttpGet]
        [AllowAnonymous]
        // Displays the Register Store View
        public IActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister([Bind("StoreId, StoreName, StoreInviteCode")] Store store, [Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId")] User admin)
        {
            Random RNG = new Random();
            const string range = "abcdefghijklmnopqrstuvwxyz0123456789";
            var randomCode = Enumerable.Range(0, 5).Select(x => range[RNG.Next(0, range.Length)]);
            string code = new string(randomCode.ToArray());

            //Makes sure the code does not already exist in the database
            var existingStoreInviteCode = (from u in _context.Store
                                           where u.StoreInviteCode.Equals(code)
                                           select u).FirstOrDefault();

            while (existingStoreInviteCode != null)
            {
                RNG = new Random();
                randomCode = Enumerable.Range(0, 5).Select(x => range[RNG.Next(0, range.Length)]);
                code = new string(randomCode.ToArray());
                existingStoreInviteCode = (from u in _context.Store
                                           where u.StoreInviteCode.Equals(code)
                                           select u).FirstOrDefault();
            }

            store.StoreInviteCode = code;
            _context.Add(store);
            await _context.SaveChangesAsync();

            admin.InviteCode = code;
            admin.Color = "#000000";
            admin.RoleId = 3;
            
            if (ModelState.IsValid)
            {
                var existingEmail = (from u in _context.User
                                     where u.Email.Equals(admin.Email)
                                     select u).FirstOrDefault();

                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "Account already exists under this email! Please use a different one.");
                    return View(admin);
                }

                var existingStoreId = (from u in _context.Store
                                       where u.StoreInviteCode.Equals(code)
                                       select u.StoreId).FirstOrDefault();
                admin.StoreId = existingStoreId;

                // hashing password (salt is also applied)
                admin.Password = Crypto.HashPassword(admin.Password);
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError(string.Empty, "There was an issue creating an account.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private bool RegisterViewModelExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> Invite()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

    }
}