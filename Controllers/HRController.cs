using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using System.Collections.Generic;

namespace CMCS.Controllers
{
    public class HRController : Controller
    {
        // Placeholder for HR-related data
        private static List<User> hrUsers = new List<User>();

        public IActionResult Index()
        {
            return View(hrUsers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User newUser)
        {
            if (ModelState.IsValid)
            {
                hrUsers.Add(newUser);
                return RedirectToAction(nameof(Index));
            }
            return View(newUser);
        }
    }
}