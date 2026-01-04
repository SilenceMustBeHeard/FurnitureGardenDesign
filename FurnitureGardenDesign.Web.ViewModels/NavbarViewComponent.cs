namespace FurnitureGardenDesign.Web.ViewModels
{
    using FurnitureGardenDesign.Data.Models;
    using FurnitureGardenDesign.Data.Repository.Interfaces;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class NavbarViewComponent : ViewComponent
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderRepository _orderRepository;

        public NavbarViewComponent(SignInManager<AppUser> signInManager,
                                   UserManager<AppUser> userManager,
                                   IOrderRepository orderRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new NavbarButtonsViewModel
            {
                IsLoggedIn = _signInManager.IsSignedIn(HttpContext.User),
                IsAdmin = false,
                IsManager = false,
                NewOrdersCount = 0
            };

            if (model.IsLoggedIn)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    model.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    model.IsManager = await _userManager.IsInRoleAsync(user, "Manager");

                    if (model.IsAdmin || model.IsManager)
                    {
                     
                        model.NewOrdersCount = await _orderRepository
                            .CountAsync(o => o.Status == Furniture_GardenDesign.Data.Enums.OrderStatus.Pending);
                    }
                }
            }

            return View(model);
        }
    }

}
