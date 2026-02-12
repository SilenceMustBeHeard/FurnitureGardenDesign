using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureGardenDesign.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService profileService;
        private readonly UserManager<AppUser> userManager;

        public ProfileController(
            IProfileService profileService,
            UserManager<AppUser> userManager)
        {
            this.profileService = profileService;
            this.userManager = userManager;
        }


        // gets the profile information of the currently logged-in user and displays it on the profile page.
        // If the user is not authenticated, it redirects them to the login page.
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var model = await profileService.GetProfileAsync(user.Id);
            return View(model);
        }


        // marks a specific message as read for the currently logged-in user
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            await profileService.MarkMessageAsReadAsync(id, user.Id);

            return RedirectToAction(nameof(Index));
        }
    }

}
