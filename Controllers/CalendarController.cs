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

        public IActionResult Shifts()
        {
            ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName");
            ViewBag.openModal = false;
            return View();
        }

        // POST: Calendar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Shifts([Bind("ShiftId,StartShift,EndShift,UserId,StoreId")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                // Error Handling
                if (DateTime.Compare(shift.StartShift, shift.EndShift) == 0 || DateTime.Compare(shift.StartShift, shift.EndShift) > 0)
                {
                    ModelState.AddModelError(string.Empty, "Oops, you either chose shifts at the same times, or your start shift is bigger than the end shift!");
                    ViewBag.openModal = true;
                    // Since ViewDatas are temporary, you have to add it in again once it's ran before returning the view
                    ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
                    return View(shift);
                }

                // Checking if user has an overlapping shift on that day 
                var existingShift = (from s in _context.Shift
                                    where s.UserId.Equals(shift.UserId) && ((shift.StartShift >= s.StartShift && shift.StartShift <= s.EndShift) || (shift.EndShift >= s.StartShift && shift.EndShift <= s.EndShift))
                                    select s).FirstOrDefault();
                if (existingShift != null)
                {
                    ModelState.AddModelError(string.Empty, "This worker has an existing shift that overlaps with this one! Choose a different time.");
                    ViewBag.openModal = true;
                    ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
                    return View(shift);
                }

                // Add shift and refresh page to update calendar
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Shifts));
            }
            ModelState.AddModelError(string.Empty, "There was an error creating a new shift.");
            ViewBag.openModal = true;
            ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
            return View(shift);
        }

        [HttpGet]
        public async Task<IActionResult> GetShifts(DateTime start, DateTime end)
        {
            Console.WriteLine(Convert.ToInt32(User.FindFirst("StoreId").Value));
            // Query the store's shifts
            var shifts = await (from s in _context.Shift
                                join u in _context.User on s.UserId equals u.UserId
                                where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value) && (s.StartShift >= start && s.EndShift < end)
                                // Then we need to feed the data as a json feed that abides to FullCalendar's property names
                                select new ShiftViewModel()
                                {
                                    Id = Convert.ToInt32(s.ShiftId),
                                    Title = u.FirstName + " " + u.LastName,
                                    Start = Convert.ToString(s.StartShift),
                                    End = Convert.ToString(s.EndShift),
                                    Color = u.Color,
                                    UserId = Convert.ToInt32(u.UserId)
                                }).ToListAsync();
            return Json(shifts);
        }

        // POST: Calendar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ShiftId,StartShift,EndShift,ShiftDate,UserId,StoreId")] Shift shift)
        {
            if (shift.ShiftId == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Error Handling
                    if (DateTime.Compare(shift.StartShift, shift.EndShift) == 0 || DateTime.Compare(shift.StartShift, shift.EndShift) > 0)
                    {
                        TempData["editShiftError"] = "Oops, you either chose shifts at the same times, or your start shift is bigger than the end shift!";
                        return RedirectToAction(nameof(Shifts));
                    }

                    // Checking if user has an overlapping shift on that day 
                    var existingShift = (from s in _context.Shift
                                        where !s.ShiftId.Equals(shift.ShiftId) && s.UserId.Equals(shift.UserId) && 
                                        ((shift.StartShift >= s.StartShift && shift.StartShift <= s.EndShift) || (shift.EndShift >= s.StartShift && shift.EndShift <= s.EndShift))
                                        select s).FirstOrDefault();
                    if (existingShift != null)
                    {
                        TempData["editShiftError"] = "This worker has an existing shift that overlaps with this one! Choose a different time.";
                        return RedirectToAction(nameof(Shifts));
                    }

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
                return RedirectToAction(nameof(Shifts));
            }
            TempData["editShiftError"] = "There was an error editing this shift.";
            return RedirectToAction(nameof(Shifts), shift);
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
