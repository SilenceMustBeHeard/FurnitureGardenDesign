using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{

    [Authorize]
    public abstract class BaseController : Controller
    {
        protected BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected bool IsUserAdmin() => User.IsInRole("Admin");

        protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



        private readonly UserManager<ApplicationUser> _userManager;

        protected string GetUserId() => _userManager.GetUserId(User);
        protected async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

    }
}
