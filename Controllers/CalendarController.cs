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
using NuGet.Frameworks;

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
        /* PETER ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼ */

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
        [Authorize(Policy = "AdminOnly")]
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

        // POST: Calendar/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete([Bind("ShiftId")] Shift shift)
        {
            if (_context.Shift == null)
            {
                return Problem("Entity set 'BaristaHomeContext.Shift'  is null.");
            }
            var queryShift = await _context.Shift.FindAsync(shift.ShiftId);
            if (queryShift != null)
            {
                _context.Shift.Remove(queryShift);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Shifts));
        }

        private bool ShiftExists(int id)
        {
          return _context.Shift.Any(e => e.ShiftId == id);
        }
        /* PETER ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ */

        /* CINDIE ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼ */

        //Return Swap view with two dropdown menus, one populated with the shifts belonging to the current user, and the other populated with other coworker's shifts
        public async Task<IActionResult> Swap()
        {
            //Get current user's shifts
            var currentUserShifts = (from store in _context.Store
                                     join user in _context.User on store.StoreId equals user.StoreId
                                     join shift in _context.Shift on user.UserId equals shift.UserId
                                     where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value)) && user.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                                     select new RequestViewModel()
                                     {
                                         UserId = user.UserId,
                                         ShiftId = shift.ShiftId,
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         StartTime = shift.StartShift,
                                         EndTime = shift.EndShift
                                     }).Select(s => new
                                     {
                                         Text =  s.StartTime + " to " + s.EndTime,
                                         Value = s.ShiftId
                                     })
                                    .ToList(); 
            //Create a select list (to use in the view's dropdown menu)
            ViewData["CurrentUserShifts"] = new SelectList(currentUserShifts, "Value", "Text");

            //Get each coworker's names and shifts
            var workerNamesAndTheirShifts = (from store in _context.Store
                                     join user in _context.User on store.StoreId equals user.StoreId
                                     join shift in _context.Shift on user.UserId equals shift.UserId
                                     where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value)) && !user.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                                     select new RequestViewModel()
                                     {
                                         UserId = user.UserId,
                                         ShiftId = shift.ShiftId,
                                         ShiftId2 = shift.ShiftId,
                                         FirstName = user.FirstName,
                                         LastName = user.LastName,
                                         StartTime = shift.StartShift,
                                         EndTime = shift.EndShift
                                     }).Select(s => new
                                     {
                                         Text = s.FirstName + " " + s.LastName + " - " + s.StartTime + " to " + s.EndTime,
                                         Value = s.ShiftId
                                     })
                                    .ToList();

            //Create a select list (to use in the view's dropdown menu)
            ViewData["workerNamesAndTheirShifts"] = new SelectList(workerNamesAndTheirShifts, "Value", "Text");
 
            return View();

        }

        //Save the shift swapping request to the database
        [HttpPost]
        public async Task<IActionResult> Swap(int CurrentUserShiftId, int ShiftId2)
        {
            //Get sender user id, recipient user id, sender shift id, recipient shift id
            int senderUserId = Convert.ToInt16(User.FindFirst("UserId").Value);
            var getRecipientUserId = await _context.Shift.FindAsync(ShiftId2);
            int recipientUserId = getRecipientUserId.UserId;
            int recipientShiftId = ShiftId2;

            //Create a new shift swapping request, and set all attributes
            var request = new ShiftSwappingRequest();
            request.SenderUserId = senderUserId;
            request.RecipientUserId = recipientUserId;
            request.SenderShiftId = CurrentUserShiftId;
            request.RecipientShiftId = recipientShiftId;

            ViewBag.Error = "";
            if (ModelState.IsValid)
            {
                //Get all shift swapping requests from the database
                var allRequests = await _context.ShiftSwappingRequest.ToListAsync();

                //For each request, check if it has the same current user's shift id and recipient shift id
                foreach (ShiftSwappingRequest r in allRequests)
                {
                    if (r.SenderShiftId == CurrentUserShiftId && r.RecipientShiftId == ShiftId2)
                    {
                        //If it matches, display the swap request form again for the user to try again
                        ViewBag.Error = "This shift swapping request has already been made.";
                        return RedirectToAction("Swap");
                    }
                }
                try
                {
                    //Save the request to the database
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Shifts");
            }
            return RedirectToAction("Swap");

        }


        public async Task<IActionResult> WorkerRequests()
        {

            //Get a list of shift swapping requests to send to the worker requests view
            List<ShiftSwappingRequest> shiftRequests = (from store in _context.Store
                                        join user in _context.User on store.StoreId equals user.StoreId
                                        join shift in _context.Shift on user.UserId equals shift.UserId
                                        join shiftRequest in _context.ShiftSwappingRequest on shift.ShiftId equals shiftRequest.SenderShiftId
                                        where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                        where shiftRequest.SenderUserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value)) //Get shift swap requests where the sender is the current user
                                        let senderUser = (from store in _context.Store
                                                        join user in _context.User on store.StoreId equals user.StoreId
                                                        where user.UserId.Equals(Convert.ToInt16(shiftRequest.SenderUserId))
                                                        select user).FirstOrDefault()
                                        let senderShift = (from store in _context.Store
                                                            join shift in _context.Shift on store.StoreId equals shift.StoreId
                                                            where shift.ShiftId.Equals(Convert.ToInt16(shiftRequest.SenderShiftId))
                                                            select shift).FirstOrDefault()
                                        select new ShiftSwappingRequest()
                                        {
                                            RequestId = shiftRequest.RequestId,
                                            SenderUserId = shiftRequest.SenderUserId,
                                            RecipientUserId = shiftRequest.RecipientUserId,
                                            SenderUser = shiftRequest.SenderUser,
                                            RecipientUser = shiftRequest.RecipientUser,
                                            SenderShift = shiftRequest.SenderShift,
                                            RecipientShift = shiftRequest.RecipientShift,
                                            Response = shiftRequest.Response

                                        }).ToList();
            ViewBag.ShiftRequests = shiftRequests;

            //Set the status to either accepted, pending or declined
            ViewData["Status"] = "Accepted";
            return View(shiftRequests);
        }


        //Switches shifts between users based on request ID
        public async Task<IActionResult> SwapShifts(int RequestId)
        {
            //Get the shift swapping request based on request ID
            var ShiftSwappingRequest = await _context.ShiftSwappingRequest.FindAsync(RequestId);

            //Get the Sender's and Recipient's shift id
            var SenderShiftId = ShiftSwappingRequest.SenderShiftId;
            var RecipientShiftId = ShiftSwappingRequest.RecipientShiftId;

            //Use the shift ids to find the shifts
            var SenderShift = await _context.Shift.FindAsync(SenderShiftId);
            var RecipientShift = await _context.Shift.FindAsync(RecipientShiftId);

            //Get the Sender's and Recipient's user ids
            var SenderUserId = ShiftSwappingRequest.SenderUserId;
            var RecipientUserId = ShiftSwappingRequest.RecipientUserId;

            //Swap both shifts' user id's
            SenderShift.UserId = RecipientUserId;
            RecipientShift.UserId = SenderUserId;

            //Set the request's response to True (to show that the request is accepted and completed)
            ShiftSwappingRequest.Response = true;

            //Find list of swap requests that are requesting for the same shift as the one that was swapped above
            var shifts = (IEnumerable<ShiftSwappingRequest>)(from store in _context.Store
                                              join user in _context.User on store.StoreId equals user.StoreId
                                              join shift in _context.Shift on user.UserId equals shift.UserId
                                              join shiftRequest in _context.ShiftSwappingRequest on shift.ShiftId equals shiftRequest.RecipientShiftId
                                              where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                              where shiftRequest.RecipientShiftId.Equals(RecipientShiftId)
                                              where (!shiftRequest.RequestId.Equals(RequestId))
                                              select shiftRequest).ToList();

            //Go through each swap request and set the response to false
            foreach (ShiftSwappingRequest r in shifts)
            {
                r.Response = false;
                _context.ShiftSwappingRequest.Update(r);
                await _context.SaveChangesAsync();

            }

            //Update the database
            _context.Shift.Update(SenderShift);
            _context.Shift.Update(RecipientShift);
            _context.ShiftSwappingRequest.Update(ShiftSwappingRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Requests");
        }

        //Sets the swap request reponse to false (to indicate that it has been declined)
        public async Task<IActionResult> DeclineSwapRequest(int RequestId)
        {
            var ShiftSwappingRequest = await _context.ShiftSwappingRequest.FindAsync(RequestId);
            ShiftSwappingRequest.Response = false;
            _context.ShiftSwappingRequest.Update(ShiftSwappingRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Requests");

        }


        //Display current user's swap requests from other workers
        public IActionResult Requests()
        {
            //Get a list of shift swapping requests 
            List<ShiftSwappingRequest> shiftRequests = (from store in _context.Store
                                                    join user in _context.User on store.StoreId equals user.StoreId
                                                    join shift in _context.Shift on user.UserId equals shift.UserId
                                                    join shiftRequest in _context.ShiftSwappingRequest on shift.ShiftId equals shiftRequest.RecipientShiftId
                                                    where store.StoreId.Equals(Convert.ToInt16(User.FindFirst("StoreId").Value))
                                                    //the recipient must be the current user
                                                    where shiftRequest.RecipientUserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value)) 
                                                    //the request must be pending (not accepted/rejected)
                                                    where shiftRequest.Response.Equals(null) 
                                                    let senderUser = (from store in _context.Store
                                                                 join user in _context.User on store.StoreId equals user.StoreId
                                                                 where user.UserId.Equals(Convert.ToInt16(shiftRequest.SenderUserId))
                                                                 select user).FirstOrDefault()
                                                    let senderShift = (from store in _context.Store
                                                                      join shift in _context.Shift on store.StoreId equals shift.StoreId
                                                                      where shift.ShiftId.Equals(Convert.ToInt16(shiftRequest.SenderShiftId))
                                                                      select shift).FirstOrDefault()
                                                    select new ShiftSwappingRequest()
                                                    {
                                                        RequestId = shiftRequest.RequestId,
                                                        SenderUserId = shiftRequest.SenderUserId,
                                                        RecipientUserId = shiftRequest.RecipientUserId,
                                                        SenderUser = senderUser,
                                                        RecipientUser = user,
                                                        SenderShift = senderShift,
                                                        RecipientShift = shift

                                                    }).ToList();
            //Display swap requests
            ViewBag.ShiftRequests = shiftRequests;
            return View(shiftRequests);
        }

        /* CINDIE ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ */



    }
}
