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
        // Displays the Register Store View
        public IActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
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
            admin.RoleId = 2;
            
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
            return RedirectToAction("Index", "Account");
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
            //Get the list of all users
            List<User>? list_of_users = await _context.User.ToListAsync();

            //Create a new list to store users that belong to the current store
            List<User> list_of_store_users = new List<User>();

            //Get the current user (which should be the owner/admin)
            var current_user = await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value);
            
            //Get their invite code
            string store_invite_code = current_user.InviteCode;

            //Go through the list of all users
            foreach (var u in list_of_users)
            {
                //And if their invite code is the same, add them to the new list
                if(store_invite_code != null && u.InviteCode != null)
                {
                    if (u.InviteCode.Equals(store_invite_code))
                    {
                        list_of_store_users.Add(u);
                    }
                }

            }
            //Pass the list of store users to the view
            return View(list_of_store_users);
        }

        public async Task<IActionResult> AccountProfile()
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

            var registerViewModel = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (registerViewModel == null)
            {
                return NotFound();
            }

            return View(registerViewModel);
        }

        // GET: Account/Edit/UserId
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

        // POST: Account/Edit/UserId (overload method)
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,InviteCode,RoleId")] User registerViewModel)
        {
            if (id.ToString().Equals(registerViewModel.UserId.ToString()))
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

                    //Workers only allowed to edit certain attributes
                    //Set the other attributes to the claim values, so that when we update the database, it doesn't become null
                    registerViewModel.RoleId = Convert.ToInt32(User.FindFirstValue("RoleId"));
                    registerViewModel.InviteCode = User.FindFirstValue("InviteCode");
                    registerViewModel.StoreId = Convert.ToInt32(User.FindFirstValue("StoreId"));
                    if(User.FindFirstValue("UserImage") != null)
                    {
                        registerViewModel.UserImage = User.FindFirstValue("UserImage");
                    }
                    if (User.FindFirstValue("UserImageData") != null) {
                        registerViewModel.UserImageData = Encoding.ASCII.GetBytes(User.FindFirstValue("UserImageData"));
                    }
                    registerViewModel.Color = User.FindFirstValue("Color");

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
                return View(registerViewModel);
/*                return RedirectToAction(nameof(Index));*/
            }
            return View(registerViewModel);
        }

        public async Task<IActionResult> OwnerEdit(int? id)
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

        /* I don't know if you were planning to keep your code in this controller and view, or move it to your own. If it was the latter, then I don't know why you didn't
         * do that in the first place so you don't CONFLICT my code in the main branch. So, here's an explanation of why it's important to organize where 
         * our controllers and views go. When you click the button Workers, what does the link on top of the address say? It should say something like 
         * https://localhost:6969/Account/Index right? And when we edit a worker, it says something like https://localhost:7187/Account/OwnerEdit/35, so we can see how
         * our controllers and views have their names in the links. However, do these address links logically reflect the task that we're doing here? 
         * We're trying to edit certain things of our workers right, (Wage, Description, Role), so our naming convention for our controller should reflect that. 
         * It's why Alex initially setup all those different controllers and view for us, so we can keep our code organized. So when we edit workers, we should see an
         * address link look like https://localhost:6969/WorkerManagment/Index and we can achieve this by keeping our code under the WorkerManagement controller
         * (probably rename it to Worker instead so the link looks more like https://localhost:6969/Worker/Index). You can see how the syntax of the links is like this:
         * https://localhost:6969/ControllerName/ActionMethodName and in your view folder it's the same name as your action method (as return Ciew() tries to look for 
         * the corresponding method's view based on the name). And obviously, you can see how keeping code with the same methods in the same controller can cause problems.
         * You inevitably conflict with my code in the main branch. Whose Edit() function is going to be used under the Account folder? Yours or mine? This is why we 
         * separate into different controllers, for different actions. Sure the methodology is the same, but your Edit() function edits a User's Wage, Description, and RoleId.
         * My Edit() function is supposed to edit FirstName, LastName, Email, and Password. So yeah, you need to move your stuff to your own controller, and basically fix
           our AccountController.cs (just copy the code from the main branch) because merging your work here will cause some bad conflicts when merging. */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OwnerEdit(int id, [Bind("UserId,FirstName,LastName,Email,Password,ConfirmPassword,Color,InviteCode,RoleId,StoreId,UserImageData,UserImage")] User worker)
        {
            // let's also keep the variable naming convention consistent (use camel case), as well as update it to something meaningful reflecting the action method
            if (!id.ToString().Equals(worker.UserId.ToString())) 
            {
                return NotFound();
            }

            // don't need this query, this is dealing with changing an email of a user (owners don't do that, users do)
            /*var existingEmail = (from u in _context.User
                                 where u.Email.Equals(registerViewModel.Email) && !u.UserId.Equals(registerViewModel.UserId)
                                 select u).FirstOrDefault();

            if (existingEmail != null)
            {
                ModelState.AddModelError(string.Empty, "The email you are trying to change already exists on another account! Please use a different one.");
                return View(registerViewModel);
            }*/
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegisterViewModelExists(worker.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return View(worker); <-- this is basically saying "hey i want to return the view with the inputted values i already put in
                return RedirectToAction(nameof(Index)); // what you want to do instead is go back a page after successfully editing a worker, so that's why i kept this
            }
            ModelState.AddModelError(string.Empty, "There was an error editing this worker.");
            return View(worker); // if it fails, you can add a model error like so above and return the view with the changed input still there because we passed in worker
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

        public async Task<IActionResult> Invite()
        {
            return View(await _context.User.FirstOrDefaultAsync(m => m.UserId.ToString() == User.FindFirst("UserId").Value));
        }

    }
}
