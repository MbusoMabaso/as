using System.Threading.Tasks;
using ClaimApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClaimApp.Controllers
{
    [Authorize(Roles = "Coordinator,Manager")]
    public class AdminController : Controller
    {
        private readonly IClaimService _svc;
        private readonly ILogger<AdminController> _log;

        public AdminController(IClaimService svc, ILogger<AdminController> log)
        {
            _svc = svc;
            _log = log;
        }

        // GET: /Admin/Pending
        public async Task<IActionResult> Pending()
        {
            var list = await _svc.GetPendingAsync();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _svc.ApproveAsync(id, User?.Identity?.Name);
                return RedirectToAction(nameof(Pending));
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "Approve failed");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Pending));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                await _svc.RejectAsync(id, User?.Identity?.Name, reason);
                return RedirectToAction(nameof(Pending));
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "Reject failed");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Pending));
            }
        }
    }
}