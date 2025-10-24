using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using System.Collections.Generic;

namespace CMCS.Controllers
{
    public class HRController : BaseController
    {
        // Placeholder for HR-related data
        private static List<User> hrUsers = new List<User>();

        public IActionResult Index()
        {
            var authCheck = RedirectToLoginIfNotAuthorized("HR");
            if (authCheck != null) return authCheck;
            
            return View(hrUsers);
        }

        public IActionResult Create()
        {
            var authCheck = RedirectToLoginIfNotAuthorized("HR");
            if (authCheck != null) return authCheck;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User newUser)
        {
            var authCheck = RedirectToLoginIfNotAuthorized("HR");
            if (authCheck != null) return authCheck;
            
            if (ModelState.IsValid)
            {
                hrUsers.Add(newUser);
                return RedirectToAction(nameof(Index));
            }
            return View(newUser);
        }
    }
}