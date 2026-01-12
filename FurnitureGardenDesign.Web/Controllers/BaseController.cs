using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{

    [Authorize]
    public abstract class BaseController : Controller
    {
        protected BaseController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected bool IsUserAdmin() => User.IsInRole("Admin");

        protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



        private readonly UserManager<AppUser> _userManager;

        protected string? GetUserId()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return null;

            return _userManager.GetUserId(User);
        }

        protected async Task<AppUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

    }
}
