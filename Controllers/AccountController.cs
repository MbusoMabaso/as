using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid)
            {
                var foundUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username && u.Password == user.Password);
                
                if (foundUser != null)
                {
                    // Simple session-based authentication for prototype
                    HttpContext.Session.SetString("UserId", foundUser.UserID.ToString());
                    HttpContext.Session.SetString("UserRole", foundUser.Role ?? "");
                    HttpContext.Session.SetString("Username", foundUser.Username ?? "");
                    
                    TempData["SuccessMessage"] = $"Welcome back, {foundUser.Username}!";
                    
                    // Redirect based on role
                    return foundUser.Role switch
                    {
                        "AcademicManager" => RedirectToAction("Dashboard", "AcademicManager"),
                        "ProgrammeCoordinator" => RedirectToAction("Dashboard", "ProgrammeCoordinator"),
                        "HR" => RedirectToAction("Dashboard", "HR"),
                        _ => RedirectToAction("Index", "Lecturer")
                    };
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == user.Username);
                
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(user);
                }

                // Set default role for new users
                user.Role = "Lecturer";
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}