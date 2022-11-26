using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        //private readonly ILogger<PayrollController> _logger;
        private readonly BaristaHomeContext _context;


        public PayrollController(BaristaHomeContext context)
        {
            _context = context;
            //_logger = (ILogger<PayrollController>?)logger;
        }


/*        public PayrollController(ILogger<PayrollController> logger)
        {
            _logger = logger;
        }*/

/*        public IActionResult Owner()
        {
            return View();
        }
*/
        public IActionResult Worker()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Owner()
        {
            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).OrderBy(o => o.FullName).ToList();

            ViewBag.Payroll = payrolls;

            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            /* Pulls list of workers under the same store id for select list  */
            List<PayrollOwnerViewModel> users  = (from user in _context.User
                                join store in _context.Store on user.StoreId equals store.StoreId
                                where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                select new PayrollOwnerViewModel
                                {
                                    UserId = user.UserId,
                                    FullName = user.FirstName + " " + user.LastName,
                                }).ToList();

            ViewData["FullNames"] = new SelectList(users, "UserId", "FullName");

            return View();
        }


        /* Adds a payroll into the database */
        [HttpPost]
        public async Task<IActionResult> Owner(PayrollOwnerViewModel payrollOwnerViewModel)
        {
            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).OrderBy(o => o.FullName).ToList();
            ViewBag.Payroll = payrolls;

            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            List<PayrollOwnerViewModel> users = (from user in _context.User
                                                 join store in _context.Store on user.StoreId equals store.StoreId
                                                 where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                 select new PayrollOwnerViewModel
                                                 {
                                                     UserId = user.UserId,
                                                     FullName = user.FirstName + " " + user.LastName,
                                                 }).ToList();


            /* Shows the full name but when selected, it will give the userId as Value so database knows what user is being added */
            ViewData["FullNames"] = new SelectList(users, "UserId", "FullName");


            if (ModelState.IsValid)
            {
                var payroll1 = new Payroll()
                {
                    Hours = payrollOwnerViewModel.Hours,
                    Amount = payrollOwnerViewModel.Amount,
                    Date = payrollOwnerViewModel.Date,
                    UserId = payrollOwnerViewModel.UserId
                };

                _context.Add(payroll1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Owner));


            }

            ViewData["FullNames"] = new SelectList(users, "FullName", "FullName");
            ModelState.AddModelError(string.Empty, "There was an issue adding this item into your Payroll");
            return View();

        }


        // Get: Payroll/Delete/PayrollId
        public async Task<IActionResult> Delete(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var PayrollEntry = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);

            if (PayrollEntry == null)
            {
                return NotFound();
            }

            var PayrollName = (from i in _context.Payroll
                            where i.PayrollId == id
                            select i.PayrollId).FirstOrDefault();

            ViewBag.PayrollID = PayrollName;

            return View(PayrollEntry);
        }


        // POST: Payroll/Delete/PayrollId
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // requery the item and  d e l e t e
            var PayrollEntry = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);
            _context.Payroll.Remove(PayrollEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Owner));
        }



        // For Edit
        private bool PayrollOwnerViewModelExists(int payId)
        {
            return _context.Payroll.Any(e => e.PayrollId == payId);
        }


        // GET Edit Payroll
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editViewModel = await _context.Payroll.FirstOrDefaultAsync(m => m.PayrollId == id);


            if (editViewModel == null)
            {
                return NotFound();
            }
            return View(editViewModel);
        }


        // Edit Value in Payroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,Hours,Amount,Date,UserId")] Payroll editViewModel)
        {
            if (id != editViewModel.PayrollId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollOwnerViewModelExists(editViewModel.PayrollId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Owner));
            }
            return View(editViewModel);
        }



        // SEARCH BAR FEATURE
        // Shows the page again after searching the user wanted
        public async Task<IActionResult> SearchBarResults(int userId)
        {
              //var userId = Convert.ToInt32(userIdS);

            /*            List<ItemViewModel> itemQuery = (from store in _context.Store
                                                         join inventory in _context.InventoryItem on store.StoreId equals inventory.StoreId // link store and inventoryitem by storeid
                                                         join item in _context.Item on inventory.ItemId equals item.ItemId                  // link inventoryitem and item by itemid
                                                         join unit in _context.Unit on item.UnitId equals unit.UnitId                       // link item and unit by unitid
                                                         where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))       // filter items by user's store
                                                         select new ItemViewModel
                                                         {
                                                             Name = item.ItemName,                  // now we can send a 
                                                             Quantity = inventory.Quantity,         // ItemViewModel object
                                                             PricePerUnit = inventory.PricePerUnit, // to the view
                                                             UnitName = unit.UnitName,
                                                             ItemId = inventory.ItemId
                                                         }).Where(i => i.Name.Contains(searchPhrase)).ToList();
                        ViewBag.Inventory = itemQuery;
            */

            List<PayrollOwnerViewModel> payrolls = (from payroll in _context.Payroll
                                                    join user in _context.User on payroll.UserId equals user.UserId
                                                    join store in _context.Store on user.StoreId equals store.StoreId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    && payroll.UserId.Equals(userId)
                                                    select new PayrollOwnerViewModel
                                                    {
                                                        Hours = payroll.Hours,
                                                        Amount = payroll.Amount,
                                                        Date = payroll.Date,
                                                        UserId = user.UserId,
                                                        FullName = user.FirstName + " " + user.LastName,
                                                        PayrollId = payroll.PayrollId,
                                                    }).ToList();
            ViewBag.Payroll = payrolls;
            ViewData["FullNames"] = new SelectList(payrolls, "UserId", "FullName");

            /* Pulls list of workers under the same store id for select list  */
            List<PayrollOwnerViewModel> users = (from user in _context.User
                                                 join store in _context.Store on user.StoreId equals store.StoreId
                                                 where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                 select new PayrollOwnerViewModel
                                                 {
                                                     UserId = user.UserId,
                                                     FullName = user.FirstName + " " + user.LastName,
                                                 }).ToList();

            


            return View();

        }














    }
}
