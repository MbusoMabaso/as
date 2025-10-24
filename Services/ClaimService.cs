using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CMCS.Data;
using CMCS.Models;
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

        private readonly string[] _allowedExt = new[] { ".pdf", ".docx", ".xlsx", ".doc", ".xls" };
        private const long MaxFileBytes = 10 * 1024 * 1024; // 10MB

        public ClaimService(ApplicationDbContext db, IWebHostEnvironment env, ILogger<ClaimService> log)
        {
            _db = db;
            _env = env;
            _log = log;
        }

        public async Task<CMCS.Models.Claim> CreateAsync(CMCS.Models.Claim claim, IFormFile file)
        {
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            // basic validation
            if (claim.TotalHours <= 0 || claim.HourlyRate <= 0)
                throw new ArgumentException("Hours and rate must be greater than zero.");

            // Set submission time
            claim.DateSubmitted = DateTime.Now;

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            // handle file after claim has an id
            if (file != null && file.Length > 0)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExt.Contains(ext)) throw new ArgumentException("Invalid file type.");
                if (file.Length > MaxFileBytes) throw new ArgumentException("File exceeds maximum allowed size (10MB).");

                var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "claims", claim.ClaimID.ToString());
                Directory.CreateDirectory(uploads);
                var stored = $"{Guid.NewGuid():N}{ext}";
                var path = Path.Combine(uploads, stored);

                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("uploads", "claims", claim.ClaimID.ToString(), stored).Replace("\\", "/");

                var doc = new Document
                {
                    ClaimID = claim.ClaimID,
                    FileName = file.FileName,
                    FilePath = relativePath,
                    FileSize = file.Length,
                    FileType = file.ContentType ?? string.Empty,
                    UploadDate = DateTime.Now
                };

                _db.Documents.Add(doc);
                await _db.SaveChangesAsync();
            }

            return claim;
        }

        public async Task<CMCS.Models.Claim?> GetAsync(int id)
        {
            return await _db.Claims.Include(c => c.Documents).Include(c => c.Lecturer).FirstOrDefaultAsync(c => c.ClaimID == id);
        }

        public async Task<IEnumerable<CMCS.Models.Claim>> GetPendingAsync()
        {
            return await _db.Claims.Where(c => c.Status == CMCS.Models.ClaimStatus.Submitted).OrderByDescending(c => c.DateSubmitted).ToListAsync();
        }

        public async Task ApproveAsync(int id, string approverId)
        {
            var claim = await _db.Claims.FindAsync(id) ?? throw new KeyNotFoundException("Claim not found.");
            // Promote to final approved status
            claim.Status = CMCS.Models.ClaimStatus.ApprovedByAcademicManager;
            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(int id, string approverId, string? reason = null)
        {
            var claim = await _db.Claims.FindAsync(id) ?? throw new KeyNotFoundException("Claim not found.");
            claim.Status = CMCS.Models.ClaimStatus.Rejected;
            claim.RejectionReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            await _db.SaveChangesAsync();
        }
    }
}