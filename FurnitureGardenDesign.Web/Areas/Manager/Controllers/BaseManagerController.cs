using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Manager.Controllers
{

    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public abstract class BaseManagerController : Controller
    {

        protected BaseManagerController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        protected bool IsUserManager() => User.IsInRole("Manager");

        protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



        private readonly UserManager<AppUser> _userManager;

        protected Guid GetUserId() => Guid.Parse(_userManager.GetUserId(User));
        protected async Task<AppUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
