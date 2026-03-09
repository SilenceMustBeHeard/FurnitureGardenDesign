using FurnitureGardenDesign.Common;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Services.Core.Interfaces;
using FurnitureGardenDesign.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace FurnitureGardenDesign.Web.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly IProfileService _profileService;
        private readonly IInboxMessageService _inbo;

        public NavbarViewComponent(
                IInboxMessageService inboxMessageService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IOrderService orderService,
            IProfileService profileService)
        {
            _inbo = inboxMessageService;
            _signInManager = signInManager;
            _userManager = userManager;
            _orderService = orderService;
            _profileService = profileService;
        }



        // It checks if the user is logged in and retrieves their role information to determine which navbar buttons to display.

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new NavbarButtonsViewModel
            {
                IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User)
            };

            if (model.IsLoggedIn)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                model.IsAdmin = HttpContext.User.IsInRole(RoleNames.Admin);
                model.IsManager = HttpContext.User.IsInRole(RoleNames.Manager);
                model.IsUser = !model.IsAdmin && !model.IsManager;

                if (model.IsAdmin || model.IsManager)
                {
                    model.PendingOrdersCount = await _orderService.GetPendingOrdersCountAsync();
                }


                if (user != null)
                {
                    model.UnreadMessagesCount = await _inbo.GetUnreadCountAsync(user.Id);
                }
            }

            return View(model);
        }
    }
}