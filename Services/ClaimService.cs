using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClaimApp.Data;
using ClaimApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClaimApp.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ClaimService> _log;

        private readonly string[] _allowedExt = new[] { ".pdf", ".docx", ".xlsx" };
        private const long MaxFileBytes = 5 * 1024 * 1024; // 5MB

        public ClaimService(ApplicationDbContext db, IWebHostEnvironment env, ILogger<ClaimService> log)
        {
            _db = db;
            _env = env;
            _log = log;
        }

        public async Task<Claim> CreateAsync(Claim claim, IFormFile file)
        {
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            // server-side validation
            if (claim.HoursWorked <= 0 || claim.HourlyRate <= 0)
                throw new ArgumentException("Hours and rate must be greater than zero.");

            if (file != null)
            {
                ValidateFile(file);
                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                Directory.CreateDirectory(uploads);

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var storedName = $"{Guid.NewGuid():N}{ext}";
                var path = Path.Combine(uploads, storedName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                claim.UploadedFileName = storedName;
                claim.OriginalFileName = file.FileName;
            }

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();
            return claim;
        }

        private void ValidateFile(IFormFile file)
        {
            if (file.Length == 0) throw new ArgumentException("Empty file.");
            if (file.Length > MaxFileBytes) throw new ArgumentException("File exceeds maximum allowed size (5MB).");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExt.Contains(ext)) throw new ArgumentException("Invalid file type.");
        }

        public async Task<Claim> GetAsync(int id)
        {
            return await _db.Claims.FindAsync(id);
        }

        public async Task<IEnumerable<Claim>> GetPendingAsync()
        {
            return await _db.Claims.Where(c => c.Status == ClaimStatus.Pending).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task ApproveAsync(int id, string approverId)
        {
            var claim = await _db.Claims.FindAsync(id) ?? throw new KeyNotFoundException("Claim not found.");
            claim.Status = ClaimStatus.Approved;
            claim.UpdatedAt = DateTime.UtcNow;
            _db.Update(claim);
            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(int id, string approverId, string reason = null)
        {
            var claim = await _db.Claims.FindAsync(id) ?? throw new KeyNotFoundException("Claim not found.");
            claim.Status = ClaimStatus.Rejected;
            claim.UpdatedAt = DateTime.UtcNow;
            // Could store rejection reason in another column/table - left minimal
            _db.Update(claim);
            await _db.SaveChangesAsync();
        }
    }
}

using System;
using System.Threading.Tasks;
using ClaimApp.Models;
using ClaimApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClaimApp.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _svc;
        private readonly ILogger<ClaimsController> _log;

        public ClaimsController(IClaimService svc, ILogger<ClaimsController> log)
        {
            _svc = svc;
            _log = log;
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            return View(new Claim());
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim model, Microsoft.AspNetCore.Http.IFormFile upload)
        {
            try
            {
                if (!ModelState.IsValid) return View(model);

                // Example: set LecturerId from logged-in user
                model.LecturerId = User?.Identity?.Name ?? "anonymous";

                var created = await _svc.CreateAsync(model, upload);
                TempData["Success"] = "Claim submitted successfully.";
                return RedirectToAction(nameof(Details), new { id = created.Id });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error creating claim");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var claim = await _svc.GetAsync(id);
            if (claim == null) return NotFound();
            return View(claim);
        }
    }
}