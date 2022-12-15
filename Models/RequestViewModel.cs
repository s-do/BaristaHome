using BaristaHome.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaristaHome.Data;
using BaristaHome.Models;
using BaristaHome.Migrations;
using Microsoft.AspNetCore.Authorization;
using BaristaHome.Controllers;
using System.Security.Claims;
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
using Xunit.Sdk;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BaristaHome.Models
{
    public class RequestViewModel
    {
        
        public int UserId { get; set; }
       
        [Required(ErrorMessage = "Please select a shift to swap out.")]
        public int CurrentUserShiftId { get; set; }

        [Required(ErrorMessage = "Please select a shift to request for.")]
        public int RequestedShiftId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }



        /*
         private readonly BaristaHomeContext _context;
         public int CurrentUserShiftId { get; set; }
         public int RequestedShiftId { get; set; }
         public RequestViewModel(BaristaHomeContext context)
         {
             _context = context;
         }
         public static IEnumerable<SelectListItem> GetCurrentUserShifts()
         {
             int userId = CalendarController.getUserId();
             int storeId = CalendarController.getStoreId();

             BaristaHomeContext _context1 = new BaristaHomeContext();

             var currentUserShifts = (from store in _context1.Store
                               join user in _context1.User on store.StoreId equals user.StoreId
                               join shift in _context1.Shift on user.UserId equals shift.UserId
                               where store.StoreId.Equals(storeId) && user.UserId.Equals(Convert.ToInt16(User.FindFirst("UserId").Value))
                               select shift)
                 .Select(s => new
                 {
                     Text = s.StartShift + " to " + s.EndShift,
                     Value = s.ShiftId
                 })
                 .ToList();

             return new SelectList(currentUserShifts, "Value", "Text");
         }

         public static IEnumerable<SelectListItem> GetWorkerNameAndShifts()
         {
             int userId = CalendarController.getUserId();
             int storeId = CalendarController.getStoreId();

             BaristaHomeContext _context2 = new BaristaHomeContext();

             var workerNameAndShifts = (from store in _context2.Store
                                join user in _context2.User on store.StoreId equals user.StoreId
                                join shift in _context2.Shift on user.UserId equals shift.UserId
                                where store.StoreId.Equals(storeId)
                                select shift)
                             .Select(s => new
                             {
                                 Text = s.User.FirstName + " " + s.User.LastName + " - " + s.StartShift + " to " + s.EndShift,
                                 Value = s.ShiftId
                             })
                             .ToList();

             return new SelectList(workerNameAndShifts, "Value", "Text");
         }
        */

    }
}
