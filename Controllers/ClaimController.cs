using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CMCS.Controllers
{
    public class ClaimController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<ClaimController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Claim
        public async Task<IActionResult> Index()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            var claims = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();
            
            return View(claims);
        }

        // GET: Claim/MyClaims - View only current user's claims
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

        // GET: Claim/Submit
        public IActionResult Submit()
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            var lecturerId = GetCurrentUserId();
            if (lecturerId == 0)
            {
                TempData["ErrorMessage"] = "Unable to identify current user.";
                return RedirectToAction("Login", "Account");
            }

            var claim = new Claim
            {
                DateSubmitted = DateTime.Now,
                LecturerID = lecturerId
            };
            return View(claim);
        }

        // POST: Claim/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Claim claim, IFormFileCollection? files)
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            var lecturerId = GetCurrentUserId();
            if (lecturerId == 0)
            {
                TempData["ErrorMessage"] = "Unable to identify current user.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values
                    claim.DateSubmitted = DateTime.Now;
                    claim.Status = ClaimStatus.Submitted;
                    claim.LecturerID = lecturerId; // Ensure the claim belongs to the current user

                    _context.Claims.Add(claim);
                    await _context.SaveChangesAsync();

                    // Handle file uploads
                    if (files != null && files.Count > 0)
                    {
                        await HandleFileUploads(claim.ClaimID, files);
                    }

                    TempData["SuccessMessage"] = "Claim submitted successfully!";
                    return RedirectToAction(nameof(Details), new { id = claim.ClaimID });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "An error occurred while saving the claim to the database.");
                    ModelState.AddModelError("", "An error occurred while submitting the claim. Please try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while submitting the claim.");
                    ModelState.AddModelError("", $"An unexpected error occurred while submitting the claim: {ex.Message}");
                }
            }

            return View(claim);
        }

        // GET: Claim/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var authCheck = RedirectToLoginIfNotAuthenticated();
            if (authCheck != null) return authCheck;

            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(m => m.ClaimID == id);

            if (claim == null)
            {
                return NotFound();
            }

            // Check if user can view this claim (either owner or admin role)
            var currentUserId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            
            if (claim.LecturerID != currentUserId && userRole != "AcademicManager" && userRole != "ProgrammeCoordinator")
            {
                TempData["ErrorMessage"] = "You do not have permission to view this claim.";
                return RedirectToAction("MyClaims");
            }

            return View(claim);
        }

        // GET: Claim/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        // POST: Claim/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Claim claim)
        {
            if (id != claim.ClaimID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.ClaimID))
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
            return View(claim);
        }

        // GET: Claim/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(m => m.ClaimID == id);

            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // POST: Claim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                _context.Claims.Remove(claim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimExists(int id)
        {
            return _context.Claims.Any(e => e.ClaimID == id);
        }

        private async Task HandleFileUploads(int claimId, IFormFileCollection files)
        {
            // Secure file upload path - prevent directory traversal
            var safeClaimId = claimId.ToString().Replace("..", "").Replace("/", "").Replace("\\", "");
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "claims", safeClaimId);
            
            // Ensure the uploads directory exists and is within the web root
            var uploadsBasePath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsBasePath))
            {
                Directory.CreateDirectory(uploadsBasePath);
            }
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    try
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".doc", ".xls" };
                        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            throw new InvalidOperationException($"File type {fileExtension} is not allowed. Only PDF, DOCX, and XLSX files are permitted.");
                        }

                        // Validate file size (10MB limit)
                        if (file.Length > 10 * 1024 * 1024)
                        {
                            throw new InvalidOperationException("File size cannot exceed 10MB.");
                        }

                        var fileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var document = new Document
                        {
                            ClaimID = claimId,
                            FileName = file.FileName,
                            FilePath = filePath,
                            FileSize = file.Length,
                            FileType = fileExtension,
                            UploadDate = DateTime.Now
                        };

                        _context.Documents.Add(document);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while uploading a file.");
                        // We can choose to either stop the entire claim submission or just skip the problematic file.
                        // For now, we will just log the error and continue.
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
