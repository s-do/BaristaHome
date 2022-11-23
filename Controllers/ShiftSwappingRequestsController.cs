using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaristaHome.Data;
using BaristaHome.Models;

namespace BaristaHome.Controllers
{
    public class ShiftSwappingRequestsController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ShiftSwappingRequestsController(BaristaHomeContext context)
        {
            _context = context;
        }

        // GET: ShiftSwappingRequests
        public async Task<IActionResult> Index()
        {
            var baristaHomeContext = _context.ShiftSwappingRequest.Include(s => s.RecipientShift).Include(s => s.RecipientUser).Include(s => s.SenderShift).Include(s => s.SenderUser);
            return View(await baristaHomeContext.ToListAsync());
        }

        // GET: ShiftSwappingRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ShiftSwappingRequest == null)
            {
                return NotFound();
            }

            var shiftSwappingRequest = await _context.ShiftSwappingRequest
                .Include(s => s.RecipientShift)
                .Include(s => s.RecipientUser)
                .Include(s => s.SenderShift)
                .Include(s => s.SenderUser)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (shiftSwappingRequest == null)
            {
                return NotFound();
            }

            return View(shiftSwappingRequest);
        }

        // GET: ShiftSwappingRequests/Create
        public IActionResult Create()
        {
            ViewData["RecipientShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId");
            ViewData["RecipientUserId"] = new SelectList(_context.User, "UserId", "Email");
            ViewData["SenderShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId");
            ViewData["SenderUserId"] = new SelectList(_context.User, "UserId", "Email");
            return View();
        }

        // POST: ShiftSwappingRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,SenderUserId,RecipientUserId,SenderShiftId,RecipientShiftId,Response")] ShiftSwappingRequest shiftSwappingRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shiftSwappingRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipientShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.RecipientShiftId);
            ViewData["RecipientUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.RecipientUserId);
            ViewData["SenderShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.SenderShiftId);
            ViewData["SenderUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.SenderUserId);
            return View(shiftSwappingRequest);
        }

        // GET: ShiftSwappingRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ShiftSwappingRequest == null)
            {
                return NotFound();
            }

            var shiftSwappingRequest = await _context.ShiftSwappingRequest.FindAsync(id);
            if (shiftSwappingRequest == null)
            {
                return NotFound();
            }
            ViewData["RecipientShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.RecipientShiftId);
            ViewData["RecipientUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.RecipientUserId);
            ViewData["SenderShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.SenderShiftId);
            ViewData["SenderUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.SenderUserId);
            return View(shiftSwappingRequest);
        }

        // POST: ShiftSwappingRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,SenderUserId,RecipientUserId,SenderShiftId,RecipientShiftId,Response")] ShiftSwappingRequest shiftSwappingRequest)
        {
            if (id != shiftSwappingRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shiftSwappingRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftSwappingRequestExists(shiftSwappingRequest.RequestId))
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
            ViewData["RecipientShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.RecipientShiftId);
            ViewData["RecipientUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.RecipientUserId);
            ViewData["SenderShiftId"] = new SelectList(_context.Shift, "ShiftId", "ShiftId", shiftSwappingRequest.SenderShiftId);
            ViewData["SenderUserId"] = new SelectList(_context.User, "UserId", "Email", shiftSwappingRequest.SenderUserId);
            return View(shiftSwappingRequest);
        }

        // GET: ShiftSwappingRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ShiftSwappingRequest == null)
            {
                return NotFound();
            }

            var shiftSwappingRequest = await _context.ShiftSwappingRequest
                .Include(s => s.RecipientShift)
                .Include(s => s.RecipientUser)
                .Include(s => s.SenderShift)
                .Include(s => s.SenderUser)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (shiftSwappingRequest == null)
            {
                return NotFound();
            }

            return View(shiftSwappingRequest);
        }

        // POST: ShiftSwappingRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ShiftSwappingRequest == null)
            {
                return Problem("Entity set 'BaristaHomeContext.ShiftSwappingRequest'  is null.");
            }
            var shiftSwappingRequest = await _context.ShiftSwappingRequest.FindAsync(id);
            if (shiftSwappingRequest != null)
            {
                _context.ShiftSwappingRequest.Remove(shiftSwappingRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftSwappingRequestExists(int id)
        {
          return _context.ShiftSwappingRequest.Any(e => e.RequestId == id);
        }
    }
}
