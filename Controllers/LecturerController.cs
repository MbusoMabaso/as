using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class LecturerController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            // Get lecturer dashboard data
            var lecturerId = GetCurrentUserId();
            
            var dashboardData = new
            {
                TotalClaims = await _context.Claims.CountAsync(c => c.LecturerID == lecturerId),
                PendingClaims = await _context.Claims.CountAsync(c => c.LecturerID == lecturerId && c.Status == "Pending"),
                ApprovedClaims = await _context.Claims.CountAsync(c => c.LecturerID == lecturerId && c.Status == "Approved"),
                RejectedClaims = await _context.Claims.CountAsync(c => c.LecturerID == lecturerId && c.Status == "Rejected"),
                RecentClaims = await _context.Claims
                    .Where(c => c.LecturerID == lecturerId)
                    .OrderByDescending(c => c.DateSubmitted)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardData);
        }

        public IActionResult Dashboard()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;
            
            return View();
        }

        public async Task<IActionResult> MyClaims()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            var lecturerId = GetCurrentUserId();
            if (lecturerId == 0)
            {
                TempData["ErrorMessage"] = "Unable to identify current user.";
                return RedirectToAction("Login", "Account");
            }

            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Where(c => c.LecturerID == lecturerId)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();
            
            return View(claims);
        }

        public IActionResult Create()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lecturer newLecturer)
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;
            
            if (ModelState.IsValid)
            {
                _context.Lecturers.Add(newLecturer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newLecturer);
        }
    }
}
