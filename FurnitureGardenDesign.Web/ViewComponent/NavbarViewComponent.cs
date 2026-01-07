using FurnitureGardenDesign.Common;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;


namespace FurnitureGardenDesign.Web.ViewModels
{


    public class NavbarViewComponent : ViewComponent
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOrderService _orderService;

        public NavbarViewComponent(SignInManager<AppUser> signInManager,
                                   IOrderService orderService)
        {
            _signInManager = signInManager;
            _orderService = orderService;
        
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new NavbarButtonsViewModel
            {
                IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User)
            };

            if (model.IsLoggedIn)
            {
                model.IsAdmin = HttpContext.User.IsInRole(RoleNames.Admin);
                model.IsManager = HttpContext.User.IsInRole(RoleNames.Manager);
                model.IsUser = !model.IsAdmin && !model.IsManager;

                if (model.IsAdmin 
                    || model.IsManager)
                {
                    model.PendingOrdersCount = await _orderService.GetPendingOrdersCountAsync();
                }
            }

            return View(model);
        }
    }


}






