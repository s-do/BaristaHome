using BaristaHome.Data;
using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace BaristaHome.Controllers
{
    [Authorize]
    public class ChecklistController : Controller
    {
        private readonly BaristaHomeContext _context;

        public ChecklistController(BaristaHomeContext context)
        {
            _context = context;
        }

        public IActionResult Checklist()
        {
            return View();
        }
    }
}
