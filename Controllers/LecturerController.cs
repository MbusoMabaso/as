using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get lecturer dashboard data
            var lecturerId = 1; // This should come from the logged-in user
            
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
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lecturer newLecturer)
        {
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
