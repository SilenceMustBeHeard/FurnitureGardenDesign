using FurnitureGardenDesign.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Areas.Manager.Controllers.Account
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


        protected void SetToastMessage(string message, string type = "success")
        {
            Response.Headers.Add("X-Toast-Message", message);
            Response.Headers.Add("X-Toast-Type", type);
        }

        protected bool IsUserAuthenticated() => User.Identity?.IsAuthenticated ?? false;



        private readonly UserManager<AppUser> _userManager;

        protected Guid GetUserId() => Guid.Parse(_userManager.GetUserId(User));
        protected async Task<AppUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
