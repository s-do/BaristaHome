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
                Console.WriteLine(shift.UserId);
                if (DateTime.Compare(shift.StartShift, shift.EndShift) == 0 || DateTime.Compare(shift.StartShift, shift.EndShift) > 0)
                {
                    ModelState.AddModelError(string.Empty, "Oops, you either chose shifts at the same times, or your start shift is bigger than the end shift!");
                    ViewBag.openModal = true;
                    // Since ViewDatas are temporary, you have to add it in again once it's ran before returning the view
                    ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
                    return View(shift);
                }
                ViewBag.openModal = false;
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
                                    EventId = Convert.ToInt32(s.ShiftId),
                                    Title = u.FirstName + " " + u.LastName,
                                    Start = Convert.ToString(s.StartShift),
                                    End = Convert.ToString(s.EndShift),
                                    Color = u.Color,
                                }).ToListAsync();
            return Json(shifts);
        }

        /*// GET: Calendar/Edit/5
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
        }*/

        // POST: Calendar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            /*ViewData["StoreId"] = new SelectList(_context.Store, "StoreId", "StoreId", shift.StoreId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", shift.UserId);*/
            ModelState.AddModelError(string.Empty, "There was an error editing this shift.");
            ViewBag.openModal = true;
            ViewData["UserId"] = new SelectList(_context.User.Where(w => w.StoreId == Convert.ToInt16(User.FindFirst("StoreId").Value)), "UserId", "FirstName", shift.UserId);
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
