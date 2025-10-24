using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using System.Collections.Generic;
using CMCS.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class AcademicManagerController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AcademicManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var authCheck = RedirectToLoginIfNotAuthorized("AcademicManager");
            if (authCheck != null) return authCheck;
            
            return View();
        }

        // Action to handle claims verification and approval
        public IActionResult Approve()
        {
            var authCheck = RedirectToLoginIfNotAuthorized("AcademicManager");
            if (authCheck != null) return authCheck;
            
            var claims = _context.Claims.Include(c => c.Lecturer).Where(c => c.Status == ClaimStatus.ApprovedByProgrammeCoordinator).ToList();
            return View(claims);
        }

        public async Task<IActionResult> ApproveClaim(int id)
        {
            var authCheck = RedirectToLoginIfNotAuthorized("AcademicManager");
            if (authCheck != null) return authCheck;
            
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.ApprovedByAcademicManager;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Approve));
        }

        public async Task<IActionResult> RejectClaim(int id)
        {
            var authCheck = RedirectToLoginIfNotAuthorized("AcademicManager");
            if (authCheck != null) return authCheck;
            
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Rejected;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Approve));
        }

        public IActionResult Create()
        {
            var authCheck = RedirectToLoginIfNotAuthorized("AcademicManager");
            if (authCheck != null) return authCheck;
            
            return View();
        }

    }
}
