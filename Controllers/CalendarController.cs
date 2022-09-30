using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaristaHome.Data;
using BaristaHome.Models;
using BaristaHome.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly BaristaHomeContext _context;

        public CalendarController(BaristaHomeContext context)
        {
            _context = context;
        }

        // GET: Calendar
        public async Task<IActionResult> Index()
        {
            var baristaHomeContext = _context.Shift.Include(s => s.Store).Include(s => s.User);
/*            foreach(Shift shift in baristaHomeContext)
            {
                shift.StartShift.
            }*/
            return View(await baristaHomeContext.ToListAsync());
        }

        public IActionResult Index1()
        {
            return View();
        }

        // GET: Calendar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shift == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
                .Include(s => s.Store)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ShiftId == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // GET: Calendar/Create
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName");
            
            return View();
        }

        // POST: Calendar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([Bind("ShiftId,StartShift,EndShift,ShiftDate,UserId,StoreId")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(shift.UserId);
                if (DateTime.Compare(shift.StartShift, shift.EndShift) == 0)
                {
                    ModelState.AddModelError(string.Empty, "The shift times can't be the same dumbass.");
                    // Since ViewDatas are temporary, you have to add it in again once it's ran before returning the view
                    ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
                    return View(shift);
                }
                // Adding the selected date to start/end times to keep dates consistent
                shift.StartShift = shift.ShiftDate.Date + shift.StartShift.TimeOfDay;
                shift.EndShift = shift.ShiftDate.Date + shift.EndShift.TimeOfDay;

                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "There was an error creating a new shift.");
            ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
            return View(shift);
        }

        // GET: Calendar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shift == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "StoreId", shift.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", shift.UserId);
            return View(shift);
        }

        // POST: Calendar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShiftId,StartShift,EndShift,ShiftDate,UserId,StoreId")] Shift shift)
        {
            if (id != shift.ShiftId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftExists(shift.ShiftId))
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
            ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "StoreId", shift.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", shift.UserId);
            return View(shift);
        }

        // GET: Calendar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shift == null)
            {
                return NotFound();
            }

            var shift = await _context.Shift
                .Include(s => s.Store)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.ShiftId == id);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // POST: Calendar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shift == null)
            {
                return Problem("Entity set 'BaristaHomeContext.Shift'  is null.");
            }
            var shift = await _context.Shift.FindAsync(id);
            if (shift != null)
            {
                _context.Shift.Remove(shift);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftExists(int id)
        {
          return _context.Shift.Any(e => e.ShiftId == id);
        }
    }
}
