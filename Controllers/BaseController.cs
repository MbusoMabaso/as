using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        protected int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return int.TryParse(userId, out int id) ? id : 0;
        }

        protected string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "";
        }

        protected string GetCurrentUsername()
        {
            return HttpContext.Session.GetString("Username") ?? "";
        }

        protected IActionResult? RedirectToLoginIfNotAuthenticated()
        {
            if (!IsUserLoggedIn())
            {
                TempData["ErrorMessage"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        protected IActionResult? RedirectToLoginIfNotAuthorized(string? requiredRole = null)
        {
            var redirectResult = RedirectToLoginIfNotAuthenticated();
            if (redirectResult != null) return redirectResult;

            if (!string.IsNullOrEmpty(requiredRole))
            {
                var userRole = GetCurrentUserRole();
                if (userRole != requiredRole)
                {
                    TempData["ErrorMessage"] = "You do not have permission to access this page.";
                    return RedirectToAction("Index", "Home");
                }
            }
            return null;
        }
    }
}
