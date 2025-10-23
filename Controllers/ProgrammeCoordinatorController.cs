using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using System.Collections.Generic;
using CMCS.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    public class ProgrammeCoordinatorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgrammeCoordinatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // This action will handle the request for the dashboard view.
        public IActionResult Dashboard()
        {
            return View();
        }

        // Action to handle claims verification and approval
        public IActionResult Approve()
        {
            var claims = _context.Claims.Include(c => c.Lecturer).Where(c => c.Status == ClaimStatus.Submitted).ToList();
            return View(claims);
        }

        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.ApprovedByProgrammeCoordinator;
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Approve));
        }

        public async Task<IActionResult> RejectClaim(int id)
        {
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
